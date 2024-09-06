namespace LiveSplit.UI.Components
{
    partial class ColumnSettings
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.cmbColumnType = new System.Windows.Forms.ComboBox();
            this.cmbComparison = new System.Windows.Forms.ComboBox();
            this.cmbTimingMethod = new System.Windows.Forms.ComboBox();
            this.btnRemoveColumn = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.groupColumn = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupColumn.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 114F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtName, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.cmbColumnType, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.cmbComparison, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.cmbTimingMethod, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnRemoveColumn, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnMoveDown, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnMoveUp, 1, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(421, 146);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 42;
            this.label1.Text = "Name:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 41;
            this.label2.Text = "Timing Method:";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 44;
            this.label3.Text = "Column Type:";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 13);
            this.label4.TabIndex = 45;
            this.label4.Text = "Comparison:";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.txtName, 3);
            this.txtName.Location = new System.Drawing.Point(93, 4);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(325, 20);
            this.txtName.TabIndex = 43;
            this.txtName.TextChanged += new System.EventHandler(txtName_TextChanged);
            // 
            // cmbColumnType
            // 
            this.cmbColumnType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.cmbColumnType, 3);
            this.cmbColumnType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbColumnType.FormattingEnabled = true;
            this.cmbColumnType.Items.AddRange(new object[] {
            "Delta",
            "Split Time",
            "Delta or Split Time",
            "Segment Delta",
            "Segment Time",
            "Segment Delta or Segment Time"});
            this.cmbColumnType.Location = new System.Drawing.Point(93, 33);
            this.cmbColumnType.Name = "cmbColumnType";
            this.cmbColumnType.Size = new System.Drawing.Size(325, 21);
            this.cmbColumnType.TabIndex = 46;
            this.cmbColumnType.SelectedIndexChanged += new System.EventHandler(cmbColumnType_SelectedIndexChanged);
            // 
            // cmbComparison
            // 
            this.cmbComparison.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.cmbComparison, 3);
            this.cmbComparison.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbComparison.FormattingEnabled = true;
            this.cmbComparison.Location = new System.Drawing.Point(93, 62);
            this.cmbComparison.Name = "cmbComparison";
            this.cmbComparison.Size = new System.Drawing.Size(325, 21);
            this.cmbComparison.TabIndex = 47;
            this.cmbComparison.SelectedIndexChanged += new System.EventHandler(cmbComparison_SelectedIndexChanged);
            // 
            // cmbTimingMethod
            // 
            this.cmbTimingMethod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.cmbTimingMethod, 3);
            this.cmbTimingMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTimingMethod.FormattingEnabled = true;
            this.cmbTimingMethod.Items.AddRange(new object[] {
            "Current Timing Method",
            "Real Time",
            "Game Time"});
            this.cmbTimingMethod.Location = new System.Drawing.Point(93, 91);
            this.cmbTimingMethod.Name = "cmbTimingMethod";
            this.cmbTimingMethod.Size = new System.Drawing.Size(325, 21);
            this.cmbTimingMethod.TabIndex = 48;
            this.cmbTimingMethod.SelectedIndexChanged += new System.EventHandler(cmbTimingMethod_SelectedIndexChanged);
            // 
            // btnRemoveColumn
            // 
            this.btnRemoveColumn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnRemoveColumn.Location = new System.Drawing.Point(324, 119);
            this.btnRemoveColumn.Name = "btnRemoveColumn";
            this.btnRemoveColumn.Size = new System.Drawing.Size(94, 23);
            this.btnRemoveColumn.TabIndex = 52;
            this.btnRemoveColumn.Text = "Remove Column";
            this.btnRemoveColumn.UseVisualStyleBackColor = true;
            this.btnRemoveColumn.Click += new System.EventHandler(this.btnRemoveColumn_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnMoveDown.Location = new System.Drawing.Point(223, 119);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(75, 23);
            this.btnMoveDown.TabIndex = 51;
            this.btnMoveDown.Text = "Move Down";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnMoveUp.Location = new System.Drawing.Point(142, 119);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(75, 23);
            this.btnMoveUp.TabIndex = 50;
            this.btnMoveUp.Text = "Move Up";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // groupColumn
            // 
            this.groupColumn.Controls.Add(this.tableLayoutPanel1);
            this.groupColumn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupColumn.Location = new System.Drawing.Point(0, 0);
            this.groupColumn.Name = "groupColumn";
            this.groupColumn.Size = new System.Drawing.Size(427, 165);
            this.groupColumn.TabIndex = 1;
            this.groupColumn.TabStop = false;
            this.groupColumn.Text = "Column Name";
            // 
            // ColumnSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupColumn);
            this.Name = "ColumnSettings";
            this.Size = new System.Drawing.Size(427, 165);
            this.Load += new System.EventHandler(ColumnSettings_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupColumn.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox cmbTimingMethod;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupColumn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbColumnType;
        private System.Windows.Forms.ComboBox cmbComparison;
        private System.Windows.Forms.Button btnRemoveColumn;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Button btnMoveUp;
    }
}