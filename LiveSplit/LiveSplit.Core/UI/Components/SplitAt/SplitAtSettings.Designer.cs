namespace LiveSplit.UI.Components.SplitAt
{
    partial class SplitAtSettings
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
            this.components = new System.ComponentModel.Container();
            this.trkSpacing = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.trkWeight = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.btnResetSpacing = new System.Windows.Forms.Button();
            this.btnResetWeight = new System.Windows.Forms.Button();
            this.txtSpacing = new System.Windows.Forms.TextBox();
            this.txtWeight = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.trkSpacing)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkWeight)).BeginInit();
            this.SuspendLayout();
            // 
            // trkSpacing
            // 
            this.trkSpacing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trkSpacing.Location = new System.Drawing.Point(63, 3);
            this.trkSpacing.Maximum = 800;
            this.trkSpacing.Name = "trkSpacing";
            this.trkSpacing.Size = new System.Drawing.Size(284, 24);
            this.trkSpacing.TabIndex = 0;
            this.trkSpacing.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Spacing:";
            this.toolTip1.SetToolTip(this.label1, "Blank space between the previous and following row/column,\r\nrelative to the overa" +
        "ll window size.");
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 290F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 54F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 58F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.trkSpacing, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.trkWeight, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnResetSpacing, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnResetWeight, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtSpacing, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtWeight, 2, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(7, 7);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(462, 186);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // trkWeight
            // 
            this.trkWeight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trkWeight.Location = new System.Drawing.Point(63, 33);
            this.trkWeight.Maximum = 4000;
            this.trkWeight.Name = "trkWeight";
            this.trkWeight.Size = new System.Drawing.Size(284, 24);
            this.trkWeight.TabIndex = 2;
            this.trkWeight.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trkWeight.Value = 1000;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Size:";
            this.toolTip1.SetToolTip(this.label2, "Size of the following row/column, relative to other rows/columns.\r\n\r\nThe first ro" +
        "w/column is always 100% (to make it smaller, increase the size of the others).");
            // 
            // btnResetSpacing
            // 
            this.btnResetSpacing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetSpacing.Location = new System.Drawing.Point(407, 3);
            this.btnResetSpacing.Name = "btnResetSpacing";
            this.btnResetSpacing.Size = new System.Drawing.Size(52, 23);
            this.btnResetSpacing.TabIndex = 6;
            this.btnResetSpacing.Text = "Reset";
            this.btnResetSpacing.UseVisualStyleBackColor = true;
            // 
            // btnResetWeight
            // 
            this.btnResetWeight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetWeight.Location = new System.Drawing.Point(407, 33);
            this.btnResetWeight.Name = "btnResetWeight";
            this.btnResetWeight.Size = new System.Drawing.Size(52, 23);
            this.btnResetWeight.TabIndex = 7;
            this.btnResetWeight.Text = "Reset";
            this.btnResetWeight.UseVisualStyleBackColor = true;
            // 
            // txtSpacing
            // 
            this.txtSpacing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSpacing.Location = new System.Drawing.Point(353, 5);
            this.txtSpacing.Name = "txtSpacing";
            this.txtSpacing.Size = new System.Drawing.Size(48, 20);
            this.txtSpacing.TabIndex = 8;
            // 
            // txtWeight
            // 
            this.txtWeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWeight.Location = new System.Drawing.Point(353, 35);
            this.txtWeight.Name = "txtWeight";
            this.txtWeight.Size = new System.Drawing.Size(48, 20);
            this.txtWeight.TabIndex = 9;
            // 
            // SplitAtSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SplitAtSettings";
            this.Padding = new System.Windows.Forms.Padding(7);
            this.Size = new System.Drawing.Size(476, 200);
            ((System.ComponentModel.ISupportInitialize)(this.trkSpacing)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkWeight)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TrackBar trkSpacing;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TrackBar trkWeight;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnResetSpacing;
        private System.Windows.Forms.Button btnResetWeight;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox txtSpacing;
        private System.Windows.Forms.TextBox txtWeight;
    }
}
