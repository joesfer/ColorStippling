using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Threading;
using System.IO;

namespace MultiClassBlueNoise
{
    public class Stippling
    {
        private static int HashCells { get { return 100; } }

        private void BuildRMatrix(double[] rDiag, int x, int y)
        {
            int c = rDiag.Length; // c = num classes
            for (int i = 0; i < c; ++i)
            {
                r[x, y, i, i] = rDiag[i];
            }
            // Sort the c classes into priority groups P with decreasing Ri
            List<Tuple<double, List<int>>> priorityGroups = new List<Tuple<double, List<int>>>();
            for (int i = 0; i < c; ++i)
            {
                bool found = false;
                List<Tuple<double, List<int>>>.Enumerator e = priorityGroups.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.Item1 == rDiag[i])
                    {
                        e.Current.Item2.Add(i);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    List<int> classList = new List<int>();
                    classList.Add(i);
                    Tuple<double, List<int>> pi = new Tuple<double, List<int>>(rDiag[i], classList);
                    priorityGroups.Add(pi);
                }
            }
            SortedDictionary<double, List<int>> P = new SortedDictionary<double, List<int>>();
            {
                List<Tuple<double, List<int>>>.Enumerator e = priorityGroups.GetEnumerator();
                while (e.MoveNext())
                {
                    P.Add(-e.Current.Item1, e.Current.Item2); // negate the r key to reverse the sorting
                }
            }

            HashSet<int> C = new HashSet<int>();
            double D = 0;
            for (int k = 0; k < P.Count; ++k)
            {
                List<int> Pk = P.ElementAt(k).Value;
                foreach (int c_i in Pk)
                {
                    C.Add(c_i);
                    D += 1.0 / (rDiag[c_i] * rDiag[c_i]);
                }

                foreach (int c_i in Pk)
                {
                    foreach (int c_j in C)
                    {
                        if (c_i != c_j)
                        {
                            r[x, y, c_i, c_j] = r[x, y, c_j, c_i] = 1.0 / Math.Sqrt(D);
                        }
                    }
                }
            }
        }

        public struct Sample_t
        {
            public double x, y;
            public int c; // class
        };

