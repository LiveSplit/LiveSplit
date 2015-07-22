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
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.btnCancel, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.btnOK, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.comparisonsListBox, 0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // comparisonsListBox
            // 
            resources.ApplyResources(this.comparisonsListBox, "comparisonsListBox");
            this.comparisonsListBox.CheckOnClick = true;
            this.tableLayoutPanel3.SetColumnSpan(this.comparisonsListBox, 2);
            this.comparisonsListBox.FormattingEnabled = true;
            this.comparisonsListBox.Name = "comparisonsListBox";
            this.comparisonsListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.comparisonsListBox_ItemCheck);
            // 
            // ChooseComparisonsDialog
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel3);
            this.Name = "ChooseComparisonsDialog";
            this.Load += new System.EventHandler(this.ChooseComparisonsDialog_Load);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.CheckedListBox comparisonsListBox;

    }
}