namespace CustomFontDialog
{
    partial class FontDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lstSize = new System.Windows.Forms.ListBox();
            this.txtSize = new System.Windows.Forms.TextBox();
            this.chbBold = new System.Windows.Forms.CheckBox();
            this.chbItalic = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblSampleText = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lstFont = new CustomFontDialog.FontList();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Font:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(187, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Size:";
            // 
            // lstSize
            // 
            this.lstSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstSize.FormattingEnabled = true;
            this.lstSize.Items.AddRange(new object[] {
            "8",
            "9",
            "10",
            "11",
            "12",
            "14",
            "16",
            "18",
            "20",
            "22",
            "24",
            "26",
            "28",
            "36",
            "48",
            "72"});
            this.lstSize.Location = new System.Drawing.Point(190, 50);
            this.lstSize.Name = "lstSize";
            this.lstSize.Size = new System.Drawing.Size(100, 171);
            this.lstSize.TabIndex = 4;
            this.lstSize.SelectedIndexChanged += new System.EventHandler(this.lstSize_SelectedIndexChanged);
            // 
            // txtSize
            // 
            this.txtSize.Location = new System.Drawing.Point(190, 30);
            this.txtSize.Name = "txtSize";
            this.txtSize.Size = new System.Drawing.Size(100, 20);
            this.txtSize.TabIndex = 5;
            this.txtSize.TextChanged += new System.EventHandler(this.txtSize_TextChanged);
            this.txtSize.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSize_KeyDown);
            // 
            // chbBold
            // 
            this.chbBold.AutoSize = true;
            this.chbBold.Location = new System.Drawing.Point(21, 27);
            this.chbBold.Name = "chbBold";
            this.chbBold.Size = new System.Drawing.Size(47, 17);
            this.chbBold.TabIndex = 6;
            this.chbBold.Text = "Bold";
            this.chbBold.UseVisualStyleBackColor = true;
            this.chbBold.CheckedChanged += new System.EventHandler(this.chb_CheckedChanged);
            // 
            // chbItalic
            // 
            this.chbItalic.AutoSize = true;
            this.chbItalic.Location = new System.Drawing.Point(21, 50);
            this.chbItalic.Name = "chbItalic";
            this.chbItalic.Size = new System.Drawing.Size(48, 17);
            this.chbItalic.TabIndex = 7;
            this.chbItalic.Text = "Italic";
            this.chbItalic.UseVisualStyleBackColor = true;
            this.chbItalic.CheckedChanged += new System.EventHandler(this.chb_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chbBold);
            this.groupBox1.Controls.Add(this.chbItalic);
            this.groupBox1.Location = new System.Drawing.Point(312, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(113, 83);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Font Style";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblSampleText);
            this.groupBox2.Location = new System.Drawing.Point(190, 238);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(235, 79);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sample Text";
            // 
            // lblSampleText
            // 
            this.lblSampleText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSampleText.Location = new System.Drawing.Point(6, 16);
            this.lblSampleText.Name = "lblSampleText";
            this.lblSampleText.Size = new System.Drawing.Size(223, 53);
            this.lblSampleText.TabIndex = 0;
            this.lblSampleText.Text = "AaBbCcXxYyZz";
            this.lblSampleText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(350, 327);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(269, 327);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 11;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // lstFont
            // 
            this.lstFont.Location = new System.Drawing.Point(16, 30);
            this.lstFont.Name = "lstFont";
            this.lstFont.SelectedFontFamily = null;
            this.lstFont.Size = new System.Drawing.Size(150, 289);
            this.lstFont.TabIndex = 1;
            // 
            // FontDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 362);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtSize);
            this.Controls.Add(this.lstSize);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lstFont);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FontDialog";
            this.ShowIcon = false;
            this.Text = "Font";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FontDialog_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private FontList lstFont;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lstSize;
        private System.Windows.Forms.TextBox txtSize;
        private System.Windows.Forms.CheckBox chbBold;
        private System.Windows.Forms.CheckBox chbItalic;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblSampleText;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}