        public class StipplingSettings_t
        {
            [DisplayName("Source File"), CategoryAttribute("Source"), DescriptionAttribute("Input image file to stipple"),
             EditorAttribute(typeof(FilteredImageFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
            public string SourceFile { get; set; }
            [DisplayName("Image Resizing Factor"), CategoryAttribute("Source"), DescriptionAttribute("Factor of enlargements of the output image. Stippling points will be further apart than the source pixels, so you may need a larger output canvas to represent the same perceived color density.")]
            public float ImageResizingFactor { get; set; }
            [DisplayName("Total Samples"), CategoryAttribute("Stippling"), DescriptionAttribute("Total number of stippling samples to place.")]
            public int TotalSamples { get; set; }
            [DisplayName("Spacing"), CategoryAttribute("Stippling"), DescriptionAttribute("Distance between samples.")]
            public float Spacing { get; set; }
            [DisplayName("Retries"), CategoryAttribute("Stippling"), DescriptionAttribute("Number of attempts for random placing of each new sample before we give up and force it by removing existing samples.")]
            public int Retries { get; set; }
            [DisplayName("Output Image File"), CategoryAttribute("Output"), DescriptionAttribute("Path to the output image file."),
             EditorAttribute(typeof(FilteredImageFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
            public string OutputImageFile { get; set; }
            [DisplayName("Output Data File"), CategoryAttribute("Output"), DescriptionAttribute("Path to the output data file. This file contains an XML description of the stippling result which could be displayed with other applications"),
            EditorAttribute(typeof(FilteredXMLFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
            public string OutputDataFile { get; set; }
            [DisplayName("Update Frequency"), CategoryAttribute("Progress"), DescriptionAttribute("How often we update the preview image, in number of samples placed.")]
            public int UpdateFrequency { get; set; }

            public float dotSizeMultiplier;

            public StipplingSettings_t()
            {
                TotalSamples = 1000000;
                ImageResizingFactor = 1.5f;
                UpdateFrequency = 500;
                Spacing = 1.0f;
                Retries = 30;
                SourceFile = "";
                string prefix = DateTime.Now.ToFileTime() + "_";
                OutputDataFile = prefix + "stipplingData.xml";
                OutputImageFile = prefix + "stippling.png";
                dotSizeMultiplier = 1.0f;
            }

        }

        public class StipplingData_t
        {
            public virtual Color[]              colorBasis { get { return _colorBase; } set { _colorBase = value; } } // we need to override it in the serializer...
            public bool additiveBlending { get { return _additiveBlending; } set { _additiveBlending = value; } }
            public List<Sample_t> samples { get { return _samples; } set { _samples = value; } }
            public StipplingSettings_t settings { get { return _settings; } set { _settings = value; } }
            
            public StipplingData_t()
            {
                colorBasis = null;
                additiveBlending = true;
                samples = new List<Sample_t>();
                settings = new StipplingSettings_t();
            }

            private Color[]             _colorBase;
            private bool                _additiveBlending;
            private List<Sample_t>      _samples;
            private StipplingSettings_t _settings;
        }

        public RectangleF? cropRegion = null;

        List<Sample_t>[,] sampleHash = null;
        
        double[, , ,] r = null;
        public double[, , ,] RadiusMatrix { get { return r; } }

        int numClasses;
        public int NumClasses { get { return this.stipplingData.colorBasis.Length; } }
        
        private int imageWidth, imageHeight;
        public int OutputImageWidth { get { return imageWidth; } }
        public int OutputImageHeight { get { return imageHeight; } }
        
        public StipplingData_t stipplingData = new StipplingData_t();

        public void InitSampleHash()
        {
            sampleHash = new List<Sample_t>[HashCells, HashCells];
            for (int i = 0; i < HashCells; ++i)
                for (int j = 0; j < HashCells; ++j)
                    sampleHash[i, j] = new List<Sample_t>();
        }

        public delegate void UpdateDelegate(float percentage);
        public UpdateDelegate Update;
        public static void StartStippling(object data, BackgroundWorker worker)
        {
            Stippling stippling = (Stippling)data;

            stippling.InitSampleHash();

            Random rnd = new Random();

            if ( stippling.RadiusMatrix == null || stippling.imageWidth == 0 || stippling.imageHeight == 0 ) stippling.PrepareStippling(stippling.stipplingData.settings.Spacing, worker);

            int[] Ni = new int[stippling.NumClasses]; // target num samples per class
            {  
                for (int i = 0; i < stippling.NumClasses; ++i) Ni[i] = (int)((float)stippling.stipplingData.settings.TotalSamples / stippling.NumClasses);
            }

            //
            int[] ni = new int[stippling.NumClasses];
            for (int i = 0; i < stippling.NumClasses; ++i) ni[i] = 0;

            float[] fillRate = new float[stippling.NumClasses];

            double[] diag = new double[stippling.NumClasses];

            do
            {
                //  pick next sample class

                for (int i = 0; i < stippling.NumClasses; ++i)
                {
                    fillRate[i] = (float)ni[i] / Ni[i];
                }

                int c = 0;
                float m = fillRate.Min();
                for (; c < stippling.NumClasses; ++c) if (fillRate[c] == m) break;


                int numAttempts = 0;
                Sample_t s;
                do
                {
                    if (worker.WorkerSupportsCancellation && worker.CancellationPending) return;

                    if (stippling.cropRegion.HasValue)
                    {
                        double w = rnd.NextDouble() * stippling.cropRegion.Value.Width + stippling.cropRegion.Value.X;
                        double h = rnd.NextDouble() * stippling.cropRegion.Value.Height + stippling.cropRegion.Value.Y;

                        s.x = w;
                        s.y = h;
                    }
                    else
                    {
                        double cellX = (double)rnd.Next(0, HashCells) / HashCells;
                        double cellY = (double)rnd.Next(0, HashCells) / HashCells;
                        double w = rnd.NextDouble() / HashCells;
                        double h = rnd.NextDouble() / HashCells;

                        s.x = cellX + w;
                        s.y = cellY + h;
                    }


                    int sx = (int)(s.x * stippling.OutputImageWidth);
                    int sy = (int)(s.y * stippling.OutputImageHeight);

                    s.c = c;
                    numAttempts++;
                } while (numAttempts < stippling.stipplingData.settings.Retries && !stippling.AcceptSample(s));
                if (numAttempts < stippling.stipplingData.settings.Retries)
                {
                    stippling.AddSample(s);
                    ni[s.c]++;
                }
                else
                {
                    int sx = (int)(s.x * stippling.OutputImageWidth);
                    int sy = (int)(s.y * stippling.OutputImageHeight);
                    if (stippling.RadiusMatrix[sx, sy, s.c, s.c] > 0.1) continue; // the conflicting radius affects too many samples

                    List<Sample_t> conflict = stippling.GetConflictSamples(s);
                    if (stippling.Removable(conflict, s, fillRate))
                    {
                        foreach (Sample_t s_ in conflict)
                        {
                            stippling.RemoveSample(s_);
                            ni[s_.c]--;
                        }
                        stippling.AddSample(s);
                        ni[s.c]++;
                    }
                }

                if (worker != null && worker.WorkerReportsProgress && 
                    stippling.stipplingData.samples.Count % stippling.stipplingData.settings.UpdateFrequency == 0)
                {
                    worker.ReportProgress((int)(100 * (float)stippling.stipplingData.samples.Count / stippling.stipplingData.settings.TotalSamples), String.Format("Stippling... {0} samples placed", stippling.stipplingData.samples.Count));
                }

            } while (stippling.stipplingData.samples.Count < stippling.stipplingData.settings.TotalSamples);

        }

        public bool PrepareStippling(double spacing, BackgroundWorker worker)
        {
            if(stipplingData.settings.SourceFile.Length == 0 ||
                !File.Exists(stipplingData.settings.SourceFile))
            {
                return false;
            }

            Bitmap source = new Bitmap(stipplingData.settings.SourceFile);

            numClasses = stipplingData.colorBasis.Length;

            double[] rDiag = new double[numClasses];
            Bitmap resizedSource = new Bitmap((int)((float)Math.Max(1, source.Width * stipplingData.settings.ImageResizingFactor)),
                                       (int)((float)Math.Max(1, source.Height * stipplingData.settings.ImageResizingFactor)));
            {
                Graphics g = Graphics.FromImage(resizedSource);
                g.DrawImage(source, new Rectangle(0, 0, resizedSource.Width, resizedSource.Height));
            }
            imageWidth = resizedSource.Width;
            imageHeight = resizedSource.Height;
            r = new double[imageWidth, imageHeight, numClasses, numClasses];
            double pixelSize = 1.0 / imageWidth;

            for (int y = 0; y < imageHeight; ++y)
            {
                if (worker.WorkerSupportsCancellation && worker.CancellationPending) return false;
                if (worker.WorkerReportsProgress) worker.ReportProgress((int)((float)y / imageHeight * 100), "Building conflict check matrix");

                for (int x = 0; x < imageWidth; ++x)
                {
                    Color col = resizedSource.GetPixel(x, y);
                    for (int c = 0; c < stipplingData.colorBasis.Length; ++c)
                    {
                        double component;

                        if (stipplingData.additiveBlending)
                        {
                            double dR = (((double)stipplingData.colorBasis[c].R / 255) * ((double)col.R / 255));
                            double dG = (((double)stipplingData.colorBasis[c].G / 255) * ((double)col.G / 255));
                            double dB = (((double)stipplingData.colorBasis[c].B / 255) * ((double)col.B / 255));
                            component = 1.0 / Math.Max(double.Epsilon, Math.Sqrt(dR * dR + dG * dG + dB * dB));
                        }
                        else
                        {
                            double dR = ((1.0 - (double)stipplingData.colorBasis[c].R / 255) * ((double)col.R / 255));
                            double dG = ((1.0 - (double)stipplingData.colorBasis[c].G / 255) * ((double)col.G / 255));
                            double dB = ((1.0 - (double)stipplingData.colorBasis[c].B / 255) * ((double)col.B / 255));
                            double dist = Math.Sqrt(dR * dR + dG * dG + dB * dB);
                            component = 1.0 / Math.Max(double.Epsilon, 1.0 - dist);
                        }

                        rDiag[c] = component * pixelSize * spacing;
                    }

                    BuildRMatrix(rDiag, x, y);
                }
            }
            return true;
        }

        private void AddSample(Sample_t s)
        {
            int hashX = (int)(s.x * HashCells);
            int hashY = (int)(s.y * HashCells);
            lock (stipplingData) // prevent the collection from being modified while we may be baking a preview
            {
                stipplingData.samples.Add(s);
            }
            sampleHash[hashX, hashY].Add(s);
        }

        private void RemoveSample(Sample_t s)
        {
            int hashX = (int)(s.x * HashCells);
            int hashY = (int)(s.y * HashCells);
            lock (stipplingData) // prevent the collection from being modified while we may be baking a preview
            {
                stipplingData.samples.Remove(s);
            }
            sampleHash[hashX, hashY].Remove(s);
        }

        private List<Sample_t> NearbySamples(double x, double y, double radius)
        {
            List<Sample_t> nearby = new List<Sample_t>();
            double cellSize = 1.0 / HashCells;
            int cellSpan = (int)Math.Ceiling(radius / cellSize);
            int hashX = (int)(x * HashCells);
            int hashY = (int)(y * HashCells);
            double radius2 = radius * radius;
            for (int j = Math.Max(0, hashY - cellSpan); j <= Math.Min(HashCells - 1, hashY + cellSize); ++j)
                for (int i = Math.Max(0, hashX - cellSpan); i <= Math.Min(HashCells - 1, hashX + cellSize); ++i)
                    foreach (Sample_t s in sampleHash[i, j])
                    {
                        double dx = x - s.x;
                        double dy = y - s.y;
                        double d = dx * dx + dy * dy;
                        if ( d <= radius2 ) nearby.Add(s);
                    }

            return nearby;
        }

        private bool AcceptSample(Sample_t s)
        {
            double radius = 0;
            for (int c = 0; c < numClasses; ++c) radius = Math.Max(radius, r[(int)(s.x * imageWidth), (int)(s.y * imageHeight), s.c, c]);

            int sx = (int)(s.x * imageWidth);
            int sy = (int)(s.y * imageHeight);

            List<Sample_t> nearby = NearbySamples(s.x, s.y, radius);
            foreach (Sample_t s_ in nearby)
            {
                double dx = s_.x - s.x;
                double dy = s_.y - s.y;
                double d = Math.Sqrt(dx * dx + dy * dy);
                if ( d <= r[sx, sy, s.c, s_.c]) return false;
            }
            return true;
        }

        List<Sample_t> GetConflictSamples(Sample_t s)
        {
            List<Sample_t> conflict = new List<Sample_t>();
            double radius = 0;
            for (int c = 0; c < numClasses; ++c) radius = Math.Max(radius, r[(int)(s.x * imageWidth), (int)(s.y * imageHeight), s.c, c]);
            double cellSize = 1.0 / HashCells;
            int cellSpan = (int)Math.Ceiling(radius / cellSize);
            int hashX = (int)(s.x * HashCells);
            int hashY = (int)(s.y * HashCells);


            for (int j = Math.Max(0, hashY - cellSpan); j <= Math.Min(HashCells - 1, hashY + cellSpan); ++j)
                for (int i = Math.Max(0, hashX - cellSpan); i <= Math.Min(HashCells - 1, hashX + cellSpan); ++i)
                    foreach (Sample_t s_ in sampleHash[i, j])
            {
                double dx = s.x - s_.x;
                double dy = s.y - s_.y;
                double d = Math.Sqrt(dx * dx + dy * dy);

                double radiusS = r[(int)(s.x * imageWidth), (int)(s.y * imageHeight), s.c, s_.c];
                double radiusS_ = r[(int)(s_.x * imageWidth), (int)(s_.y * imageHeight), s_.c, s.c];

                if (d <= (radiusS + radiusS_) * 0.5) conflict.Add(s_);
                //if (d <= radiusS) conflict.Add(s_);
            }
            return conflict;
        }

        private bool Removable(List<Sample_t> conflict, Sample_t s, float[] fillRate)
        {
            int sx = (int)(s.x * imageWidth);
            int sy = (int)(s.y * imageHeight);
            foreach (Sample_t s_ in conflict)
            {
                int sx_ = (int)(s_.x * imageWidth);
                int sy_ = (int)(s_.y * imageHeight);
                if (r[sx_, sy_, s_.c, s_.c] >= r[sx, sy, s.c, s.c] ||
                     fillRate[s_.c] < fillRate[s.c]) return false;
            }
            return true;
        }

        internal void Clear()
        {
            stipplingData.samples.Clear();
            r = null;
            imageWidth = 0;
            imageHeight = 0;
        }
    }
}
