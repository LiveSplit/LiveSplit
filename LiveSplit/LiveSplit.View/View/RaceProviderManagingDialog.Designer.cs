namespace LiveSplit.View
{
    partial class RaceProviderManagingDialog
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
            this.providerListBox = new System.Windows.Forms.CheckedListBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.settingsUIPanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.websiteTextLabel = new System.Windows.Forms.Label();
            this.rulesTextLabel = new System.Windows.Forms.Label();
            this.websiteLink = new System.Windows.Forms.LinkLabel();
            this.rulesLink = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.providerListBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.settingsUIPanel, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(436, 224);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // providerListBox
            // 
            this.providerListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.providerListBox.FormattingEnabled = true;
            this.providerListBox.Location = new System.Drawing.Point(3, 3);
            this.providerListBox.Name = "providerListBox";
            this.tableLayoutPanel1.SetRowSpan(this.providerListBox, 3);
            this.providerListBox.Size = new System.Drawing.Size(144, 218);
            this.providerListBox.TabIndex = 0;
            this.providerListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.providerListBox_ItemCheck);
            this.providerListBox.SelectedIndexChanged += new System.EventHandler(this.providerListBox_SelectedIndexChanged);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.btnOK);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(153, 195);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(280, 26);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(202, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(121, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // settingsUIPanel
            // 
            this.settingsUIPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsUIPanel.Location = new System.Drawing.Point(153, 43);
            this.settingsUIPanel.Name = "settingsUIPanel";
            this.settingsUIPanel.Size = new System.Drawing.Size(280, 146);
            this.settingsUIPanel.TabIndex = 2;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.websiteTextLabel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.rulesTextLabel, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.websiteLink, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.rulesLink, 1, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(153, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(280, 34);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // websiteTextLabel
            // 
            this.websiteTextLabel.AutoSize = true;
            this.websiteTextLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.websiteTextLabel.Location = new System.Drawing.Point(3, 0);
            this.websiteTextLabel.Name = "websiteTextLabel";
            this.websiteTextLabel.Size = new System.Drawing.Size(49, 17);
            this.websiteTextLabel.TabIndex = 0;
            this.websiteTextLabel.Text = "Website:";
            this.websiteTextLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.websiteTextLabel.Visible = false;
            // 
            // rulesTextLabel
            // 
            this.rulesTextLabel.AutoSize = true;
            this.rulesTextLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rulesTextLabel.Location = new System.Drawing.Point(3, 17);
            this.rulesTextLabel.Name = "rulesTextLabel";
            this.rulesTextLabel.Size = new System.Drawing.Size(49, 17);
            this.rulesTextLabel.TabIndex = 1;
            this.rulesTextLabel.Text = "Rules:";
            this.rulesTextLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rulesTextLabel.Visible = false;
            // 
            // websiteLink
            // 
            this.websiteLink.AutoSize = true;
            this.websiteLink.Dock = System.Windows.Forms.DockStyle.Left;
            this.websiteLink.Location = new System.Drawing.Point(58, 0);
            this.websiteLink.Name = "websiteLink";
            this.websiteLink.Size = new System.Drawing.Size(0, 17);
            this.websiteLink.TabIndex = 2;
            this.websiteLink.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // rulesLink
            // 
            this.rulesLink.AutoSize = true;
            this.rulesLink.Dock = System.Windows.Forms.DockStyle.Left;
            this.rulesLink.Location = new System.Drawing.Point(58, 17);
            this.rulesLink.Name = "rulesLink";
            this.rulesLink.Size = new System.Drawing.Size(0, 17);
            this.rulesLink.TabIndex = 3;
            this.rulesLink.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // RaceProviderManagingDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(436, 224);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(450, 230);
            this.Name = "RaceProviderManagingDialog";
            this.ShowIcon = false;
            this.Text = "Manage Racing Services";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckedListBox providerListBox;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel settingsUIPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label websiteTextLabel;
        private System.Windows.Forms.Label rulesTextLabel;
        private System.Windows.Forms.LinkLabel websiteLink;
        private System.Windows.Forms.LinkLabel rulesLink;
    }
}