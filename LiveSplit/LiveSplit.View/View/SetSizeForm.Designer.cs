namespace LiveSplit.View
{
    partial class SetSizeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetSizeForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nmWidth = new System.Windows.Forms.NumericUpDown();
            this.nmHeight = new System.Windows.Forms.NumericUpDown();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.chkKeepAspectRatio = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.nmWidth, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.nmHeight, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnOK, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.chkKeepAspectRatio, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // nmWidth
            // 
            resources.ApplyResources(this.nmWidth, "nmWidth");
            this.tableLayoutPanel1.SetColumnSpan(this.nmWidth, 2);
            this.nmWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nmWidth.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nmWidth.Name = "nmWidth";
            this.nmWidth.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // nmHeight
            // 
            resources.ApplyResources(this.nmHeight, "nmHeight");
            this.tableLayoutPanel1.SetColumnSpan(this.nmHeight, 2);
            this.nmHeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nmHeight.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nmHeight.Name = "nmHeight";
            this.nmHeight.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.tableLayoutPanel1.SetColumnSpan(this.btnOK, 2);
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // chkKeepAspectRatio
            // 
            resources.ApplyResources(this.chkKeepAspectRatio, "chkKeepAspectRatio");
            this.tableLayoutPanel1.SetColumnSpan(this.chkKeepAspectRatio, 3);
            this.chkKeepAspectRatio.Name = "chkKeepAspectRatio";
            this.chkKeepAspectRatio.UseVisualStyleBackColor = true;
            // 
            // SetSizeForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SetSizeForm";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmHeight)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nmWidth;
        private System.Windows.Forms.NumericUpDown nmHeight;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkKeepAspectRatio;
    }
}