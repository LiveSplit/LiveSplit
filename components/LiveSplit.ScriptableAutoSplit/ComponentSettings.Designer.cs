namespace LiveSplit.UI.Components
{
    partial class ComponentSettings
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelScriptPath = new System.Windows.Forms.Label();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.txtScriptPath = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCheckAll = new System.Windows.Forms.Button();
            this.btnUncheckAll = new System.Windows.Forms.Button();
            this.btnResetToDefault = new System.Windows.Forms.Button();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkboxStart = new System.Windows.Forms.CheckBox();
            this.checkboxSplit = new System.Windows.Forms.CheckBox();
            this.checkboxReset = new System.Windows.Forms.CheckBox();
            this.lblGameVersion = new System.Windows.Forms.Label();
            this.labelOptions = new System.Windows.Forms.Label();
            this.labelCustomSettings = new System.Windows.Forms.Label();
            this.treeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmiExpandTree = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiCollapseTree = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiCollapseTreeToSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cmiExpandBranch = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiCollapseBranch = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cmiCheckBranch = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiUncheckBranch = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiResetBranchToDefault = new System.Windows.Forms.ToolStripMenuItem();
            this.treeContextMenu2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmiExpandTree2 = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiCollapseTree2 = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiCollapseTreeToSelection2 = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiResetSettingToDefault = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.resetSettingToDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.treeCustomSettings = new LiveSplit.UI.Components.NewTreeView();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.treeContextMenu.SuspendLayout();
            this.treeContextMenu2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 76F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.labelScriptPath, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSelectFile, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtScriptPath, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelOptions, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelCustomSettings, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.treeCustomSettings, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(7, 7);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(462, 498);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // labelScriptPath
            // 
            this.labelScriptPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelScriptPath.AutoSize = true;
            this.labelScriptPath.Location = new System.Drawing.Point(3, 8);
            this.labelScriptPath.Name = "labelScriptPath";
            this.labelScriptPath.Size = new System.Drawing.Size(70, 13);
            this.labelScriptPath.TabIndex = 3;
            this.labelScriptPath.Text = "Script Path:";
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectFile.Location = new System.Drawing.Point(385, 3);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(74, 23);
            this.btnSelectFile.TabIndex = 1;
            this.btnSelectFile.Text = "Browse...";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // txtScriptPath
            // 
            this.txtScriptPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtScriptPath.Location = new System.Drawing.Point(79, 4);
            this.txtScriptPath.Name = "txtScriptPath";
            this.txtScriptPath.Size = new System.Drawing.Size(300, 20);
            this.txtScriptPath.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Controls.Add(this.btnCheckAll);
            this.flowLayoutPanel1.Controls.Add(this.btnUncheckAll);
            this.flowLayoutPanel1.Controls.Add(this.btnResetToDefault);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(205, 457);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(254, 29);
            this.flowLayoutPanel1.TabIndex = 8;
            // 
            // btnCheckAll
            // 
            this.btnCheckAll.Location = new System.Drawing.Point(3, 3);
            this.btnCheckAll.Name = "btnCheckAll";
            this.btnCheckAll.Size = new System.Drawing.Size(62, 23);
            this.btnCheckAll.TabIndex = 5;
            this.btnCheckAll.Text = "Check All";
            this.btnCheckAll.UseVisualStyleBackColor = true;
            this.btnCheckAll.Click += new System.EventHandler(this.btnCheckAll_Click);
            // 
            // btnUncheckAll
            // 
            this.btnUncheckAll.AutoSize = true;
            this.btnUncheckAll.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnUncheckAll.Location = new System.Drawing.Point(71, 3);
            this.btnUncheckAll.Name = "btnUncheckAll";
            this.btnUncheckAll.Size = new System.Drawing.Size(75, 23);
            this.btnUncheckAll.TabIndex = 6;
            this.btnUncheckAll.Text = "Uncheck All";
            this.btnUncheckAll.UseVisualStyleBackColor = true;
            this.btnUncheckAll.Click += new System.EventHandler(this.btnUncheckAll_Click);
            // 
            // btnResetToDefault
            // 
            this.btnResetToDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetToDefault.Location = new System.Drawing.Point(152, 3);
            this.btnResetToDefault.Name = "btnResetToDefault";
            this.btnResetToDefault.Size = new System.Drawing.Size(99, 23);
            this.btnResetToDefault.TabIndex = 7;
            this.btnResetToDefault.Text = "Reset to Default";
            this.btnResetToDefault.UseVisualStyleBackColor = true;
            this.btnResetToDefault.Click += new System.EventHandler(this.btnResetToDefault_Click);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel2, 2);
            this.flowLayoutPanel2.Controls.Add(this.checkboxStart);
            this.flowLayoutPanel2.Controls.Add(this.checkboxSplit);
            this.flowLayoutPanel2.Controls.Add(this.checkboxReset);
            this.flowLayoutPanel2.Controls.Add(this.lblGameVersion);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(79, 32);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(380, 23);
            this.flowLayoutPanel2.TabIndex = 12;
            // 
            // checkboxStart
            // 
            this.checkboxStart.Enabled = false;
            this.checkboxStart.Location = new System.Drawing.Point(3, 3);
            this.checkboxStart.Name = "checkboxStart";
            this.checkboxStart.Size = new System.Drawing.Size(48, 17);
            this.checkboxStart.TabIndex = 11;
            this.checkboxStart.Text = "Start";
            this.checkboxStart.UseVisualStyleBackColor = true;
            this.checkboxStart.CheckedChanged += new System.EventHandler(this.methodCheckbox_CheckedChanged);
            // 
            // checkboxSplit
            // 
            this.checkboxSplit.Enabled = false;
            this.checkboxSplit.Location = new System.Drawing.Point(57, 3);
            this.checkboxSplit.Name = "checkboxSplit";
            this.checkboxSplit.Size = new System.Drawing.Size(46, 17);
            this.checkboxSplit.TabIndex = 0;
            this.checkboxSplit.Text = "Split";
            this.checkboxSplit.UseVisualStyleBackColor = true;
            this.checkboxSplit.CheckedChanged += new System.EventHandler(this.methodCheckbox_CheckedChanged);
            // 
            // checkboxReset
            // 
            this.checkboxReset.Enabled = false;
            this.checkboxReset.Location = new System.Drawing.Point(109, 3);
            this.checkboxReset.Name = "checkboxReset";
            this.checkboxReset.Size = new System.Drawing.Size(54, 17);
            this.checkboxReset.TabIndex = 0;
            this.checkboxReset.Text = "Reset";
            this.checkboxReset.UseVisualStyleBackColor = true;
            this.checkboxReset.CheckedChanged += new System.EventHandler(this.methodCheckbox_CheckedChanged);
            // 
            // lblGameVersion
            // 
            this.lblGameVersion.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblGameVersion.AutoEllipsis = true;
            this.lblGameVersion.Location = new System.Drawing.Point(169, 5);
            this.lblGameVersion.Name = "lblGameVersion";
            this.lblGameVersion.Size = new System.Drawing.Size(208, 13);
            this.lblGameVersion.TabIndex = 10;
            this.lblGameVersion.Text = "Game Version: 1.0";
            this.lblGameVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelOptions
            // 
            this.labelOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelOptions.AutoSize = true;
            this.labelOptions.Location = new System.Drawing.Point(3, 37);
            this.labelOptions.Name = "labelOptions";
            this.labelOptions.Size = new System.Drawing.Size(70, 13);
            this.labelOptions.TabIndex = 9;
            this.labelOptions.Text = "Options:";
            // 
            // labelCustomSettings
            // 
            this.labelCustomSettings.AutoSize = true;
            this.labelCustomSettings.Location = new System.Drawing.Point(3, 63);
            this.labelCustomSettings.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.labelCustomSettings.Name = "labelCustomSettings";
            this.labelCustomSettings.Size = new System.Drawing.Size(59, 13);
            this.labelCustomSettings.TabIndex = 13;
            this.labelCustomSettings.Text = "Advanced:";
            // 
            // treeContextMenu
            // 
            this.treeContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmiExpandTree,
            this.cmiCollapseTree,
            this.cmiCollapseTreeToSelection,
            this.toolStripSeparator1,
            this.cmiExpandBranch,
            this.cmiCollapseBranch,
            this.toolStripSeparator2,
            this.cmiCheckBranch,
            this.cmiUncheckBranch,
            this.cmiResetBranchToDefault,
            this.toolStripSeparator3,
            this.cmiResetSettingToDefault});
            this.treeContextMenu.Name = "treeContextMenu";
            this.treeContextMenu.Size = new System.Drawing.Size(199, 220);
            // 
            // cmiExpandTree
            // 
            this.cmiExpandTree.Name = "cmiExpandTree";
            this.cmiExpandTree.Size = new System.Drawing.Size(198, 22);
            this.cmiExpandTree.Text = "Expand Tree";
            this.cmiExpandTree.Click += new System.EventHandler(this.cmiExpandTree_Click);
            // 
            // cmiCollapseTree
            // 
            this.cmiCollapseTree.Name = "cmiCollapseTree";
            this.cmiCollapseTree.Size = new System.Drawing.Size(198, 22);
            this.cmiCollapseTree.Text = "Collapse Tree";
            this.cmiCollapseTree.Click += new System.EventHandler(this.cmiCollapseTree_Click);
            // 
            // cmiCollapseTreeToSelection
            // 
            this.cmiCollapseTreeToSelection.Name = "cmiCollapseTreeToSelection";
            this.cmiCollapseTreeToSelection.Size = new System.Drawing.Size(198, 22);
            this.cmiCollapseTreeToSelection.Text = "Collapse Tree to Selection";
            this.cmiCollapseTreeToSelection.Click += new System.EventHandler(this.cmiCollapseTreeToSelection_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(195, 6);
            // 
            // cmiExpandBranch
            // 
            this.cmiExpandBranch.Name = "cmiExpandBranch";
            this.cmiExpandBranch.Size = new System.Drawing.Size(198, 22);
            this.cmiExpandBranch.Text = "Expand Branch";
            this.cmiExpandBranch.Click += new System.EventHandler(this.cmiExpandBranch_Click);
            // 
            // cmiCollapseBranch
            // 
            this.cmiCollapseBranch.Name = "cmiCollapseBranch";
            this.cmiCollapseBranch.Size = new System.Drawing.Size(198, 22);
            this.cmiCollapseBranch.Text = "Collapse Branch";
            this.cmiCollapseBranch.Click += new System.EventHandler(this.cmiCollapseBranch_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(195, 6);
            // 
            // cmiCheckBranch
            // 
            this.cmiCheckBranch.Name = "cmiCheckBranch";
            this.cmiCheckBranch.Size = new System.Drawing.Size(198, 22);
            this.cmiCheckBranch.Text = "Check Branch";
            this.cmiCheckBranch.Click += new System.EventHandler(this.cmiCheckBranch_Click);
            // 
            // cmiUncheckBranch
            // 
            this.cmiUncheckBranch.Name = "cmiUncheckBranch";
            this.cmiUncheckBranch.Size = new System.Drawing.Size(198, 22);
            this.cmiUncheckBranch.Text = "Uncheck Branch";
            this.cmiUncheckBranch.Click += new System.EventHandler(this.cmiUncheckBranch_Click);
            // 
            // cmiResetBranchToDefault
            // 
            this.cmiResetBranchToDefault.Name = "cmiResetBranchToDefault";
            this.cmiResetBranchToDefault.Size = new System.Drawing.Size(198, 22);
            this.cmiResetBranchToDefault.Text = "Reset Branch to Default";
            this.cmiResetBranchToDefault.Click += new System.EventHandler(this.cmiResetBranchToDefault_Click);
            // 
            // treeContextMenu2
            // 
            this.treeContextMenu2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmiExpandTree2,
            this.cmiCollapseTree2,
            this.cmiCollapseTreeToSelection2,
            this.toolStripSeparator4,
            this.resetSettingToDefaultToolStripMenuItem});
            this.treeContextMenu2.Name = "treeContextMenu";
            this.treeContextMenu2.Size = new System.Drawing.Size(199, 98);
            // 
            // cmiExpandTree2
            // 
            this.cmiExpandTree2.Name = "cmiExpandTree2";
            this.cmiExpandTree2.Size = new System.Drawing.Size(198, 22);
            this.cmiExpandTree2.Text = "Expand Tree";
            this.cmiExpandTree2.Click += new System.EventHandler(this.cmiExpandTree_Click);
            // 
            // cmiCollapseTree2
            // 
            this.cmiCollapseTree2.Name = "cmiCollapseTree2";
            this.cmiCollapseTree2.Size = new System.Drawing.Size(198, 22);
            this.cmiCollapseTree2.Text = "Collapse Tree";
            this.cmiCollapseTree2.Click += new System.EventHandler(this.cmiCollapseTree_Click);
            // 
            // cmiCollapseTreeToSelection2
            // 
            this.cmiCollapseTreeToSelection2.Name = "cmiCollapseTreeToSelection2";
            this.cmiCollapseTreeToSelection2.Size = new System.Drawing.Size(198, 22);
            this.cmiCollapseTreeToSelection2.Text = "Collapse Tree to Selection";
            this.cmiCollapseTreeToSelection2.Click += new System.EventHandler(this.cmiCollapseTreeToSelection_Click);
            // 
            // cmiResetSettingToDefault
            // 
            this.cmiResetSettingToDefault.Name = "cmiResetSettingToDefault";
            this.cmiResetSettingToDefault.Size = new System.Drawing.Size(198, 22);
            this.cmiResetSettingToDefault.Text = "Reset Setting to Default";
            this.cmiResetSettingToDefault.Click += new System.EventHandler(this.cmiResetSettingToDefault_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(195, 6);
            // 
            // resetSettingToDefaultToolStripMenuItem
            // 
            this.resetSettingToDefaultToolStripMenuItem.Name = "resetSettingToDefaultToolStripMenuItem";
            this.resetSettingToDefaultToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.resetSettingToDefaultToolStripMenuItem.Text = "Reset Setting to Default";
            this.resetSettingToDefaultToolStripMenuItem.Click += new System.EventHandler(this.cmiResetSettingToDefault_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(195, 6);
            // 
            // treeCustomSettings
            // 
            this.treeCustomSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeCustomSettings.CheckBoxes = true;
            this.tableLayoutPanel1.SetColumnSpan(this.treeCustomSettings, 2);
            this.treeCustomSettings.Location = new System.Drawing.Point(79, 61);
            this.treeCustomSettings.Name = "treeCustomSettings";
            this.treeCustomSettings.ShowNodeToolTips = true;
            this.treeCustomSettings.Size = new System.Drawing.Size(380, 390);
            this.treeCustomSettings.TabIndex = 14;
            this.treeCustomSettings.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.settingsTree_BeforeCheck);
            this.treeCustomSettings.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.settingsTree_AfterCheck);
            this.treeCustomSettings.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.settingsTree_NodeMouseClick);
            // 
            // ComponentSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ComponentSettings";
            this.Padding = new System.Windows.Forms.Padding(7);
            this.Size = new System.Drawing.Size(476, 512);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.treeContextMenu.ResumeLayout(false);
            this.treeContextMenu2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelScriptPath;
        private System.Windows.Forms.Button btnSelectFile;
        public System.Windows.Forms.TextBox txtScriptPath;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnCheckAll;
        private System.Windows.Forms.Button btnUncheckAll;
        private System.Windows.Forms.Button btnResetToDefault;
        private System.Windows.Forms.Label labelOptions;
        private System.Windows.Forms.Label lblGameVersion;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.CheckBox checkboxStart;
        private System.Windows.Forms.CheckBox checkboxReset;
        private System.Windows.Forms.CheckBox checkboxSplit;
        private System.Windows.Forms.Label labelCustomSettings;
        private NewTreeView treeCustomSettings;
        private System.Windows.Forms.ContextMenuStrip treeContextMenu;
        private System.Windows.Forms.ToolStripMenuItem cmiCheckBranch;
        private System.Windows.Forms.ToolStripMenuItem cmiUncheckBranch;
        private System.Windows.Forms.ToolStripMenuItem cmiResetBranchToDefault;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem cmiExpandBranch;
        private System.Windows.Forms.ToolStripMenuItem cmiCollapseBranch;
        private System.Windows.Forms.ToolStripMenuItem cmiCollapseTreeToSelection;
        private System.Windows.Forms.ContextMenuStrip treeContextMenu2;
        private System.Windows.Forms.ToolStripMenuItem cmiCollapseTreeToSelection2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem cmiExpandTree;
        private System.Windows.Forms.ToolStripMenuItem cmiCollapseTree;
        private System.Windows.Forms.ToolStripMenuItem cmiExpandTree2;
        private System.Windows.Forms.ToolStripMenuItem cmiCollapseTree2;
        private System.Windows.Forms.ToolStripMenuItem cmiResetSettingToDefault;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem resetSettingToDefaultToolStripMenuItem;
    }
}
