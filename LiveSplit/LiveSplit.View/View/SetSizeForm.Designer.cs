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
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 83F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.nmWidth, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.nmHeight, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnOK, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.chkKeepAspectRatio, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(7, 7);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(216, 141);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Width:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Height:";
            // 
            // nmWidth
            // 
            this.nmWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.nmWidth, 2);
            this.nmWidth.Location = new System.Drawing.Point(55, 4);
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
            this.nmWidth.Size = new System.Drawing.Size(158, 20);
            this.nmWidth.TabIndex = 2;
            this.nmWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nmWidth.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // nmHeight
            // 
            this.nmHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.nmHeight, 2);
            this.nmHeight.Location = new System.Drawing.Point(55, 33);
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
            this.nmHeight.Size = new System.Drawing.Size(158, 20);
            this.nmHeight.TabIndex = 3;
            this.nmHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nmHeight.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(138, 115);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.btnOK, 2);
            this.btnOK.Location = new System.Drawing.Point(57, 115);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // chkKeepAspectRatio
            // 
            this.chkKeepAspectRatio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkKeepAspectRatio.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.chkKeepAspectRatio, 3);
            this.chkKeepAspectRatio.Location = new System.Drawing.Point(7, 64);
            this.chkKeepAspectRatio.Margin = new System.Windows.Forms.Padding(7, 3, 3, 3);
            this.chkKeepAspectRatio.Name = "chkKeepAspectRatio";
            this.chkKeepAspectRatio.Size = new System.Drawing.Size(206, 17);
            this.chkKeepAspectRatio.TabIndex = 6;
            this.chkKeepAspectRatio.Text = "Keep Aspect Ratio";
            this.chkKeepAspectRatio.UseVisualStyleBackColor = true;
            // 
            // SetSizeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(230, 155);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SetSizeForm";
            this.Padding = new System.Windows.Forms.Padding(7);
            this.Text = "Set Layout Size";
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