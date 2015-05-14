namespace LiveSplit.View
{
    partial class BrowseSpeedrunComDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BrowseSpeedrunComDialog));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnDownload = new System.Windows.Forms.Button();
            this.splitsTreeView = new System.Windows.Forms.TreeView();
            this.chkDownloadEmpty = new System.Windows.Forms.CheckBox();
            this.chkIncludeTimes = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.txtSearch, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSearch, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnDownload, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.splitsTreeView, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.chkDownloadEmpty, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.chkIncludeTimes, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(7);
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(570, 405);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.txtSearch, 3);
            this.txtSearch.Location = new System.Drawing.Point(10, 11);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(463, 20);
            this.txtSearch.TabIndex = 0;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(479, 10);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(81, 23);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnDownload
            // 
            this.btnDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDownload.Enabled = false;
            this.btnDownload.Location = new System.Drawing.Point(479, 373);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(81, 22);
            this.btnDownload.TabIndex = 2;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // splitsTreeView
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.splitsTreeView, 4);
            this.splitsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitsTreeView.Location = new System.Drawing.Point(10, 39);
            this.splitsTreeView.Name = "splitsTreeView";
            this.splitsTreeView.Size = new System.Drawing.Size(550, 328);
            this.splitsTreeView.TabIndex = 3;
            this.splitsTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.splitsTreeView_AfterSelect);
            // 
            // chkDownloadEmpty
            // 
            this.chkDownloadEmpty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkDownloadEmpty.AutoSize = true;
            this.chkDownloadEmpty.Location = new System.Drawing.Point(14, 375);
            this.chkDownloadEmpty.Margin = new System.Windows.Forms.Padding(7, 3, 3, 3);
            this.chkDownloadEmpty.Name = "chkDownloadEmpty";
            this.chkDownloadEmpty.Size = new System.Drawing.Size(106, 17);
            this.chkDownloadEmpty.TabIndex = 4;
            this.chkDownloadEmpty.Text = "Download Empty";
            this.chkDownloadEmpty.UseVisualStyleBackColor = true;
            this.chkDownloadEmpty.CheckedChanged += new System.EventHandler(this.chkDownloadEmpty_CheckedChanged);
            // 
            // chkIncludeTimes
            // 
            this.chkIncludeTimes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkIncludeTimes.AutoSize = true;
            this.chkIncludeTimes.Enabled = false;
            this.chkIncludeTimes.Location = new System.Drawing.Point(130, 375);
            this.chkIncludeTimes.Margin = new System.Windows.Forms.Padding(7, 3, 3, 3);
            this.chkIncludeTimes.Name = "chkIncludeTimes";
            this.chkIncludeTimes.Size = new System.Drawing.Size(164, 17);
            this.chkIncludeTimes.TabIndex = 5;
            this.chkIncludeTimes.Text = "Include Times as Comparison";
            this.chkIncludeTimes.UseVisualStyleBackColor = true;
            // 
            // BrowseSpeedrunComDialog
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(570, 405);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(400, 200);
            this.Name = "BrowseSpeedrunComDialog";
            this.Text = "Browse Speedrun.com";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.TreeView splitsTreeView;
        private System.Windows.Forms.CheckBox chkDownloadEmpty;
        private System.Windows.Forms.CheckBox chkIncludeTimes;
    }
}