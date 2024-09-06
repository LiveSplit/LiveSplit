namespace LiveSplit.Racetime.View
{
    partial class ChannelForm
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
            this.chatBox = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.loadMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // chatBox
            // 
            this.chatBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chatBox.Location = new System.Drawing.Point(0, 0);
            this.chatBox.Name = "chatBox";
            this.chatBox.Size = new System.Drawing.Size(1145, 619);
            this.chatBox.TabIndex = 1;
            // 
            // loadMessage
            // 
            this.loadMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.loadMessage.AutoSize = true;
            this.loadMessage.Cursor = System.Windows.Forms.Cursors.AppStarting;
            this.loadMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 50F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadMessage.Location = new System.Drawing.Point(409, 277);
            this.loadMessage.Name = "loadMessage";
            this.loadMessage.Size = new System.Drawing.Size(326, 76);
            this.loadMessage.TabIndex = 2;
            this.loadMessage.Text = "Loading...";
            this.loadMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ChannelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1145, 619);
            this.Controls.Add(this.chatBox);
            this.Controls.Add(this.loadMessage);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "ChannelForm";
            this.ShowIcon = false;
            this.Text = "ChannelWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChannelForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Microsoft.Web.WebView2.WinForms.WebView2 chatBox;
        private System.Windows.Forms.Label loadMessage;
    }
}