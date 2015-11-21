namespace CustomFontDialog
{
    partial class FontList
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
            this.lstFont = new System.Windows.Forms.ListBox();
            this.txtFont = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lstFont
            // 
            this.lstFont.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstFont.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstFont.ItemHeight = 20;
            this.lstFont.Location = new System.Drawing.Point(0, 21);
            this.lstFont.Name = "lstFont";
            this.lstFont.Size = new System.Drawing.Size(220, 282);
            this.lstFont.TabIndex = 0;
            this.lstFont.SelectedIndexChanged += new System.EventHandler(this.lstFont_SelectedIndexChanged);
            this.lstFont.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstFont_KeyDown);
            // 
            // txtFont
            // 
            this.txtFont.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFont.Location = new System.Drawing.Point(0, 0);
            this.txtFont.Name = "txtFont";
            this.txtFont.Size = new System.Drawing.Size(220, 20);
            this.txtFont.TabIndex = 1;
            this.txtFont.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txtFont_MouseClick);
            this.txtFont.TextChanged += new System.EventHandler(this.txtFont_TextChanged);
            // 
            // FontList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtFont);
            this.Controls.Add(this.lstFont);
            this.Name = "FontList";
            this.Size = new System.Drawing.Size(220, 307);
            this.Load += new System.EventHandler(this.FontList_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstFont;
        private System.Windows.Forms.TextBox txtFont;
    }
}
