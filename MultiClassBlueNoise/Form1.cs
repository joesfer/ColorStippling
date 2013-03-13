#define USE_HASHING

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Threading;
using System.Reflection;

namespace MultiClassBlueNoise
{
    public partial class ColorStippling : Form
    {
  
        public ColorStippling()
        {
            InitializeComponent();
        }

        Stippling stippling = new Stippling();
      
        private void goButton_Click(object sender, EventArgs e)
        {
            if (stipplingWorker.IsBusy || prepareStipplingWorker.IsBusy)
            {
                if (prepareStipplingWorker.IsBusy) prepareStipplingWorker.CancelAsync();
                if (stipplingWorker.IsBusy) stipplingWorker.CancelAsync();
                this.goButton.Text = "Go";
                splitContainer2.Panel1.Enabled = true;
                progressBar1.Value = 0;
                statusReportLabel.Text = "...";
                return;
            }
            
            if (stippling.stipplingData.settings.SourceFile.Length == 0 ||
                !File.Exists(stippling.stipplingData.settings.SourceFile))
            {
                MessageBox.Show("Please specify a valid input image file", "Unable to start stippling", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            stippling.stipplingData.colorBasis = ((ColorBasisItem)(colorBasisListBox.SelectedItem)).basis;
            stippling.stipplingData.additiveBlending = ((ColorBasisItem)(colorBasisListBox.SelectedItem)).additiveBlending;

            stipplingWorker.RunWorkerAsync();

            splitContainer2.Panel1.Enabled = false;
            snapshotPanel.Enabled = true;
            this.goButton.Text = "Stop";
            inputColorBasisList.Items.Clear();
        }

        private string BuidOutputImagePath() { return Path.GetFileNameWithoutExtension(stippling.stipplingData.settings.OutputImageFile) + ".png";  }

        public void UpdateResult(float percentage)
        {
            BakePreview();

            if (inputColorBasisList.Items.Count == 0)
                // we need to wait for the first update before creating these
                // because we need data created by the stippling thread
                CreateColorBasisInputThumbnails(true);

            if (!serializerWorker.IsBusy) serializerWorker.RunWorkerAsync();

            progressBar1.Value = (int)((float)progressBar1.Maximum * percentage);
        }

        void UpdatePreviewBitmapMainThread(Bitmap bmp) 
        { 
            pictureBox1.Image = bmp;
            mostRecentPreview = bmp;
        }

        private Bitmap mostRecentPreview = null;
        private void BakePreview()
        {
            if (bakePreviewWorker.IsBusy)
            {
                previewDirty = true;
                bakePreviewWorker.CancelAsync();
            }
            else
            {
                bakePreviewWorker.RunWorkerAsync();
            }
        }

        private Bitmap CreateMockupPreview(int translateX, int translateY, float scale)
        {
            if (mostRecentPreview == null) return null;
            if (translateX == 0 && translateY == 0 && Math.Abs(scale - 1.0f) < 0.0001f) return mostRecentPreview;

            Bitmap mockupPreview = new Bitmap(mostRecentPreview.Width, mostRecentPreview.Height);
            Graphics g = Graphics.FromImage(mockupPreview);
            g.Clear(Color.FromArgb(32,32,32));
            int halfW = (int)((mostRecentPreview.Width / 2) * scale);
            int halfH = (int)((mostRecentPreview.Height / 2) * scale);
            int x = mostRecentPreview.Width / 2 - halfW + translateX;
            int y = mostRecentPreview.Height / 2 - halfH + translateY;
            g.DrawImage(mostRecentPreview, x, y, 2 * halfW, 2 * halfH);
            return mockupPreview;
        }

        private bool previewDirty = false;
        private Bitmap UpdatePreview(string outputPath, int longestAxisRes, int previewBasisIndex, BackgroundWorker backgroundWorker)
        {
            if (stippling.OutputImageWidth == 0 || stippling.OutputImageHeight == 0) return null;

            lock (stippling.stipplingData)
            {
                Bitmap bmp;
                if (longestAxisRes > 0)
                {
                    // override resolution
                    float aspectRatio = (float)stippling.OutputImageWidth / stippling.OutputImageHeight;
                    if (stippling.OutputImageWidth >= stippling.OutputImageHeight)
                    {
                        bmp = new Bitmap(longestAxisRes, (int)(longestAxisRes / aspectRatio));
                    }
                    else
                    {
                        bmp = new Bitmap((int)(longestAxisRes * aspectRatio), longestAxisRes);
                    }
                }
                else
                {
                    bmp = new Bitmap(stippling.OutputImageWidth, stippling.OutputImageHeight);
                }

                float radius = (1.0f / previewWindow.Width) * stippling.stipplingData.settings.dotSizeMultiplier;

                bool previewAllColors = previewBasisIndex == 0;

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    if (stippling.stipplingData.additiveBlending) g.Clear(Color.Black);
                    else g.Clear(Color.White);
                    {
                        int brushSize = (int)(2 * radius);
                        int numSamples = 0;
                        foreach (Stippling.Sample_t smp in stippling.stipplingData.samples)
                        {
                            numSamples++;
                            if (backgroundWorker != null && numSamples % 100 == 0) backgroundWorker.ReportProgress((int)(100 * (float)numSamples / stippling.stipplingData.samples.Count));
                            if (backgroundWorker != null & backgroundWorker.WorkerSupportsCancellation && backgroundWorker.CancellationPending) return null;
                            if (!(previewAllColors || smp.c == previewBasisIndex - 1)) continue;
                            if (!previewWindow.Contains((float)smp.x, (float)smp.y)) continue;

                            int x = (int)((smp.x - previewWindow.X) / previewWindow.Width * bmp.Width);
                            int y = (int)((smp.y - previewWindow.Y) / previewWindow.Height * bmp.Height);
                            if (brushSize > 1)
                            {
                                for (int y_ = 0; y_ < brushSize; ++y_)
                                {
                                    for (int x_ = 0; x_ < brushSize; ++x_)
                                    {
                                        int dx = x_ - brushSize / 2;
                                        int dy = y_ - brushSize / 2;
                                        if (dx * dx + dy * dy > radius * radius) continue;

                                        int px = x + x_ - brushSize / 2;
                                        int py = y + y_ - brushSize / 2;
                                        if (px < 0 || px >= bmp.Width || py < 0 || py >= bmp.Height) continue;

                                        Color foreground = stippling.stipplingData.colorBasis[smp.c];
                                        Color background = bmp.GetPixel(px, py);
                                        Color result;
                                        if (stippling.stipplingData.additiveBlending)
                                            result = Color.FromArgb(Math.Min(255, foreground.R + background.R),
                                                                     Math.Min(255, foreground.G + background.G),
                                                                     Math.Min(255, foreground.B + background.B));
                                        else
                                        {
                                            float fgR = (float)foreground.R / 255;
                                            float fgG = (float)foreground.G / 255;
                                            float fgB = (float)foreground.B / 255;
                                            float bgR = (float)background.R / 255;
                                            float bgG = (float)background.G / 255;
                                            float bgB = (float)background.B / 255;

                                            result = Color.FromArgb((int)(255 * fgR * bgR), (int)(255 * fgG * bgG), (int)(255 * fgB * bgB));
                                        }
                                        bmp.SetPixel(px, py, result);
                                    }
                                }
                            }
                            else
                            {
                                Color foreground = stippling.stipplingData.colorBasis[smp.c];
                                Color background = bmp.GetPixel(x, y);
                                Color result;
                                if (stippling.stipplingData.additiveBlending)
                                    result = Color.FromArgb(Math.Min(255, foreground.R + background.R),
                                                             Math.Min(255, foreground.G + background.G),
                                                             Math.Min(255, foreground.B + background.B));
                                else
                                {
                                    float fgR = (float)foreground.R / 255;
                                    float fgG = (float)foreground.G / 255;
                                    float fgB = (float)foreground.B / 255;
                                    float bgR = (float)background.R / 255;
                                    float bgG = (float)background.G / 255;
                                    float bgB = (float)background.B / 255;

                                    result = Color.FromArgb((int)(255 * fgR * bgR), (int)(255 * fgG * bgG), (int)(255 * fgB * bgB));
                                }
                                bmp.SetPixel(x, y, result);
                            }
                        }
                    }
                }

                if (outputPath.Length > 0) bmp.Save(outputPath);

                return bmp;
            }
        }

        private void CreateColorBasisInputThumbnails(bool inputImage)
        {
            
            inputColorBasisList.Items.Clear();
            ImageList largeImages = new ImageList();
            int thumbnailSize = 64;

            if (inputImage)
            {
                int scaleFactor = Math.Max(4, (int)Math.Ceiling((float)Math.Min(stippling.OutputImageWidth, stippling.OutputImageHeight) / thumbnailSize));
                float[,] hdrBitmap = new float[stippling.OutputImageWidth / scaleFactor, stippling.OutputImageHeight / scaleFactor];
                for (int i = 0; i < stippling.NumClasses; i++)
                {
                    List<float> histogram = new List<float>((stippling.OutputImageWidth / scaleFactor) * (stippling.OutputImageHeight / scaleFactor));
                    for (int y = 0; y < stippling.OutputImageHeight / scaleFactor; y++)
                        for (int x = 0; x < stippling.OutputImageWidth / scaleFactor; x++)
                        {
                            float b = (float)Math.Max(float.MinValue, (stippling.RadiusMatrix[x * scaleFactor, y * scaleFactor, i, i] / stippling.stipplingData.settings.Spacing) + 1);
                            b = (float)Math.Log10(b);
                            if (float.IsInfinity(b) || float.IsNaN(b)) b = 0;
                            hdrBitmap[x, y] = b;
                            histogram.Add(b);
                        }

                    histogram.Sort();
                    float min = histogram[(int)(histogram.Count * 0.1)];
                    float max = histogram[(int)(histogram.Count * 0.9)];
                    max = Math.Max(max, min + 1e-4f);
                    Bitmap bmp = new Bitmap(stippling.OutputImageWidth / scaleFactor, stippling.OutputImageHeight / scaleFactor);
                    largeImages.ImageSize = bmp.Size;
                    for (int y = 0; y < bmp.Height; y++)
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            Color c = stippling.stipplingData.colorBasis[i];
                            float b = (hdrBitmap[x, y] - min) / (max - min);
                            b = 1.0f - Math.Max(0.0f, Math.Min(1.0f, b));
                            c = Color.FromArgb((int)(c.R * b), (int)(c.G * b), (int)(c.B * b));

                            bmp.SetPixel(x, y, c);
                        }
                    largeImages.Images.Add(bmp);
                    inputColorBasisList.Items.Add(new ListViewItem(stippling.stipplingData.colorBasis[i].ToString(), i));
                }
                
            }
            else
            {
                // color swatches only
                for (int i = 0; i < stippling.NumClasses; i++)
                {
                    Bitmap bmp = new Bitmap(thumbnailSize, thumbnailSize);
                    largeImages.ImageSize = bmp.Size;
                    Graphics g = Graphics.FromImage(bmp);
                    g.Clear(stippling.stipplingData.colorBasis[i]);
                    largeImages.Images.Add(bmp);
                    inputColorBasisList.Items.Add(new ListViewItem(stippling.stipplingData.colorBasis[i].ToString(), i));
                }
                
            }
            inputColorBasisList.LargeImageList = largeImages;
        }
       
