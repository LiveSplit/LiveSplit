namespace LiveSplit.View
{
    partial class ChooseComparisonsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseComparisonsDialog));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.comparisonsListBox = new System.Windows.Forms.CheckedListBox();
            this.groupBoxHcpSettings = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownHcpNBestRuns = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownHcpHistorySize = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBoxHcpSettings.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHcpNBestRuns)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHcpHistorySize)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOK.Location = new System.Drawing.Point(121, 270);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 16;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(202, 270);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.tableLayoutPanel3.Controls.Add(this.btnCancel, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.btnOK, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.comparisonsListBox, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.groupBoxHcpSettings, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(7, 7);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(280, 296);
            this.tableLayoutPanel3.TabIndex = 41;
            // 
            // comparisonsListBox
            // 
            this.comparisonsListBox.CheckOnClick = true;
            this.tableLayoutPanel3.SetColumnSpan(this.comparisonsListBox, 2);
            this.comparisonsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comparisonsListBox.FormattingEnabled = true;
            this.comparisonsListBox.Location = new System.Drawing.Point(3, 3);
            this.comparisonsListBox.Name = "comparisonsListBox";
            this.comparisonsListBox.Size = new System.Drawing.Size(274, 187);
            this.comparisonsListBox.TabIndex = 18;
            this.comparisonsListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.comparisonsListBox_ItemCheck);
            // 
            // groupBoxHcpSettings
            // 
            this.groupBoxHcpSettings.AutoSize = true;
            this.groupBoxHcpSettings.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.SetColumnSpan(this.groupBoxHcpSettings, 2);
            this.groupBoxHcpSettings.Controls.Add(this.tableLayoutPanel1);
            this.groupBoxHcpSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxHcpSettings.Location = new System.Drawing.Point(2, 195);
            this.groupBoxHcpSettings.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxHcpSettings.MinimumSize = new System.Drawing.Size(0, 70);
            this.groupBoxHcpSettings.Name = "groupBoxHcpSettings";
            this.groupBoxHcpSettings.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBoxHcpSettings.Size = new System.Drawing.Size(276, 70);
            this.groupBoxHcpSettings.TabIndex = 19;
            this.groupBoxHcpSettings.TabStop = false;
            this.groupBoxHcpSettings.Text = "Golf HCP Settings";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 78.48411F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.51589F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.numericUpDownHcpNBestRuns, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.numericUpDownHcpHistorySize, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 15);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(272, 53);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "History Size: ";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 33);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "N Best Runs to Use: ";
            // 
            // numericUpDownHcpNBestRuns
            // 
            this.numericUpDownHcpNBestRuns.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownHcpNBestRuns.Location = new System.Drawing.Point(215, 29);
            this.numericUpDownHcpNBestRuns.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.numericUpDownHcpNBestRuns.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDownHcpNBestRuns.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownHcpNBestRuns.Name = "numericUpDownHcpNBestRuns";
            this.numericUpDownHcpNBestRuns.Size = new System.Drawing.Size(55, 20);
            this.numericUpDownHcpNBestRuns.TabIndex = 3;
            this.numericUpDownHcpNBestRuns.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericUpDownHcpNBestRuns.ValueChanged += new System.EventHandler(this.numericUpDownHcpNBestRuns_ValueChanged);
            // 
            // numericUpDownHcpHistorySize
            // 
            this.numericUpDownHcpHistorySize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownHcpHistorySize.AutoSize = true;
            this.numericUpDownHcpHistorySize.Location = new System.Drawing.Point(215, 3);
            this.numericUpDownHcpHistorySize.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.numericUpDownHcpHistorySize.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDownHcpHistorySize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownHcpHistorySize.Name = "numericUpDownHcpHistorySize";
            this.numericUpDownHcpHistorySize.Size = new System.Drawing.Size(55, 20);
            this.numericUpDownHcpHistorySize.TabIndex = 2;
            this.numericUpDownHcpHistorySize.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDownHcpHistorySize.ValueChanged += new System.EventHandler(this.numericUpDownHcpHistorySize_ValueChanged);
            // 
            // ChooseComparisonsDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 310);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(520, 10000);
            this.MinimumSize = new System.Drawing.Size(310, 230);
            this.Name = "ChooseComparisonsDialog";
            this.Padding = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.Text = "Choose Comparisons";
            this.Load += new System.EventHandler(this.ChooseComparisonsDialog_Load);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.groupBoxHcpSettings.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHcpNBestRuns)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHcpHistorySize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.CheckedListBox comparisonsListBox;
        private System.Windows.Forms.GroupBox groupBoxHcpSettings;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownHcpNBestRuns;
        private System.Windows.Forms.NumericUpDown numericUpDownHcpHistorySize;
    }
}
