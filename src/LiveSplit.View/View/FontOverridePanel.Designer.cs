namespace LiveSplit.View
{
    partial class FontOverridePanel
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._groupBox = new System.Windows.Forms.GroupBox();
            this._tableLayout = new System.Windows.Forms.TableLayoutPanel();
            this._chkOverrideTimerFont = new System.Windows.Forms.CheckBox();
            this._btnTimerFont = new System.Windows.Forms.Button();
            this._lblTimerFont = new System.Windows.Forms.Label();
            this._chkOverrideTimesFont = new System.Windows.Forms.CheckBox();
            this._btnTimesFont = new System.Windows.Forms.Button();
            this._lblTimesFont = new System.Windows.Forms.Label();
            this._chkOverrideTextFont = new System.Windows.Forms.CheckBox();
            this._btnTextFont = new System.Windows.Forms.Button();
            this._lblTextFont = new System.Windows.Forms.Label();
            this._groupBox.SuspendLayout();
            this._tableLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // _groupBox
            // 
            this._groupBox.Controls.Add(this._tableLayout);
            this._groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._groupBox.Location = new System.Drawing.Point(0, 0);
            this._groupBox.Name = "_groupBox";
            this._groupBox.Padding = new System.Windows.Forms.Padding(10, 6, 10, 6);
            this._groupBox.Size = new System.Drawing.Size(476, 120);
            this._groupBox.TabIndex = 0;
            this._groupBox.TabStop = false;
            this._groupBox.Text = "Font Overrides";
            // 
            // _tableLayout
            // 
            this._tableLayout.ColumnCount = 3;
            this._tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this._tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this._tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayout.Controls.Add(this._chkOverrideTimerFont, 0, 0);
            this._tableLayout.Controls.Add(this._btnTimerFont, 1, 0);
            this._tableLayout.Controls.Add(this._lblTimerFont, 2, 0);
            this._tableLayout.Controls.Add(this._chkOverrideTimesFont, 0, 1);
            this._tableLayout.Controls.Add(this._btnTimesFont, 1, 1);
            this._tableLayout.Controls.Add(this._lblTimesFont, 2, 1);
            this._tableLayout.Controls.Add(this._chkOverrideTextFont, 0, 2);
            this._tableLayout.Controls.Add(this._btnTextFont, 1, 2);
            this._tableLayout.Controls.Add(this._lblTextFont, 2, 2);
            this._tableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tableLayout.Location = new System.Drawing.Point(6, 16);
            this._tableLayout.Name = "_tableLayout";
            this._tableLayout.RowCount = 3;
            this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this._tableLayout.Size = new System.Drawing.Size(464, 101);
            this._tableLayout.TabIndex = 0;
            // 
            // _chkOverrideTimerFont
            // 
            this._chkOverrideTimerFont.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._chkOverrideTimerFont.AutoSize = true;
            this._chkOverrideTimerFont.Location = new System.Drawing.Point(3, 8);
            this._chkOverrideTimerFont.Name = "_chkOverrideTimerFont";
            this._chkOverrideTimerFont.Size = new System.Drawing.Size(128, 17);
            this._chkOverrideTimerFont.TabIndex = 0;
            this._chkOverrideTimerFont.Text = "Override Timer Font";
            this._chkOverrideTimerFont.UseVisualStyleBackColor = true;
            this._chkOverrideTimerFont.CheckedChanged += new System.EventHandler(this.chkOverrideTimerFont_CheckedChanged);
            // 
            // _btnTimerFont
            // 
            this._btnTimerFont.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._btnTimerFont.Enabled = false;
            this._btnTimerFont.Location = new System.Drawing.Point(163, 5);
            this._btnTimerFont.Name = "_btnTimerFont";
            this._btnTimerFont.Size = new System.Drawing.Size(74, 23);
            this._btnTimerFont.TabIndex = 1;
            this._btnTimerFont.Text = "Choose...";
            this._btnTimerFont.UseVisualStyleBackColor = true;
            this._btnTimerFont.Click += new System.EventHandler(this.btnTimerFont_Click);
            // 
            // _lblTimerFont
            // 
            this._lblTimerFont.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lblTimerFont.AutoSize = true;
            this._lblTimerFont.Location = new System.Drawing.Point(243, 10);
            this._lblTimerFont.Name = "_lblTimerFont";
            this._lblTimerFont.Size = new System.Drawing.Size(63, 13);
            this._lblTimerFont.TabIndex = 2;
            this._lblTimerFont.Text = "Using global";
            // 
            // _chkOverrideTimesFont
            // 
            this._chkOverrideTimesFont.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._chkOverrideTimesFont.AutoSize = true;
            this._chkOverrideTimesFont.Location = new System.Drawing.Point(3, 41);
            this._chkOverrideTimesFont.Name = "_chkOverrideTimesFont";
            this._chkOverrideTimesFont.Size = new System.Drawing.Size(130, 17);
            this._chkOverrideTimesFont.TabIndex = 3;
            this._chkOverrideTimesFont.Text = "Override Times Font";
            this._chkOverrideTimesFont.UseVisualStyleBackColor = true;
            this._chkOverrideTimesFont.CheckedChanged += new System.EventHandler(this.chkOverrideTimesFont_CheckedChanged);
            // 
            // _btnTimesFont
            // 
            this._btnTimesFont.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._btnTimesFont.Enabled = false;
            this._btnTimesFont.Location = new System.Drawing.Point(163, 38);
            this._btnTimesFont.Name = "_btnTimesFont";
            this._btnTimesFont.Size = new System.Drawing.Size(74, 23);
            this._btnTimesFont.TabIndex = 4;
            this._btnTimesFont.Text = "Choose...";
            this._btnTimesFont.UseVisualStyleBackColor = true;
            this._btnTimesFont.Click += new System.EventHandler(this.btnTimesFont_Click);
            // 
            // _lblTimesFont
            // 
            this._lblTimesFont.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lblTimesFont.AutoSize = true;
            this._lblTimesFont.Location = new System.Drawing.Point(243, 43);
            this._lblTimesFont.Name = "_lblTimesFont";
            this._lblTimesFont.Size = new System.Drawing.Size(63, 13);
            this._lblTimesFont.TabIndex = 5;
            this._lblTimesFont.Text = "Using global";
            // 
            // _chkOverrideTextFont
            // 
            this._chkOverrideTextFont.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._chkOverrideTextFont.AutoSize = true;
            this._chkOverrideTextFont.Location = new System.Drawing.Point(3, 75);
            this._chkOverrideTextFont.Name = "_chkOverrideTextFont";
            this._chkOverrideTextFont.Size = new System.Drawing.Size(122, 17);
            this._chkOverrideTextFont.TabIndex = 6;
            this._chkOverrideTextFont.Text = "Override Text Font";
            this._chkOverrideTextFont.UseVisualStyleBackColor = true;
            this._chkOverrideTextFont.CheckedChanged += new System.EventHandler(this.chkOverrideTextFont_CheckedChanged);
            // 
            // _btnTextFont
            // 
            this._btnTextFont.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._btnTextFont.Enabled = false;
            this._btnTextFont.Location = new System.Drawing.Point(163, 72);
            this._btnTextFont.Name = "_btnTextFont";
            this._btnTextFont.Size = new System.Drawing.Size(74, 23);
            this._btnTextFont.TabIndex = 7;
            this._btnTextFont.Text = "Choose...";
            this._btnTextFont.UseVisualStyleBackColor = true;
            this._btnTextFont.Click += new System.EventHandler(this.btnTextFont_Click);
            // 
            // _lblTextFont
            // 
            this._lblTextFont.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lblTextFont.AutoSize = true;
            this._lblTextFont.Location = new System.Drawing.Point(243, 77);
            this._lblTextFont.Name = "_lblTextFont";
            this._lblTextFont.Size = new System.Drawing.Size(63, 13);
            this._lblTextFont.TabIndex = 8;
            this._lblTextFont.Text = "Using global";
            // 
            // FontOverridePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._groupBox);
            this.Name = "FontOverridePanel";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(476, 134);
            this._groupBox.ResumeLayout(false);
            this._tableLayout.ResumeLayout(false);
            this._tableLayout.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox _groupBox;
        private System.Windows.Forms.TableLayoutPanel _tableLayout;
        private System.Windows.Forms.CheckBox _chkOverrideTimerFont;
        private System.Windows.Forms.Button _btnTimerFont;
        private System.Windows.Forms.Label _lblTimerFont;
        private System.Windows.Forms.CheckBox _chkOverrideTimesFont;
        private System.Windows.Forms.Button _btnTimesFont;
        private System.Windows.Forms.Label _lblTimesFont;
        private System.Windows.Forms.CheckBox _chkOverrideTextFont;
        private System.Windows.Forms.Button _btnTextFont;
        private System.Windows.Forms.Label _lblTextFont;
    }
}
