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
            this.btnShowOnSpeedrunCom = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.txtSearch, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSearch, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnDownload, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.splitsTreeView, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.chkDownloadEmpty, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.chkIncludeTimes, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnShowOnSpeedrunCom, 2, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // txtSearch
            // 
            resources.ApplyResources(this.txtSearch, "txtSearch");
            this.tableLayoutPanel1.SetColumnSpan(this.txtSearch, 3);
            this.txtSearch.Name = "txtSearch";
            // 
            // btnSearch
            // 
            resources.ApplyResources(this.btnSearch, "btnSearch");
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnDownload
            // 
            resources.ApplyResources(this.btnDownload, "btnDownload");
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // splitsTreeView
            // 
            resources.ApplyResources(this.splitsTreeView, "splitsTreeView");
            this.tableLayoutPanel1.SetColumnSpan(this.splitsTreeView, 4);
            this.splitsTreeView.Name = "splitsTreeView";
            this.splitsTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.splitsTreeView_AfterSelect);
            // 
            // chkDownloadEmpty
            // 
            resources.ApplyResources(this.chkDownloadEmpty, "chkDownloadEmpty");
            this.chkDownloadEmpty.Name = "chkDownloadEmpty";
            this.chkDownloadEmpty.UseVisualStyleBackColor = true;
            this.chkDownloadEmpty.CheckedChanged += new System.EventHandler(this.chkDownloadEmpty_CheckedChanged);
            // 
            // chkIncludeTimes
            // 
            resources.ApplyResources(this.chkIncludeTimes, "chkIncludeTimes");
            this.chkIncludeTimes.Name = "chkIncludeTimes";
            this.chkIncludeTimes.UseVisualStyleBackColor = true;
            // 
            // btnShowOnSpeedrunCom
            // 
            resources.ApplyResources(this.btnShowOnSpeedrunCom, "btnShowOnSpeedrunCom");
            this.btnShowOnSpeedrunCom.Name = "btnShowOnSpeedrunCom";
            this.btnShowOnSpeedrunCom.UseVisualStyleBackColor = true;
            this.btnShowOnSpeedrunCom.Click += new System.EventHandler(this.btnShowOnSpeedrunCom_Click);
            // 
            // BrowseSpeedrunComDialog
            // 
            this.AcceptButton = this.btnSearch;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "BrowseSpeedrunComDialog";
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
        private System.Windows.Forms.Button btnShowOnSpeedrunCom;
    }
}