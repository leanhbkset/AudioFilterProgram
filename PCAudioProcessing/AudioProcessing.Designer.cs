using PCAudioProcessing.UserControls;

namespace PCAudioProcessing
{
    partial class AudioProcessing
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.savePWMDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.segmentationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.prevButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.specNoComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cepstrumSizeTextBox = new System.Windows.Forms.TextBox();
            this.hopSizeTextBox = new System.Windows.Forms.TextBox();
            this.fftSizeTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.lblx = new System.Windows.Forms.Label();
            this.btnFilter = new System.Windows.Forms.Button();
            this.txtFreq = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtWidthFreq = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnSaveas = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblNote = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblMaxValue = new System.Windows.Forms.Label();
            this.txtMaxValue = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtTotalTime = new System.Windows.Forms.TextBox();
            this.spectrumPanelAfterFilter = new PCAudioProcessing.UserControls.LinePlot();
            this.signalPanelAfterFilter = new PCAudioProcessing.UserControls.SignalPlot();
            this.signalPanel = new PCAudioProcessing.UserControls.SignalPlot();
            this.spectrumPanel = new PCAudioProcessing.UserControls.LinePlot();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.playToolStripMenuItem,
            this.pauseToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.segmentationToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1344, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.savePWMDataToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.saveAsToolStripMenuItem.Text = "Save as";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // savePWMDataToolStripMenuItem
            // 
            this.savePWMDataToolStripMenuItem.Enabled = false;
            this.savePWMDataToolStripMenuItem.Name = "savePWMDataToolStripMenuItem";
            this.savePWMDataToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.savePWMDataToolStripMenuItem.Text = "Save PWM data";
            this.savePWMDataToolStripMenuItem.Click += new System.EventHandler(this.savePWMDataToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // playToolStripMenuItem
            // 
            this.playToolStripMenuItem.Name = "playToolStripMenuItem";
            this.playToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.playToolStripMenuItem.Text = "Play";
            this.playToolStripMenuItem.Click += new System.EventHandler(this.playToolStripMenuItem_Click);
            // 
            // pauseToolStripMenuItem
            // 
            this.pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
            this.pauseToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.pauseToolStripMenuItem.Text = "Pause";
            this.pauseToolStripMenuItem.Click += new System.EventHandler(this.pauseToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // segmentationToolStripMenuItem
            // 
            this.segmentationToolStripMenuItem.Name = "segmentationToolStripMenuItem";
            this.segmentationToolStripMenuItem.Size = new System.Drawing.Size(93, 20);
            this.segmentationToolStripMenuItem.Text = "Segmentation";
            this.segmentationToolStripMenuItem.Click += new System.EventHandler(this.segmentationToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(248, 387);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Frequency domain";
            // 
            // prevButton
            // 
            this.prevButton.Location = new System.Drawing.Point(595, 77);
            this.prevButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.prevButton.Name = "prevButton";
            this.prevButton.Size = new System.Drawing.Size(47, 20);
            this.prevButton.TabIndex = 5;
            this.prevButton.Text = "Prev";
            this.prevButton.UseVisualStyleBackColor = true;
            this.prevButton.Visible = false;
            this.prevButton.Click += new System.EventHandler(this.prevButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.Location = new System.Drawing.Point(695, 77);
            this.nextButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(42, 20);
            this.nextButton.TabIndex = 6;
            this.nextButton.Text = "Next";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Visible = false;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // specNoComboBox
            // 
            this.specNoComboBox.FormattingEnabled = true;
            this.specNoComboBox.ItemHeight = 13;
            this.specNoComboBox.Location = new System.Drawing.Point(646, 77);
            this.specNoComboBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.specNoComboBox.Name = "specNoComboBox";
            this.specNoComboBox.Size = new System.Drawing.Size(45, 21);
            this.specNoComboBox.TabIndex = 7;
            this.specNoComboBox.Visible = false;
            this.specNoComboBox.TextChanged += new System.EventHandler(this.specNoComboBox_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cepstrumSizeTextBox);
            this.groupBox1.Controls.Add(this.hopSizeTextBox);
            this.groupBox1.Controls.Add(this.fftSizeTextBox);
            this.groupBox1.Location = new System.Drawing.Point(589, 132);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(158, 151);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parameters";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 109);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Cepstrum size";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Hop size";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "FFT size";
            // 
            // cepstrumSizeTextBox
            // 
            this.cepstrumSizeTextBox.Location = new System.Drawing.Point(91, 106);
            this.cepstrumSizeTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cepstrumSizeTextBox.Name = "cepstrumSizeTextBox";
            this.cepstrumSizeTextBox.ReadOnly = true;
            this.cepstrumSizeTextBox.Size = new System.Drawing.Size(43, 20);
            this.cepstrumSizeTextBox.TabIndex = 13;
            this.cepstrumSizeTextBox.Text = "256";
            // 
            // hopSizeTextBox
            // 
            this.hopSizeTextBox.Location = new System.Drawing.Point(91, 73);
            this.hopSizeTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.hopSizeTextBox.Name = "hopSizeTextBox";
            this.hopSizeTextBox.ReadOnly = true;
            this.hopSizeTextBox.Size = new System.Drawing.Size(43, 20);
            this.hopSizeTextBox.TabIndex = 12;
            this.hopSizeTextBox.Text = "100";
            // 
            // fftSizeTextBox
            // 
            this.fftSizeTextBox.ForeColor = System.Drawing.Color.White;
            this.fftSizeTextBox.Location = new System.Drawing.Point(91, 39);
            this.fftSizeTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.fftSizeTextBox.Name = "fftSizeTextBox";
            this.fftSizeTextBox.ReadOnly = true;
            this.fftSizeTextBox.Size = new System.Drawing.Size(43, 20);
            this.fftSizeTextBox.TabIndex = 11;
            this.fftSizeTextBox.Text = "1024";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(248, 55);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 16);
            this.label7.TabIndex = 12;
            this.label7.Text = "Time domain";
            // 
            // txtFilePath
            // 
            this.txtFilePath.Location = new System.Drawing.Point(19, 28);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(568, 20);
            this.txtFilePath.TabIndex = 13;
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(631, 25);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(71, 23);
            this.btnOpen.TabIndex = 14;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // lblx
            // 
            this.lblx.AutoSize = true;
            this.lblx.BackColor = System.Drawing.Color.Transparent;
            this.lblx.ForeColor = System.Drawing.Color.Red;
            this.lblx.Location = new System.Drawing.Point(516, 390);
            this.lblx.Name = "lblx";
            this.lblx.Size = new System.Drawing.Size(57, 13);
            this.lblx.TabIndex = 20;
            this.lblx.Text = "Frequency";
            // 
            // btnFilter
            // 
            this.btnFilter.Location = new System.Drawing.Point(631, 444);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(75, 24);
            this.btnFilter.TabIndex = 23;
            this.btnFilter.Text = "Filter";
            this.btnFilter.UseVisualStyleBackColor = true;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // txtFreq
            // 
            this.txtFreq.Location = new System.Drawing.Point(598, 499);
            this.txtFreq.Name = "txtFreq";
            this.txtFreq.Size = new System.Drawing.Size(72, 20);
            this.txtFreq.TabIndex = 24;
            this.txtFreq.Text = "0";
            this.txtFreq.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(598, 480);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 25;
            this.label2.Text = "Frequency";
            // 
            // txtWidthFreq
            // 
            this.txtWidthFreq.Location = new System.Drawing.Point(681, 499);
            this.txtWidthFreq.Name = "txtWidthFreq";
            this.txtWidthFreq.Size = new System.Drawing.Size(66, 20);
            this.txtWidthFreq.TabIndex = 26;
            this.txtWidthFreq.Text = "150";
            this.txtWidthFreq.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(692, 480);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 27;
            this.label6.Text = "Width";
            // 
            // btnSaveas
            // 
            this.btnSaveas.Location = new System.Drawing.Point(631, 678);
            this.btnSaveas.Name = "btnSaveas";
            this.btnSaveas.Size = new System.Drawing.Size(75, 23);
            this.btnSaveas.TabIndex = 28;
            this.btnSaveas.Text = "Save as";
            this.btnSaveas.UseVisualStyleBackColor = true;
            this.btnSaveas.Click += new System.EventHandler(this.btnSaveas_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(973, 55);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(143, 16);
            this.label8.TabIndex = 31;
            this.label8.Text = "Time domain after filter";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(973, 387);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(162, 15);
            this.label9.TabIndex = 32;
            this.label9.Text = "Frequency domain after filter";
            // 
            // lblNote
            // 
            this.lblNote.AutoSize = true;
            this.lblNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNote.ForeColor = System.Drawing.Color.Red;
            this.lblNote.Location = new System.Drawing.Point(628, 426);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(37, 16);
            this.lblNote.TabIndex = 33;
            this.lblNote.Text = "Note";
            this.lblNote.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(631, 554);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 34;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lblMaxValue
            // 
            this.lblMaxValue.AutoSize = true;
            this.lblMaxValue.Location = new System.Drawing.Point(618, 298);
            this.lblMaxValue.Name = "lblMaxValue";
            this.lblMaxValue.Size = new System.Drawing.Size(105, 13);
            this.lblMaxValue.TabIndex = 35;
            this.lblMaxValue.Text = "Max Amplitude value";
            // 
            // txtMaxValue
            // 
            this.txtMaxValue.BackColor = System.Drawing.Color.White;
            this.txtMaxValue.Location = new System.Drawing.Point(619, 314);
            this.txtMaxValue.Name = "txtMaxValue";
            this.txtMaxValue.ReadOnly = true;
            this.txtMaxValue.Size = new System.Drawing.Size(100, 20);
            this.txtMaxValue.TabIndex = 36;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(640, 337);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(47, 13);
            this.label10.TabIndex = 37;
            this.label10.Text = "Duration";
            // 
            // txtTotalTime
            // 
            this.txtTotalTime.BackColor = System.Drawing.Color.White;
            this.txtTotalTime.Location = new System.Drawing.Point(619, 353);
            this.txtTotalTime.Name = "txtTotalTime";
            this.txtTotalTime.ReadOnly = true;
            this.txtTotalTime.Size = new System.Drawing.Size(100, 20);
            this.txtTotalTime.TabIndex = 38;
            // 
            // spectrumPanelAfterFilter
            // 
            this.spectrumPanelAfterFilter.AutoScroll = true;
            this.spectrumPanelAfterFilter.AutoSize = true;
            this.spectrumPanelAfterFilter.BackColor = System.Drawing.Color.White;
            this.spectrumPanelAfterFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.spectrumPanelAfterFilter.ForeColor = System.Drawing.Color.Blue;
            this.spectrumPanelAfterFilter.Gain = null;
            this.spectrumPanelAfterFilter.Legend = null;
            this.spectrumPanelAfterFilter.Line = null;
            this.spectrumPanelAfterFilter.Location = new System.Drawing.Point(752, 405);
            this.spectrumPanelAfterFilter.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.spectrumPanelAfterFilter.Mark = null;
            this.spectrumPanelAfterFilter.Markline = null;
            this.spectrumPanelAfterFilter.max_freq_value = 0F;
            this.spectrumPanelAfterFilter.Name = "spectrumPanelAfterFilter";
            this.spectrumPanelAfterFilter.PaddingX = 30;
            this.spectrumPanelAfterFilter.PaddingY = 20;
            this.spectrumPanelAfterFilter.Size = new System.Drawing.Size(568, 296);
            this.spectrumPanelAfterFilter.Stride = 1;
            this.spectrumPanelAfterFilter.TabIndex = 30;
            this.spectrumPanelAfterFilter.Thickness = 1;
            // 
            // signalPanelAfterFilter
            // 
            this.signalPanelAfterFilter.AutoScroll = true;
            this.signalPanelAfterFilter.BackColor = System.Drawing.Color.White;
            this.signalPanelAfterFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.signalPanelAfterFilter.ForeColor = System.Drawing.Color.Blue;
            this.signalPanelAfterFilter.Gain = 1F;
            this.signalPanelAfterFilter.Location = new System.Drawing.Point(752, 77);
            this.signalPanelAfterFilter.Margin = new System.Windows.Forms.Padding(2);
            this.signalPanelAfterFilter.max_time_value = 20000F;
            this.signalPanelAfterFilter.Name = "signalPanelAfterFilter";
            this.signalPanelAfterFilter.PaddingX = 24;
            this.signalPanelAfterFilter.PaddingY = 5;
            this.signalPanelAfterFilter.Signal = null;
            this.signalPanelAfterFilter.Size = new System.Drawing.Size(568, 296);
            this.signalPanelAfterFilter.Stride = 64;
            this.signalPanelAfterFilter.TabIndex = 29;
            // 
            // signalPanel
            // 
            this.signalPanel.AutoScroll = true;
            this.signalPanel.BackColor = System.Drawing.Color.White;
            this.signalPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.signalPanel.ForeColor = System.Drawing.Color.Blue;
            this.signalPanel.Gain = 1F;
            this.signalPanel.Location = new System.Drawing.Point(19, 77);
            this.signalPanel.Margin = new System.Windows.Forms.Padding(2);
            this.signalPanel.max_time_value = 20000F;
            this.signalPanel.Name = "signalPanel";
            this.signalPanel.PaddingX = 24;
            this.signalPanel.PaddingY = 5;
            this.signalPanel.Signal = null;
            this.signalPanel.Size = new System.Drawing.Size(568, 296);
            this.signalPanel.Stride = 64;
            this.signalPanel.TabIndex = 15;
            // 
            // spectrumPanel
            // 
            this.spectrumPanel.AutoScroll = true;
            this.spectrumPanel.AutoSize = true;
            this.spectrumPanel.BackColor = System.Drawing.Color.White;
            this.spectrumPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.spectrumPanel.ForeColor = System.Drawing.Color.Blue;
            this.spectrumPanel.Gain = null;
            this.spectrumPanel.Legend = null;
            this.spectrumPanel.Line = null;
            this.spectrumPanel.Location = new System.Drawing.Point(19, 405);
            this.spectrumPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.spectrumPanel.Mark = null;
            this.spectrumPanel.Markline = null;
            this.spectrumPanel.max_freq_value = 0F;
            this.spectrumPanel.Name = "spectrumPanel";
            this.spectrumPanel.PaddingX = 30;
            this.spectrumPanel.PaddingY = 20;
            this.spectrumPanel.Size = new System.Drawing.Size(568, 296);
            this.spectrumPanel.Stride = 1;
            this.spectrumPanel.TabIndex = 1;
            this.spectrumPanel.Thickness = 1;
            this.spectrumPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.spectrumPanel_MouseMove);
            // 
            // AudioProcessing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1344, 730);
            this.Controls.Add(this.txtTotalTime);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtMaxValue);
            this.Controls.Add(this.lblMaxValue);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.lblNote);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.spectrumPanelAfterFilter);
            this.Controls.Add(this.signalPanelAfterFilter);
            this.Controls.Add(this.btnSaveas);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtWidthFreq);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtFreq);
            this.Controls.Add(this.btnFilter);
            this.Controls.Add(this.lblx);
            this.Controls.Add(this.signalPanel);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.txtFilePath);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.specNoComboBox);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.prevButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.spectrumPanel);
            this.Controls.Add(this.menuStrip1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "AudioProcessing";
            this.Text = " ";
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.AudioProcessing_MouseMove);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private LinePlot spectrumPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button prevButton;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.ComboBox specNoComboBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox cepstrumSizeTextBox;
        private System.Windows.Forms.TextBox hopSizeTextBox;
        private System.Windows.Forms.TextBox fftSizeTextBox;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Button btnOpen;
        private SignalPlot signalPanel;
        private System.Windows.Forms.ToolStripMenuItem playToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.Label lblx;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.TextBox txtFreq;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtWidthFreq;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnSaveas;
        private SignalPlot signalPanelAfterFilter;
        private LinePlot spectrumPanelAfterFilter;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblNote;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblMaxValue;
        private System.Windows.Forms.TextBox txtMaxValue;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtTotalTime;
        private System.Windows.Forms.ToolStripMenuItem savePWMDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem segmentationToolStripMenuItem;
    }
}