using LiveSplit.UI.Components;

namespace LiveSplit.AutoSplittingRuntime
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
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelScriptPath = new System.Windows.Forms.Label();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.txtScriptPath = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.settingsTable = new System.Windows.Forms.TableLayoutPanel();
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
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.cmiResetSettingToDefault = new System.Windows.Forms.ToolStripMenuItem();
            this.treeContextMenu2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmiExpandTree2 = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiCollapseTree2 = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiCollapseTreeToSelection2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.resetSettingToDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.settingsBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel.SuspendLayout();
            this.treeContextMenu.SuspendLayout();
            this.treeContextMenu2.SuspendLayout();
            this.settingsBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 76F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.labelScriptPath, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.btnSelectFile, 2, 0);
            this.tableLayoutPanel.Controls.Add(this.txtScriptPath, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.flowLayoutPanel2, 1, 1);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(7, 7);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 3;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(462, 498);
            this.tableLayoutPanel.TabIndex = 0;
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
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.flowLayoutPanel2, 2);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(79, 32);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(380, 0);
            this.flowLayoutPanel2.TabIndex = 12;
            // 
            // settingsTable
            // 
            this.settingsTable.AutoScroll = true;
            this.settingsTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.settingsTable.ColumnCount = 1;
            this.settingsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.settingsTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsTable.Location = new System.Drawing.Point(3, 16);
            this.settingsTable.Name = "settingsTable";
            this.settingsTable.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.settingsTable.RowCount = 1;
            this.settingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.settingsTable.Size = new System.Drawing.Size(450, 431);
            this.settingsTable.TabIndex = 13;
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
            this.treeContextMenu.Size = new System.Drawing.Size(209, 220);
            // 
            // cmiExpandTree
            // 
            this.cmiExpandTree.Name = "cmiExpandTree";
            this.cmiExpandTree.Size = new System.Drawing.Size(208, 22);
            this.cmiExpandTree.Text = "Expand Tree";
            // 
            // cmiCollapseTree
            // 
            this.cmiCollapseTree.Name = "cmiCollapseTree";
            this.cmiCollapseTree.Size = new System.Drawing.Size(208, 22);
            this.cmiCollapseTree.Text = "Collapse Tree";
            // 
            // cmiCollapseTreeToSelection
            // 
            this.cmiCollapseTreeToSelection.Name = "cmiCollapseTreeToSelection";
            this.cmiCollapseTreeToSelection.Size = new System.Drawing.Size(208, 22);
            this.cmiCollapseTreeToSelection.Text = "Collapse Tree to Selection";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(205, 6);
            // 
            // cmiExpandBranch
            // 
            this.cmiExpandBranch.Name = "cmiExpandBranch";
            this.cmiExpandBranch.Size = new System.Drawing.Size(208, 22);
            this.cmiExpandBranch.Text = "Expand Branch";
            // 
            // cmiCollapseBranch
            // 
            this.cmiCollapseBranch.Name = "cmiCollapseBranch";
            this.cmiCollapseBranch.Size = new System.Drawing.Size(208, 22);
            this.cmiCollapseBranch.Text = "Collapse Branch";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(205, 6);
            // 
            // cmiCheckBranch
            // 
            this.cmiCheckBranch.Name = "cmiCheckBranch";
            this.cmiCheckBranch.Size = new System.Drawing.Size(208, 22);
            // 
            // cmiUncheckBranch
            // 
            this.cmiUncheckBranch.Name = "cmiUncheckBranch";
            this.cmiUncheckBranch.Size = new System.Drawing.Size(208, 22);
            // 
            // cmiResetBranchToDefault
            // 
            this.cmiResetBranchToDefault.Name = "cmiResetBranchToDefault";
            this.cmiResetBranchToDefault.Size = new System.Drawing.Size(208, 22);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(205, 6);
            // 
            // cmiResetSettingToDefault
            // 
            this.cmiResetSettingToDefault.Name = "cmiResetSettingToDefault";
            this.cmiResetSettingToDefault.Size = new System.Drawing.Size(208, 22);
            this.cmiResetSettingToDefault.Text = "Reset Setting to Default";
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
            this.treeContextMenu2.Size = new System.Drawing.Size(209, 98);
            // 
            // cmiExpandTree2
            // 
            this.cmiExpandTree2.Name = "cmiExpandTree2";
            this.cmiExpandTree2.Size = new System.Drawing.Size(208, 22);
            this.cmiExpandTree2.Text = "Expand Tree";
            // 
            // cmiCollapseTree2
            // 
            this.cmiCollapseTree2.Name = "cmiCollapseTree2";
            this.cmiCollapseTree2.Size = new System.Drawing.Size(208, 22);
            this.cmiCollapseTree2.Text = "Collapse Tree";
            // 
            // cmiCollapseTreeToSelection2
            // 
            this.cmiCollapseTreeToSelection2.Name = "cmiCollapseTreeToSelection2";
            this.cmiCollapseTreeToSelection2.Size = new System.Drawing.Size(208, 22);
            this.cmiCollapseTreeToSelection2.Text = "Collapse Tree to Selection";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(205, 6);
            // 
            // resetSettingToDefaultToolStripMenuItem
            // 
            this.resetSettingToDefaultToolStripMenuItem.Name = "resetSettingToDefaultToolStripMenuItem";
            this.resetSettingToDefaultToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.resetSettingToDefaultToolStripMenuItem.Text = "Reset Setting to Default";
            // 
            // settingsBox
            // 
            this.settingsBox.Controls.Add(this.settingsTable);
            this.settingsBox.Location = new System.Drawing.Point(10, 52);
            this.settingsBox.Name = "settingsBox";
            this.settingsBox.Size = new System.Drawing.Size(456, 450);
            this.settingsBox.TabIndex = 13;
            this.settingsBox.TabStop = false;
            this.settingsBox.Text = "Auto Splitter Settings";
            // 
            // ComponentSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.settingsBox);
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "ComponentSettings";
            this.Padding = new System.Windows.Forms.Padding(7);
            this.Size = new System.Drawing.Size(476, 512);
            this.Load += new System.EventHandler(this.ComponentSettings_Load);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.treeContextMenu.ResumeLayout(false);
            this.treeContextMenu2.ResumeLayout(false);
            this.settingsBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label labelScriptPath;
        private System.Windows.Forms.Button btnSelectFile;
        public System.Windows.Forms.TextBox txtScriptPath;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
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
        private System.Windows.Forms.TableLayoutPanel settingsTable;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.GroupBox settingsBox;
    }
}
