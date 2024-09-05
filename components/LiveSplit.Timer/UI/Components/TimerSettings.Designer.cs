namespace LiveSplit.UI.Components
{
    partial class TimerSettings
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cmbAccuracy = new System.Windows.Forms.ComboBox();
            this.cmbTimingMethod = new System.Windows.Forms.ComboBox();
            this.trkSize = new System.Windows.Forms.TrackBar();
            this.lblSize = new System.Windows.Forms.Label();
            this.chkGradient = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnTimerColor = new System.Windows.Forms.Button();
            this.chkOverrideTimerColors = new System.Windows.Forms.CheckBox();
            this.btnColor1 = new System.Windows.Forms.Button();
            this.btnColor2 = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.chkCenterTimer = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbDigitsFormat = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.trkDecimalsSize = new System.Windows.Forms.TrackBar();
            this.cmbGradientType = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkSize)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkDecimalsSize)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 157F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.cmbAccuracy, 4, 6);
            this.tableLayoutPanel1.Controls.Add(this.cmbTimingMethod, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.trkSize, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblSize, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.chkGradient, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnColor1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnColor2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label11, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.chkCenterTimer, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.cmbDigitsFormat, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.trkDecimalsSize, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.cmbGradientType, 3, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(7, 7);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 83F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(462, 287);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // cmbAccuracy
            // 
            this.cmbAccuracy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAccuracy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAccuracy.FormattingEnabled = true;
            this.cmbAccuracy.Items.AddRange(new object[] {
            "",
            ".2",
            ".23",
            ".234"});
            this.cmbAccuracy.Location = new System.Drawing.Point(313, 232);
            this.cmbAccuracy.Name = "cmbAccuracy";
            this.cmbAccuracy.Size = new System.Drawing.Size(146, 21);
            this.cmbAccuracy.TabIndex = 45;
            this.cmbAccuracy.SelectedIndexChanged += new System.EventHandler(this.cmbAccuracy_SelectedIndexChanged);
            // 
            // cmbTimingMethod
            // 
            this.cmbTimingMethod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.cmbTimingMethod, 4);
            this.cmbTimingMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTimingMethod.FormattingEnabled = true;
            this.cmbTimingMethod.Items.AddRange(new object[] {
            "Current Timing Method",
            "Real Time",
            "Game Time"});
            this.cmbTimingMethod.Location = new System.Drawing.Point(160, 33);
            this.cmbTimingMethod.Name = "cmbTimingMethod";
            this.cmbTimingMethod.Size = new System.Drawing.Size(299, 21);
            this.cmbTimingMethod.TabIndex = 3;
            this.cmbTimingMethod.SelectedIndexChanged += new System.EventHandler(this.cmbTimingMethod_SelectedIndexChanged);
            // 
            // trkSize
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.trkSize, 4);
            this.trkSize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trkSize.Location = new System.Drawing.Point(160, 61);
            this.trkSize.Name = "trkSize";
            this.trkSize.Size = new System.Drawing.Size(299, 23);
            this.trkSize.TabIndex = 4;
            this.trkSize.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // lblSize
            // 
            this.lblSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSize.AutoSize = true;
            this.lblSize.Location = new System.Drawing.Point(3, 66);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(151, 13);
            this.lblSize.TabIndex = 1;
            this.lblSize.Text = "Height:";
            // 
            // chkGradient
            // 
            this.chkGradient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkGradient.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.chkGradient, 2);
            this.chkGradient.Location = new System.Drawing.Point(7, 176);
            this.chkGradient.Margin = new System.Windows.Forms.Padding(7, 3, 3, 3);
            this.chkGradient.Name = "chkGradient";
            this.chkGradient.Size = new System.Drawing.Size(176, 17);
            this.chkGradient.TabIndex = 6;
            this.chkGradient.Text = "Show Gradient";
            this.chkGradient.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox2, 5);
            this.groupBox2.Controls.Add(this.tableLayoutPanel3);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 90);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(456, 77);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Timer Color";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.55556F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.44444F));
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.btnTimerColor, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.chkOverrideTimerColors, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(450, 58);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(145, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Color:";
            // 
            // btnTimerColor
            // 
            this.btnTimerColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTimerColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnTimerColor.Location = new System.Drawing.Point(154, 32);
            this.btnTimerColor.Name = "btnTimerColor";
            this.btnTimerColor.Size = new System.Drawing.Size(23, 23);
            this.btnTimerColor.TabIndex = 1;
            this.btnTimerColor.UseVisualStyleBackColor = false;
            this.btnTimerColor.Click += new System.EventHandler(this.ColorButtonClick);
            // 
            // chkOverrideTimerColors
            // 
            this.chkOverrideTimerColors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkOverrideTimerColors.AutoSize = true;
            this.tableLayoutPanel3.SetColumnSpan(this.chkOverrideTimerColors, 2);
            this.chkOverrideTimerColors.Location = new System.Drawing.Point(7, 6);
            this.chkOverrideTimerColors.Margin = new System.Windows.Forms.Padding(7, 3, 3, 3);
            this.chkOverrideTimerColors.Name = "chkOverrideTimerColors";
            this.chkOverrideTimerColors.Size = new System.Drawing.Size(440, 17);
            this.chkOverrideTimerColors.TabIndex = 0;
            this.chkOverrideTimerColors.Text = "Override Layout Settings";
            this.chkOverrideTimerColors.UseVisualStyleBackColor = true;
            this.chkOverrideTimerColors.CheckedChanged += new System.EventHandler(this.chkOverrideTimerColors_CheckedChanged);
            // 
            // btnColor1
            // 
            this.btnColor1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnColor1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnColor1.Location = new System.Drawing.Point(160, 3);
            this.btnColor1.Name = "btnColor1";
            this.btnColor1.Size = new System.Drawing.Size(23, 23);
            this.btnColor1.TabIndex = 0;
            this.btnColor1.UseVisualStyleBackColor = false;
            this.btnColor1.Click += new System.EventHandler(this.ColorButtonClick);
            // 
            // btnColor2
            // 
            this.btnColor2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnColor2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnColor2.Location = new System.Drawing.Point(189, 3);
            this.btnColor2.Name = "btnColor2";
            this.btnColor2.Size = new System.Drawing.Size(23, 23);
            this.btnColor2.TabIndex = 1;
            this.btnColor2.UseVisualStyleBackColor = false;
            this.btnColor2.Click += new System.EventHandler(this.ColorButtonClick);
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 8);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(151, 13);
            this.label11.TabIndex = 39;
            this.label11.Text = "Background Color:";
            // 
            // chkCenterTimer
            // 
            this.chkCenterTimer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkCenterTimer.AutoSize = true;
            this.chkCenterTimer.Location = new System.Drawing.Point(7, 205);
            this.chkCenterTimer.Margin = new System.Windows.Forms.Padding(7, 3, 3, 3);
            this.chkCenterTimer.Name = "chkCenterTimer";
            this.chkCenterTimer.Size = new System.Drawing.Size(147, 17);
            this.chkCenterTimer.TabIndex = 7;
            this.chkCenterTimer.Text = "Align to Center";
            this.chkCenterTimer.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(151, 13);
            this.label2.TabIndex = 41;
            this.label2.Text = "Timing Method:";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 236);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(151, 13);
            this.label3.TabIndex = 43;
            this.label3.Text = "Timer Format:";
            // 
            // cmbDigitsFormat
            // 
            this.cmbDigitsFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.cmbDigitsFormat, 3);
            this.cmbDigitsFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDigitsFormat.FormattingEnabled = true;
            this.cmbDigitsFormat.Items.AddRange(new object[] {
            "1",
            "00:01",
            "0:00:01",
            "00:00:01"});
            this.cmbDigitsFormat.Location = new System.Drawing.Point(160, 232);
            this.cmbDigitsFormat.Name = "cmbDigitsFormat";
            this.cmbDigitsFormat.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cmbDigitsFormat.Size = new System.Drawing.Size(147, 21);
            this.cmbDigitsFormat.TabIndex = 8;
            this.cmbDigitsFormat.SelectedIndexChanged += new System.EventHandler(this.cmbTimerFormat_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 265);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(151, 13);
            this.label4.TabIndex = 44;
            this.label4.Text = "Decimals Font Size:";
            // 
            // trkDecimalsSize
            // 
            this.trkDecimalsSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.trkDecimalsSize, 4);
            this.trkDecimalsSize.Location = new System.Drawing.Point(160, 260);
            this.trkDecimalsSize.Maximum = 50;
            this.trkDecimalsSize.Minimum = 10;
            this.trkDecimalsSize.Name = "trkDecimalsSize";
            this.trkDecimalsSize.Size = new System.Drawing.Size(299, 24);
            this.trkDecimalsSize.TabIndex = 9;
            this.trkDecimalsSize.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trkDecimalsSize.Value = 10;
            // 
            // cmbGradientType
            // 
            this.cmbGradientType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.cmbGradientType, 2);
            this.cmbGradientType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGradientType.FormattingEnabled = true;
            this.cmbGradientType.Items.AddRange(new object[] {
            "Plain",
            "Vertical",
            "Horizontal",
            "Plain With Delta Color",
            "Vertical With Delta Color",
            "Horizontal With Delta Color"});
            this.cmbGradientType.Location = new System.Drawing.Point(218, 4);
            this.cmbGradientType.Name = "cmbGradientType";
            this.cmbGradientType.Size = new System.Drawing.Size(241, 21);
            this.cmbGradientType.TabIndex = 2;
            this.cmbGradientType.SelectedIndexChanged += new System.EventHandler(this.cmbGradientType_SelectedIndexChanged);
            // 
            // TimerSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "TimerSettings";
            this.Padding = new System.Windows.Forms.Padding(7);
            this.Size = new System.Drawing.Size(476, 301);
            this.Load += new System.EventHandler(this.TimerSettings_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkSize)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkDecimalsSize)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TrackBar trkSize;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button btnTimerColor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkOverrideTimerColors;
        private System.Windows.Forms.CheckBox chkGradient;
        private System.Windows.Forms.ComboBox cmbGradientType;
        private System.Windows.Forms.Button btnColor1;
        private System.Windows.Forms.Button btnColor2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox chkCenterTimer;
        private System.Windows.Forms.ComboBox cmbTimingMethod;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbDigitsFormat;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TrackBar trkDecimalsSize;
        private System.Windows.Forms.ComboBox cmbAccuracy;
    }
}