        private RectangleF previewWindow = new RectangleF(0, 0, 1, 1);
        private Point lastMousePos = new Point(-1, -1);
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (lastMousePos.X >= 0 && lastMousePos.Y >= 0)
            {
                if (e.Button == MouseButtons.Left)
                {
                    int deltaX = e.X - lastMousePos.X;
                    int deltaY = e.Y - lastMousePos.Y;
                    float dx = (float)deltaX / pictureBox1.Width;
                    float dy = (float)deltaY / pictureBox1.Height;
                    previewWindow.X -= dx * previewWindow.Width;
                    previewWindow.Y -= dy * previewWindow.Height;

                    if (mostRecentPreview != null)
                    {
                        int tx = (int)(dx * mostRecentPreview.Width);
                        int ty = (int)(dy * mostRecentPreview.Height);
                        UpdatePreviewBitmapMainThread(CreateMockupPreview(tx, ty, 1.0f));
                    }
                    BakePreview();
                }
                if (e.Button == MouseButtons.Right)
                {
                    float w = previewWindow.Width;
                    float h = previewWindow.Height;
                    int deltaX = e.X - lastMousePos.X;
                    float delta = (float)deltaX / pictureBox1.Width;
                    if (delta > 0)
                    {
                        previewWindow.X += w * delta * 0.5f;
                        previewWindow.Y += h * delta * 0.5f;
                        previewWindow.Width *= (1.0f - delta);
                        previewWindow.Height *= (1.0f - delta);
                    }
                    else
                    {
                        previewWindow.X += w * delta * 0.5f;
                        previewWindow.Y += h * delta * 0.5f;
                        previewWindow.Width *= (1.0f - delta);
                        previewWindow.Height *= (1.0f - delta);
                    }

                    if (mostRecentPreview != null)
                    {
                        UpdatePreviewBitmapMainThread(CreateMockupPreview(0, 0, 1.0f + delta));
                    }

                    BakePreview();
                }          
            }
            lastMousePos.X = e.X;
            lastMousePos.Y = e.Y;
        }

