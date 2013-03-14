namespace MultiClassBlueNoise
{
    partial class ColorStippling
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.loadButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.colorBasisListBox = new System.Windows.Forms.ListBox();
            this.splitter4 = new System.Windows.Forms.Splitter();
            this.label3 = new System.Windows.Forms.Label();
            this.inputColorBasisList = new System.Windows.Forms.ListView();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.snapshotPanel = new System.Windows.Forms.Panel();
            this.snapshotButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.snapshotSizeMaskedTextBox1 = new System.Windows.Forms.MaskedTextBox();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.goButton = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.previewBasisComboBox = new System.Windows.Forms.ComboBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pointSizeComboBox = new System.Windows.Forms.ComboBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.splitter5 = new System.Windows.Forms.Splitter();
            this.pictureBoxTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.previewBakeProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.bakePreviewWorker = new System.ComponentModel.BackgroundWorker();
            this.stipplingWorker = new System.ComponentModel.BackgroundWorker();
            this.serializerWorker = new System.ComponentModel.BackgroundWorker();
            this.prepareStipplingWorker = new System.ComponentModel.BackgroundWorker();
            this.statusReportLabel = new System.Windows.Forms.Label();
            this.cropRegionCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.snapshotPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.cropRegionCheckBox);
            this.splitContainer1.Panel2.Controls.Add(this.statusReportLabel);
            this.splitContainer1.Panel2.Controls.Add(this.goButton);
            this.splitContainer1.Panel2.Controls.Add(this.progressBar1);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.previewBasisComboBox);
            this.splitContainer1.Panel2.Controls.Add(this.pictureBox1);
            this.splitContainer1.Panel2.Controls.Add(this.pointSizeComboBox);
            this.splitContainer1.Panel2.Controls.Add(this.splitter1);
            this.splitContainer1.Panel2.Controls.Add(this.splitter5);
            this.splitContainer1.Size = new System.Drawing.Size(1202, 717);
            this.splitContainer1.SplitterDistance = 421;
            this.splitContainer1.TabIndex = 5;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.label3);
            this.splitContainer2.Panel2.Controls.Add(this.inputColorBasisList);
            this.splitContainer2.Panel2.Controls.Add(this.splitter2);
            this.splitContainer2.Panel2.Controls.Add(this.snapshotPanel);
            this.splitContainer2.Panel2.Controls.Add(this.splitter3);
            this.splitContainer2.Size = new System.Drawing.Size(421, 717);
            this.splitContainer2.SplitterDistance = 477;
            this.splitContainer2.TabIndex = 13;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.propertyGrid1);
            this.splitContainer3.Panel1.Controls.Add(this.splitContainer4);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.label5);
            this.splitContainer3.Panel2.Controls.Add(this.colorBasisListBox);
            this.splitContainer3.Panel2.Controls.Add(this.splitter4);
            this.splitContainer3.Size = new System.Drawing.Size(421, 477);
            this.splitContainer3.SplitterDistance = 314;
            this.splitContainer3.TabIndex = 0;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 34);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(421, 280);
            this.propertyGrid1.TabIndex = 14;
            this.propertyGrid1.ToolbarVisible = false;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.loadButton);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.clearButton);
            this.splitContainer4.Size = new System.Drawing.Size(421, 34);
            this.splitContainer4.SplitterDistance = 203;
            this.splitContainer4.TabIndex = 18;
            // 
            // loadButton
            // 
            this.loadButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.loadButton.Location = new System.Drawing.Point(0, 0);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(203, 30);
            this.loadButton.TabIndex = 23;
            this.loadButton.Text = "Load Data File";
            this.pictureBoxTooltip.SetToolTip(this.loadButton, "Load a generated data file to continue a previous stippling job");
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.clearButton.Location = new System.Drawing.Point(0, 0);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(214, 30);
            this.clearButton.TabIndex = 24;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 17);
            this.label5.TabIndex = 19;
            this.label5.Text = "Color Basis";
            // 
            // colorBasisListBox
            // 
            this.colorBasisListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.colorBasisListBox.FormattingEnabled = true;
            this.colorBasisListBox.ItemHeight = 16;
            this.colorBasisListBox.Location = new System.Drawing.Point(0, 24);
            this.colorBasisListBox.Name = "colorBasisListBox";
            this.colorBasisListBox.Size = new System.Drawing.Size(421, 135);
            this.colorBasisListBox.TabIndex = 11;
            this.colorBasisListBox.SelectedIndexChanged += new System.EventHandler(this.colorBasisListBox_SelectedIndexChanged);
            // 
            // splitter4
            // 
            this.splitter4.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter4.Location = new System.Drawing.Point(0, 0);
            this.splitter4.Name = "splitter4";
            this.splitter4.Size = new System.Drawing.Size(421, 24);
            this.splitter4.TabIndex = 17;
            this.splitter4.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(146, 17);
            this.label3.TabIndex = 16;
            this.label3.Text = "Converted Input Basis";
            // 
            // inputColorBasisList
            // 
            this.inputColorBasisList.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.inputColorBasisList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inputColorBasisList.Location = new System.Drawing.Point(0, 24);
            this.inputColorBasisList.Name = "inputColorBasisList";
            this.inputColorBasisList.Size = new System.Drawing.Size(421, 180);
            this.inputColorBasisList.TabIndex = 13;
            this.inputColorBasisList.UseCompatibleStateImageBehavior = false;
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter2.Location = new System.Drawing.Point(0, 0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(421, 24);
            this.splitter2.TabIndex = 15;
            this.splitter2.TabStop = false;
            // 
            // snapshotPanel
            // 
            this.snapshotPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.snapshotPanel.Controls.Add(this.snapshotButton);
            this.snapshotPanel.Controls.Add(this.label1);
            this.snapshotPanel.Controls.Add(this.snapshotSizeMaskedTextBox1);
            this.snapshotPanel.Location = new System.Drawing.Point(0, 204);
            this.snapshotPanel.Name = "snapshotPanel";
            this.snapshotPanel.Size = new System.Drawing.Size(421, 32);
            this.snapshotPanel.TabIndex = 21;
            // 
            // snapshotButton
            // 
            this.snapshotButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.snapshotButton.Location = new System.Drawing.Point(215, 3);
            this.snapshotButton.Name = "snapshotButton";
            this.snapshotButton.Size = new System.Drawing.Size(199, 23);
            this.snapshotButton.TabIndex = 22;
            this.snapshotButton.Text = "Snapshot";
            this.pictureBoxTooltip.SetToolTip(this.snapshotButton, "Use the Snapshot feature to generate images of a chosen resolution.");
            this.snapshotButton.UseVisualStyleBackColor = true;
            this.snapshotButton.Click += new System.EventHandler(this.snapshotButton_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 17);
            this.label1.TabIndex = 23;
            this.label1.Text = "Pixels along longest axis";
            // 
            // snapshotSizeMaskedTextBox1
            // 
            this.snapshotSizeMaskedTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.snapshotSizeMaskedTextBox1.Location = new System.Drawing.Point(164, 3);
            this.snapshotSizeMaskedTextBox1.Mask = "00000";
            this.snapshotSizeMaskedTextBox1.Name = "snapshotSizeMaskedTextBox1";
            this.snapshotSizeMaskedTextBox1.Size = new System.Drawing.Size(39, 22);
            this.snapshotSizeMaskedTextBox1.TabIndex = 24;
            this.snapshotSizeMaskedTextBox1.Text = "1024";
            this.snapshotSizeMaskedTextBox1.ValidatingType = typeof(int);
            // 
            // splitter3
            // 
            this.splitter3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter3.Location = new System.Drawing.Point(0, 204);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(421, 32);
            this.splitter3.TabIndex = 18;
            this.splitter3.TabStop = false;
            // 
            // goButton
            // 
            this.goButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.goButton.Location = new System.Drawing.Point(3, 688);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(125, 23);
            this.goButton.TabIndex = 12;
            this.goButton.Text = "Go";
            this.goButton.UseVisualStyleBackColor = true;
            this.goButton.Click += new System.EventHandler(this.goButton_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(134, 688);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(404, 23);
            this.progressBar1.TabIndex = 23;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(502, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(131, 17);
            this.label2.TabIndex = 21;
            this.label2.Text = "Point Size Multiplier";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(220, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(94, 17);
            this.label4.TabIndex = 20;
            this.label4.Text = "Preview Color";
            // 
            // previewBasisComboBox
            // 
            this.previewBasisComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.previewBasisComboBox.FormattingEnabled = true;
            this.previewBasisComboBox.Location = new System.Drawing.Point(320, 1);
            this.previewBasisComboBox.Name = "previewBasisComboBox";
            this.previewBasisComboBox.Size = new System.Drawing.Size(176, 24);
            this.previewBasisComboBox.TabIndex = 3;
            this.previewBasisComboBox.SelectedIndexChanged += new System.EventHandler(this.previewBasisComboBox_SelectedIndexChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 34);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(777, 651);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 18;
            this.pictureBox1.TabStop = false;
            this.pictureBoxTooltip.SetToolTip(this.pictureBox1, "Left mouse + Drag to pan\r\nRight mouse + Drag to zoom");
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // pointSizeComboBox
            // 
            this.pointSizeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pointSizeComboBox.FormattingEnabled = true;
            this.pointSizeComboBox.Items.AddRange(new object[] {
            "1.0",
            "1.5",
            "2.0",
            "4.0",
            "8.0",
            "16.0"});
            this.pointSizeComboBox.Location = new System.Drawing.Point(637, 0);
            this.pointSizeComboBox.Name = "pointSizeComboBox";
            this.pointSizeComboBox.Size = new System.Drawing.Size(121, 24);
            this.pointSizeComboBox.TabIndex = 15;
            this.pointSizeComboBox.Text = "1.0";
            this.pointSizeComboBox.SelectedIndexChanged += new System.EventHandler(this.pointSizeComboBox_SelectedIndexChanged);
            this.pointSizeComboBox.TextChanged += new System.EventHandler(this.pointSizeComboBox_TextChanged);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(777, 34);
            this.splitter1.TabIndex = 14;
            this.splitter1.TabStop = false;
            // 
            // splitter5
            // 
            this.splitter5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter5.Location = new System.Drawing.Point(0, 685);
            this.splitter5.Name = "splitter5";
            this.splitter5.Size = new System.Drawing.Size(777, 32);
            this.splitter5.TabIndex = 22;
            this.splitter5.TabStop = false;
            // 
            // pictureBoxTooltip
            // 
            this.pictureBoxTooltip.AutoPopDelay = 5000;
            this.pictureBoxTooltip.InitialDelay = 1000;
            this.pictureBoxTooltip.ReshowDelay = 100;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.previewBakeProgressBar,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 717);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1202, 25);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // previewBakeProgressBar
            // 
            this.previewBakeProgressBar.Name = "previewBakeProgressBar";
            this.previewBakeProgressBar.Size = new System.Drawing.Size(100, 19);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(110, 20);
            this.toolStripStatusLabel1.Text = "Baking preview";
            // 
            // bakePreviewWorker
            // 
            this.bakePreviewWorker.WorkerReportsProgress = true;
            this.bakePreviewWorker.WorkerSupportsCancellation = true;
            this.bakePreviewWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bakePreviewWorker_DoWork);
            this.bakePreviewWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bakePreviewWorker_ProgressChanged);
            this.bakePreviewWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bakePreviewWorker_RunWorkerCompleted);
            // 
            // stipplingWorker
            // 
            this.stipplingWorker.WorkerReportsProgress = true;
            this.stipplingWorker.WorkerSupportsCancellation = true;
            this.stipplingWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.stipplingWorker_DoWork);
            this.stipplingWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.stipplingWorker_ProgressChanged);
            // 
            // serializerWorker
            // 
            this.serializerWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.serializerWorker_DoWork);
            // 
            // prepareStipplingWorker
            // 
            this.prepareStipplingWorker.WorkerReportsProgress = true;
            this.prepareStipplingWorker.WorkerSupportsCancellation = true;
            this.prepareStipplingWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.prepareStipplingWorker_DoWork);
            this.prepareStipplingWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.prepareStipplingWorker_ProgressChanged);
            this.prepareStipplingWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.prepareStipplingWorker_RunWorkerCompleted);
            // 
            // statusReportLabel
            // 
            this.statusReportLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.statusReportLabel.AutoSize = true;
            this.statusReportLabel.Location = new System.Drawing.Point(544, 694);
            this.statusReportLabel.Name = "statusReportLabel";
            this.statusReportLabel.Size = new System.Drawing.Size(20, 17);
            this.statusReportLabel.TabIndex = 24;
            this.statusReportLabel.Text = "...";
            // 
            // cropRegionCheckBox
            // 
            this.cropRegionCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.cropRegionCheckBox.AutoSize = true;
            this.cropRegionCheckBox.Location = new System.Drawing.Point(3, 3);
            this.cropRegionCheckBox.Name = "cropRegionCheckBox";
            this.cropRegionCheckBox.Size = new System.Drawing.Size(97, 27);
            this.cropRegionCheckBox.TabIndex = 25;
            this.cropRegionCheckBox.Text = "Crop Region";
            this.pictureBoxTooltip.SetToolTip(this.cropRegionCheckBox, "Enable this option and drag a rectangle \r\nto constrain samples to a smaller image" +
        " \r\nregion for faster preview.");
            this.cropRegionCheckBox.UseVisualStyleBackColor = true;
            this.cropRegionCheckBox.Click += new System.EventHandler(this.cropRegionCheckBox_Click);
            // 
            // ColorStippling
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1202, 742);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.splitContainer1);
            this.Name = "ColorStippling";
            this.Text = "Color Stippling | Jose Esteve - www.joesfer.com";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.snapshotPanel.ResumeLayout(false);
            this.snapshotPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button goButton;
        private System.Windows.Forms.ComboBox previewBasisComboBox;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ListBox colorBasisListBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ComboBox pointSizeComboBox;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ListView inputColorBasisList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Splitter splitter4;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Splitter splitter3;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Splitter splitter5;
        private System.Windows.Forms.ToolTip pictureBoxTooltip;
        private System.Windows.Forms.Panel snapshotPanel;
        private System.Windows.Forms.Button snapshotButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MaskedTextBox snapshotSizeMaskedTextBox1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar previewBakeProgressBar;
        private System.ComponentModel.BackgroundWorker bakePreviewWorker;
        private System.ComponentModel.BackgroundWorker stipplingWorker;
        private System.ComponentModel.BackgroundWorker serializerWorker;
        private System.ComponentModel.BackgroundWorker prepareStipplingWorker;
        private System.Windows.Forms.Label statusReportLabel;
        private System.Windows.Forms.CheckBox cropRegionCheckBox;
    }
}

