namespace LiveSplit.ManualGameTime.UI.Components
{
    partial class ShitSplitter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShitSplitter));
            this.txtGameTime = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtGameTime
            // 
            this.txtGameTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGameTime.Location = new System.Drawing.Point(12, 12);
            this.txtGameTime.Name = "txtGameTime";
            this.txtGameTime.Size = new System.Drawing.Size(260, 20);
            this.txtGameTime.TabIndex = 0;
            this.txtGameTime.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtGameTime_KeyPress);
            // 
            // ShitSplitter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 44);
            this.Controls.Add(this.txtGameTime);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ShitSplitter";
            this.Text = "Enter Game Time";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtGameTime;
    }
}