        struct ColorBasisItem
        {
            public ColorBasisItem(string name, bool additiveBlend, params Color[] basis)
            {
                this.name = name;
                this.additiveBlending = additiveBlend;
                this.basis = new Color[basis.Length];
                for (int i = 0; i < basis.Length; ++i) this.basis[i] = basis[i];
            }
            public override string ToString() { return name; }

            private string name;
            public Color[] basis;

            public bool additiveBlending;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            colorBasisListBox.Items.Add( new ColorBasisItem("(+) R G B", 
                                                            true,
                                                            Color.Red,
                                                            Color.Lime, 
                                                            Color.Blue) );
            colorBasisListBox.Items.Add(new ColorBasisItem("(+) R G B W", 
                                                            true,
                                                            Color.Red,
                                                            Color.Lime, 
                                                            Color.Blue,
                                                            Color.White ));
            colorBasisListBox.Items.Add(new ColorBasisItem("(-) C M Y K", 
                                                            false,
                                                            Color.Cyan,
                                                            Color.Magenta,
                                                            Color.Yellow,
                                                            Color.Black));
            colorBasisListBox.Items.Add(new ColorBasisItem("(+) R G B C M Y LP LC LY O LG V",
                                                            true,
                                                            Color.Red, Color.Lime, Color.Blue,
                                                            Color.Cyan, Color.Magenta, Color.Yellow,
                                                            Color.LightPink, Color.LightCyan, Color.LightYellow,
                                                            Color.Orange, Color.LightGreen, Color.Violet));
            colorBasisListBox.Items.Add(new ColorBasisItem("(+) White", true, Color.White));
            colorBasisListBox.Items.Add(new ColorBasisItem("(-) Black", false, Color.Black));

            colorBasisListBox.SelectedIndex = 0;

            //
            propertyGrid1.SelectedObject = stippling.stipplingData.settings;

            //
            snapshotPanel.Enabled = false;

        }


        private void snapshotButton_Click(object sender, EventArgs e)
        {
            try
            {
                int longestAxisRes = int.Parse(snapshotSizeMaskedTextBox1.Text);
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.FileName = stippling.stipplingData.settings.OutputImageFile;
                dlg.Filter = "PNG Image (*.png)|*.png";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    UpdatePreview(dlg.FileName, longestAxisRes, 0, null);
                }
            }
            catch (System.Exception)
            {
            	
            }

        }

        private void colorBasisListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (colorBasisListBox.SelectedIndex == -1)
            {
                mostRecentPreview = null;
                return;
            }

            if (mostRecentPreview != null)
            {                
                if (MessageBox.Show("Changing the color basis will discard the current stippling data.\nDo you want to continue?", "Changing color basis", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    // abort color basis change
                    return;
                }
                else
                {
                    ClearStippling();

                }
            }

            previewBasisComboBox.Items.Clear();
            ColorBasisItem sel = (ColorBasisItem)colorBasisListBox.SelectedItem;
            if (sel.basis.Length > 1)
            {
                previewBasisComboBox.Items.Add(sel.ToString());
            }

            foreach (Color c in sel.basis)
            {
                previewBasisComboBox.Items.Add(c.ToString());
            }

            previewBasisComboBox.SelectedIndex = 0;
            
            stippling.stipplingData.colorBasis = ((ColorBasisItem)(colorBasisListBox.SelectedItem)).basis;

            CreateColorBasisInputThumbnails(false);
        }

        private void ClearStippling()
        {
            // clear previous samples and color basis
            stippling.stipplingData.samples = new List<Stippling.Sample_t>();
            snapshotPanel.Enabled = false;
            BakePreview();
        }
        private void pointSizeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangePointSize();
        }

        private void pointSizeComboBox_TextChanged(object sender, EventArgs e)
        {
            ChangePointSize();
        }

        private void ChangePointSize()
        {
            try
            {
                stippling.stipplingData.settings.dotSizeMultiplier = float.Parse(pointSizeComboBox.Text);
                BakePreview();
            }
            catch (Exception)
            {
                pointSizeComboBox.Text = stippling.stipplingData.settings.dotSizeMultiplier.ToString();
            }
        }

        private int previewBasisIndex = 0; // store it in a local variable so that we can access it from the worker thread without need for an Invoke()
        private void previewBasisComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            previewBasisIndex = previewBasisComboBox.SelectedIndex;
            BakePreview();
        }

     
        private void clearButton_Click(object sender, EventArgs e)
        {
            ClearStippling();
        }

        #region bakePreviewWorker
        private void bakePreviewWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            previewDirty = false;
            BackgroundWorker worker = sender as BackgroundWorker;
            e.Result = UpdatePreview(BuidOutputImagePath(), 0, previewBasisIndex, worker); // this may take a while, better no halt the UI thread and still do it on the worker thread
        }

        private void bakePreviewWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            previewBakeProgressBar.Value = (int)((float)e.ProgressPercentage / 100 * previewBakeProgressBar.Maximum);
        }

        private void bakePreviewWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (previewDirty == false) UpdatePreviewBitmapMainThread(e.Result as Bitmap);
            else bakePreviewWorker.RunWorkerAsync();
        }
        #endregion

        #region stipplingWorker
        private void stipplingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            Stippling.StartStippling(stippling, worker);
        }

        private void stipplingWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateResult((float)e.ProgressPercentage / 100);
            if (e.UserState != null) statusReportLabel.Text = e.UserState as string;
        }
        #endregion

        #region Serialization
        public class StipplingSerializedData : Stippling.StipplingData_t
        {
            public StipplingSerializedData() { }

            public StipplingSerializedData(ref Stippling.StipplingData_t source)
            {
                this.samples = source.samples;
                this.settings = source.settings;
                this.colorBasis = source.colorBasis;
                this.additiveBlending = source.additiveBlending;
            }

            [XmlElement("ColorBasis")]
            public string[] colorBasisHtml
            {
                get { return _colorBasisHtml; }
                set { _colorBasisHtml = value; }
            }

            [XmlIgnore] // the Xml serializer doesn't like System.Drawing.Color, we'll use a proxy property instead
            public override Color[] colorBasis
            {
                get 
                {
                    if (_colorBasis == null)
                    {
                        if (colorBasisHtml != null && colorBasisHtml.Length > 0)
                        {
                            _colorBasis = new Color[colorBasisHtml.Length];
                            for (int i = 0; i < _colorBasis.Length; ++i) _colorBasis[i] = ColorTranslator.FromHtml(colorBasisHtml[i]);
                        }
                    }
                    return _colorBasis;
                }

                set
                {
                    _colorBasis = null;
                    if (value != null && value.Length > 0)
                    {
                        colorBasisHtml = new string[value.Length];
                        for (int i = 0; i < value.Length; ++i) colorBasisHtml[i] = ColorTranslator.ToHtml(value[i]);
                    }
                }
            }

            private string[] _colorBasisHtml = null;
            private Color[] _colorBasis = null;
           
        };

        private void serializerWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // the xml serializer won't work with Color[] colorBasis, so let's ignore it and use the proxy property colorBasisHTML instead
                var overrides = new XmlAttributeOverrides();
                var attributes = new XmlAttributes { XmlIgnore = true };
                overrides.Add(typeof(Stippling.StipplingData_t), "colorBasis", attributes);

                XmlSerializer serializer = new XmlSerializer(typeof(StipplingSerializedData),overrides);                
                using (var writer = new StreamWriter(stippling.stipplingData.settings.OutputDataFile))
                {   
                    serializer.Serialize(writer, new StipplingSerializedData(ref stippling.stipplingData));
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.Message);
            }
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "XML Files|*.xml";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                stippling.stipplingData.samples = new List<Stippling.Sample_t>();

                XmlSerializer serializer = new XmlSerializer(typeof(StipplingSerializedData));
                using (var reader = new StreamReader(dlg.FileName))
                {
                    try
                    {
                        StipplingSerializedData data = (StipplingSerializedData)serializer.Deserialize(reader);
                        stippling.stipplingData = (Stippling.StipplingData_t)data;
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(String.Format("Unable to load data file {0}\nError {1}", dlg.FileName, ex.Message));
                        return;
                    }
                    
                    goButton.Enabled = false;

                    propertyGrid1.SelectedObject = stippling.stipplingData.settings;
                    prepareStipplingWorker.RunWorkerAsync();

                    // try finding this basis in the list, or add it if new
                    {
                        colorBasisListBox.SelectedIndex = -1;
                        for (int i = 0; i < colorBasisListBox.Items.Count; ++i)
                        {
                            Color[] basis = ((ColorBasisItem)colorBasisListBox.Items[i]).basis;
                            if (basis.Length == stippling.stipplingData.colorBasis.Length)
                            {
                                bool match = true;
                                for (int j = 0; match && j < basis.Length; ++j)
                                {
                                    if (basis[j] != stippling.stipplingData.colorBasis[j]) match = false;
                                }
                                if (match)
                                {
                                    colorBasisListBox.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                        if (colorBasisListBox.SelectedIndex == -1)
                        {
                            string basisName = "";
                            if (stippling.stipplingData.additiveBlending) basisName += "(+) ";
                            else basisName += "(-) ";
                            for (int i = 0; i < stippling.stipplingData.colorBasis.Length; ++i) basisName += stippling.stipplingData.colorBasis[i].ToString();
                            colorBasisListBox.Items.Add(new ColorBasisItem(basisName, stippling.stipplingData.additiveBlending, stippling.stipplingData.colorBasis));
                            colorBasisListBox.SelectedIndex = colorBasisListBox.Items.Count - 1;
                        }
                    }
                }
            }
        }
        #endregion
        #region Prepare Stippling worker
        private void prepareStipplingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            e.Result = stippling.PrepareStippling(stippling.stipplingData.settings.Spacing, worker);
        }

        private void prepareStipplingWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = (int)((float)e.ProgressPercentage / 100 * progressBar1.Maximum);
            if (e.UserState != null) statusReportLabel.Text = e.UserState as string;
        }

        private void prepareStipplingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Result as Boolean?) == true)
            {
                CreateColorBasisInputThumbnails(true);
                goButton.Enabled = true;
                BakePreview();
            }
            else
            {
                if (stippling.stipplingData.settings.SourceFile.Length == 0 ||
               !File.Exists(stippling.stipplingData.settings.SourceFile))
                {
                    MessageBox.Show(String.Format("Unable to load source file '{0}' referenced in settings.", stippling.stipplingData.settings.SourceFile), "Unable to prepare stippling", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion
    }
}
