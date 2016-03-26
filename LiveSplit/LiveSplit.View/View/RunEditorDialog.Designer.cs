namespace LiveSplit.View
{
    partial class RunEditorDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RunEditorDialog));
            this.runGrid = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbxGameName = new LiveSplit.View.CustomAutoCompleteComboBox(this);
            this.cbxRunCategory = new System.Windows.Forms.ComboBox();
            this.tbxTimeOffset = new System.Windows.Forms.TextBox();
            this.picGameIcon = new System.Windows.Forms.PictureBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.RealTime = new System.Windows.Forms.TabPage();
            this.GameTime = new System.Windows.Forms.TabPage();
            this.Metadata = new System.Windows.Forms.TabPage();
            this.metadataControl = new LiveSplit.View.MetadataControl();
            this.tbxAttempts = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblDescription = new System.Windows.Forms.Label();
            this.btnInsert = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnAddComparison = new System.Windows.Forms.Button();
            this.btnImportComparison = new System.Windows.Forms.Button();
            this.btnOther = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnWebsite = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnActivate = new System.Windows.Forms.Button();
            this.RemoveIconMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.setIconToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadBoxartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadIconToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFromURLMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeIconToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ImportComparisonMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.fromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromURLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromSpeedruncomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OtherMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearHistoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearTimesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanSumOfBestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iRunBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.iSegmentBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.runGrid)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picGameIcon)).BeginInit();
            this.tabControl.SuspendLayout();
            this.Metadata.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.RemoveIconMenu.SuspendLayout();
            this.ImportComparisonMenu.SuspendLayout();
            this.OtherMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iRunBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iSegmentBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // runGrid
            // 
            resources.ApplyResources(this.runGrid, "runGrid");
            this.runGrid.AllowUserToAddRows = false;
            this.runGrid.AllowUserToResizeColumns = false;
            this.runGrid.AllowUserToResizeRows = false;
            this.runGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.runGrid.BackgroundColor = System.Drawing.SystemColors.Control;
            this.runGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.runGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tableLayoutPanel1.SetColumnSpan(this.runGrid, 9);
            this.runGrid.GridColor = System.Drawing.Color.Gainsboro;
            this.runGrid.Name = "runGrid";
            this.runGrid.RowHeadersVisible = false;
            this.runGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.tableLayoutPanel1.SetRowSpan(this.runGrid, 8);
            this.runGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
<<<<<<< HEAD
=======
            this.runGrid.Size = new System.Drawing.Size(530, 295);
            this.runGrid.TabIndex = 0;
>>>>>>> refs/remotes/LiveSplit/master
            this.runGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.runGrid_KeyDown);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.cbxGameName, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.cbxRunCategory, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.tbxTimeOffset, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.picGameIcon, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.runGrid, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.tabControl, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.tbxAttempts, 6, 3);
            this.tableLayoutPanel1.Controls.Add(this.label4, 5, 3);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 6, 14);
            this.tableLayoutPanel1.Controls.Add(this.lblDescription, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnInsert, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.btnAdd, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.btnRemove, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.btnMoveUp, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.btnMoveDown, 0, 10);
            this.tableLayoutPanel1.Controls.Add(this.btnAddComparison, 0, 11);
            this.tableLayoutPanel1.Controls.Add(this.btnImportComparison, 0, 12);
            this.tableLayoutPanel1.Controls.Add(this.btnOther, 0, 13);
<<<<<<< HEAD
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
=======
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 5, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 15;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(684, 511);
            this.tableLayoutPanel1.TabIndex = 1;
>>>>>>> refs/remotes/LiveSplit/master
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.tableLayoutPanel1.SetColumnSpan(this.label2, 2);
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.tableLayoutPanel1.SetColumnSpan(this.label3, 2);
            this.label3.Name = "label3";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Name = "label1";
            // 
            // cbxGameName
            // 
            resources.ApplyResources(this.cbxGameName, "cbxGameName");
            this.tableLayoutPanel1.SetColumnSpan(this.cbxGameName, 5);
<<<<<<< HEAD
=======
            this.cbxGameName.GetAllItemsForText = null;
            this.cbxGameName.Location = new System.Drawing.Point(246, 12);
            this.cbxGameName.Margin = new System.Windows.Forms.Padding(5, 0, 10, 0);
>>>>>>> refs/remotes/LiveSplit/master
            this.cbxGameName.Name = "cbxGameName";
            this.cbxGameName.TextChanged += new System.EventHandler(this.cbxGameName_TextChanged);
            // 
            // cbxRunCategory
            // 
            resources.ApplyResources(this.cbxRunCategory, "cbxRunCategory");
            this.tableLayoutPanel1.SetColumnSpan(this.cbxRunCategory, 5);
            this.cbxRunCategory.Name = "cbxRunCategory";
            // 
            // tbxTimeOffset
            // 
            resources.ApplyResources(this.tbxTimeOffset, "tbxTimeOffset");
            this.tableLayoutPanel1.SetColumnSpan(this.tbxTimeOffset, 2);
            this.tbxTimeOffset.Name = "tbxTimeOffset";
            // 
            // picGameIcon
            // 
            resources.ApplyResources(this.picGameIcon, "picGameIcon");
            this.picGameIcon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.picGameIcon.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picGameIcon.Name = "picGameIcon";
            this.tableLayoutPanel1.SetRowSpan(this.picGameIcon, 4);
            this.picGameIcon.TabStop = false;
            this.picGameIcon.DoubleClick += new System.EventHandler(this.picGameIcon_DoubleClick);
            this.picGameIcon.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picGameIcon_MouseUp);
            // 
            // tabControl
            // 
            resources.ApplyResources(this.tabControl, "tabControl");
            this.tableLayoutPanel1.SetColumnSpan(this.tabControl, 9);
            this.tabControl.Controls.Add(this.RealTime);
            this.tabControl.Controls.Add(this.GameTime);
<<<<<<< HEAD
=======
            this.tabControl.Controls.Add(this.Metadata);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(144, 148);
            this.tabControl.Margin = new System.Windows.Forms.Padding(4, 3, 10, 0);
>>>>>>> refs/remotes/LiveSplit/master
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Selected += new System.Windows.Forms.TabControlEventHandler(this.TabSelected);
            // 
            // RealTime
            // 
            resources.ApplyResources(this.RealTime, "RealTime");
            this.RealTime.Name = "RealTime";
            this.RealTime.UseVisualStyleBackColor = true;
            // 
            // GameTime
            // 
            resources.ApplyResources(this.GameTime, "GameTime");
            this.GameTime.Name = "GameTime";
            this.GameTime.UseVisualStyleBackColor = true;
            // 
            // Metadata
            // 
            this.Metadata.Controls.Add(this.metadataControl);
            this.Metadata.Location = new System.Drawing.Point(4, 22);
            this.Metadata.Name = "Metadata";
            this.Metadata.Size = new System.Drawing.Size(522, 0);
            this.Metadata.TabIndex = 2;
            this.Metadata.Text = "Additional Info";
            this.Metadata.UseVisualStyleBackColor = true;
            // 
            // metadataControl
            // 
            this.metadataControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metadataControl.Location = new System.Drawing.Point(0, 0);
            this.metadataControl.Metadata = null;
            this.metadataControl.Name = "metadataControl";
            this.metadataControl.Padding = new System.Windows.Forms.Padding(7);
            this.metadataControl.Size = new System.Drawing.Size(522, 0);
            this.metadataControl.TabIndex = 0;
            // 
            // tbxAttempts
            // 
            resources.ApplyResources(this.tbxAttempts, "tbxAttempts");
            this.tableLayoutPanel1.SetColumnSpan(this.tbxAttempts, 2);
            this.tbxAttempts.Name = "tbxAttempts";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel2, 4);
            this.tableLayoutPanel2.Controls.Add(this.btnCancel, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnOK, 0, 0);
<<<<<<< HEAD
=======
            this.tableLayoutPanel2.Location = new System.Drawing.Point(491, 475);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
>>>>>>> refs/remotes/LiveSplit/master
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
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
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblDescription
            // 
<<<<<<< HEAD
            resources.ApplyResources(this.lblDescription, "lblDescription");
            this.tableLayoutPanel1.SetColumnSpan(this.lblDescription, 5);
            this.lblDescription.Name = "lblDescription";
            // 
            // btnActivate
            // 
            resources.ApplyResources(this.btnActivate, "btnActivate");
            this.btnActivate.Name = "btnActivate";
            this.btnActivate.UseVisualStyleBackColor = true;
            this.btnActivate.Click += new System.EventHandler(this.btnActivate_Click);
            // 
            // btnSettings
            // 
            resources.ApplyResources(this.btnSettings, "btnSettings");
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
=======
            this.lblDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDescription.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lblDescription, 4);
            this.lblDescription.Location = new System.Drawing.Point(140, 121);
            this.lblDescription.Margin = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(281, 13);
            this.lblDescription.TabIndex = 13;
            this.lblDescription.Text = "Description";
            // 
>>>>>>> refs/remotes/LiveSplit/master
            // btnInsert
            // 
            resources.ApplyResources(this.btnInsert, "btnInsert");
            this.btnInsert.Name = "btnInsert";
            this.btnInsert.UseVisualStyleBackColor = true;
            this.btnInsert.Click += new System.EventHandler(this.btnInsert_Click);
            // 
            // btnAdd
            // 
            resources.ApplyResources(this.btnAdd, "btnAdd");
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            resources.ApplyResources(this.btnRemove, "btnRemove");
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnMoveUp
            // 
            resources.ApplyResources(this.btnMoveUp, "btnMoveUp");
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnMoveDown
            // 
            resources.ApplyResources(this.btnMoveDown, "btnMoveDown");
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnAddComparison
            // 
            resources.ApplyResources(this.btnAddComparison, "btnAddComparison");
            this.btnAddComparison.Name = "btnAddComparison";
            this.btnAddComparison.UseVisualStyleBackColor = true;
            this.btnAddComparison.Click += new System.EventHandler(this.btnAddComparison_Click);
            // 
            // btnImportComparison
            // 
            resources.ApplyResources(this.btnImportComparison, "btnImportComparison");
            this.btnImportComparison.Name = "btnImportComparison";
            this.btnImportComparison.UseVisualStyleBackColor = true;
            this.btnImportComparison.Click += new System.EventHandler(this.btnImportComparison_Click);
            // 
            // btnOther
            // 
            resources.ApplyResources(this.btnOther, "btnOther");
            this.btnOther.Name = "btnOther";
            this.btnOther.UseVisualStyleBackColor = true;
            this.btnOther.Click += new System.EventHandler(this.btnOther_Click);
            // 
            // flowLayoutPanel1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 3);
            this.flowLayoutPanel1.Controls.Add(this.btnWebsite);
            this.flowLayoutPanel1.Controls.Add(this.btnSettings);
            this.flowLayoutPanel1.Controls.Add(this.btnActivate);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(429, 113);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 3, 7, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(247, 29);
            this.flowLayoutPanel1.TabIndex = 18;
            // 
            // btnWebsite
            // 
            this.btnWebsite.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnWebsite.Location = new System.Drawing.Point(169, 3);
            this.btnWebsite.Name = "btnWebsite";
            this.btnWebsite.Size = new System.Drawing.Size(75, 23);
            this.btnWebsite.TabIndex = 7;
            this.btnWebsite.Text = "Website";
            this.btnWebsite.UseVisualStyleBackColor = true;
            this.btnWebsite.Visible = false;
            this.btnWebsite.Click += new System.EventHandler(this.btnWebsite_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnSettings.Location = new System.Drawing.Point(88, 3);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(75, 23);
            this.btnSettings.TabIndex = 6;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnActivate
            // 
            this.btnActivate.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnActivate.Location = new System.Drawing.Point(7, 3);
            this.btnActivate.Name = "btnActivate";
            this.btnActivate.Size = new System.Drawing.Size(75, 23);
            this.btnActivate.TabIndex = 5;
            this.btnActivate.Text = "Activate";
            this.btnActivate.UseVisualStyleBackColor = true;
            this.btnActivate.Click += new System.EventHandler(this.btnActivate_Click);
            // 
            // RemoveIconMenu
            // 
            resources.ApplyResources(this.RemoveIconMenu, "RemoveIconMenu");
            this.RemoveIconMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setIconToolStripMenuItem,
            this.downloadBoxartToolStripMenuItem,
            this.downloadIconToolStripMenuItem,
            this.openFromURLMenuItem,
            this.removeIconToolStripMenuItem});
            this.RemoveIconMenu.Name = "RemoveIconMenu";
<<<<<<< HEAD
=======
            this.RemoveIconMenu.Size = new System.Drawing.Size(170, 136);
>>>>>>> refs/remotes/LiveSplit/master
            // 
            // setIconToolStripMenuItem
            // 
<<<<<<< HEAD
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
=======
            this.setIconToolStripMenuItem.Name = "setIconToolStripMenuItem";
            this.setIconToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.setIconToolStripMenuItem.Text = "Set Icon...";
            this.setIconToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
>>>>>>> refs/remotes/LiveSplit/master
            // 
            // downloadBoxartToolStripMenuItem
            // 
            resources.ApplyResources(this.downloadBoxartToolStripMenuItem, "downloadBoxartToolStripMenuItem");
            this.downloadBoxartToolStripMenuItem.Name = "downloadBoxartToolStripMenuItem";
            this.downloadBoxartToolStripMenuItem.Click += new System.EventHandler(this.downloadBoxartToolStripMenuItem_Click);
            // 
            // downloadIconToolStripMenuItem
            // 
            this.downloadIconToolStripMenuItem.Name = "downloadIconToolStripMenuItem";
            this.downloadIconToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.downloadIconToolStripMenuItem.Text = "Download Icon";
            this.downloadIconToolStripMenuItem.Click += new System.EventHandler(this.downloadIconToolStripMenuItem_Click);
            // 
            // openFromURLMenuItem
            // 
            resources.ApplyResources(this.openFromURLMenuItem, "openFromURLMenuItem");
            this.openFromURLMenuItem.Name = "openFromURLMenuItem";
            this.openFromURLMenuItem.Click += new System.EventHandler(this.openFromURLMenuItem_Click);
            // 
            // removeIconToolStripMenuItem
            // 
            resources.ApplyResources(this.removeIconToolStripMenuItem, "removeIconToolStripMenuItem");
            this.removeIconToolStripMenuItem.Name = "removeIconToolStripMenuItem";
            this.removeIconToolStripMenuItem.Click += new System.EventHandler(this.removeIconToolStripMenuItem_Click);
            // 
            // ImportComparisonMenu
            // 
            resources.ApplyResources(this.ImportComparisonMenu, "ImportComparisonMenu");
            this.ImportComparisonMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fromFileToolStripMenuItem,
            this.fromURLToolStripMenuItem,
            this.fromSpeedruncomToolStripMenuItem});
            this.ImportComparisonMenu.Name = "ImportComparisonMenu";
            // 
            // fromFileToolStripMenuItem
            // 
            resources.ApplyResources(this.fromFileToolStripMenuItem, "fromFileToolStripMenuItem");
            this.fromFileToolStripMenuItem.Name = "fromFileToolStripMenuItem";
            this.fromFileToolStripMenuItem.Click += new System.EventHandler(this.fromFileToolStripMenuItem_Click);
            // 
            // fromURLToolStripMenuItem
            // 
            resources.ApplyResources(this.fromURLToolStripMenuItem, "fromURLToolStripMenuItem");
            this.fromURLToolStripMenuItem.Name = "fromURLToolStripMenuItem";
            this.fromURLToolStripMenuItem.Click += new System.EventHandler(this.fromURLToolStripMenuItem_Click);
            // 
            // fromSpeedruncomToolStripMenuItem
            // 
            resources.ApplyResources(this.fromSpeedruncomToolStripMenuItem, "fromSpeedruncomToolStripMenuItem");
            this.fromSpeedruncomToolStripMenuItem.Name = "fromSpeedruncomToolStripMenuItem";
            this.fromSpeedruncomToolStripMenuItem.Click += new System.EventHandler(this.fromSpeedruncomToolStripMenuItem_Click);
            // 
            // OtherMenu
            // 
            resources.ApplyResources(this.OtherMenu, "OtherMenu");
            this.OtherMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearHistoryToolStripMenuItem,
            this.clearTimesToolStripMenuItem,
            this.cleanSumOfBestToolStripMenuItem});
            this.OtherMenu.Name = "OtherMenu";
            // 
            // clearHistoryToolStripMenuItem
            // 
            resources.ApplyResources(this.clearHistoryToolStripMenuItem, "clearHistoryToolStripMenuItem");
            this.clearHistoryToolStripMenuItem.Name = "clearHistoryToolStripMenuItem";
            this.clearHistoryToolStripMenuItem.Click += new System.EventHandler(this.clearHistoryToolStripMenuItem_Click);
            // 
            // clearTimesToolStripMenuItem
            // 
            resources.ApplyResources(this.clearTimesToolStripMenuItem, "clearTimesToolStripMenuItem");
            this.clearTimesToolStripMenuItem.Name = "clearTimesToolStripMenuItem";
            this.clearTimesToolStripMenuItem.Click += new System.EventHandler(this.clearTimesToolStripMenuItem_Click);
            // 
            // cleanSumOfBestToolStripMenuItem
            // 
            resources.ApplyResources(this.cleanSumOfBestToolStripMenuItem, "cleanSumOfBestToolStripMenuItem");
            this.cleanSumOfBestToolStripMenuItem.Name = "cleanSumOfBestToolStripMenuItem";
            this.cleanSumOfBestToolStripMenuItem.Click += new System.EventHandler(this.cleanSumOfBestToolStripMenuItem_Click);
            // 
            // iRunBindingSource
            // 
            this.iRunBindingSource.DataSource = typeof(LiveSplit.Model.IRun);
            // 
            // iSegmentBindingSource
            // 
            this.iSegmentBindingSource.DataSource = typeof(LiveSplit.Model.ISegment);
            // 
            // RunEditorDialog
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
<<<<<<< HEAD
=======
            this.ClientSize = new System.Drawing.Size(684, 511);
>>>>>>> refs/remotes/LiveSplit/master
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "RunEditorDialog";
            this.Load += new System.EventHandler(this.RunEditorDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.runGrid)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picGameIcon)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.Metadata.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.RemoveIconMenu.ResumeLayout(false);
            this.ImportComparisonMenu.ResumeLayout(false);
            this.OtherMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.iRunBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iSegmentBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView runGrid;
        private System.Windows.Forms.BindingSource iSegmentBindingSource;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox picGameIcon;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.BindingSource iRunBindingSource;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnInsert;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.ContextMenuStrip RemoveIconMenu;
        private System.Windows.Forms.ToolStripMenuItem removeIconToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setIconToolStripMenuItem;
        private CustomAutoCompleteComboBox cbxGameName;
        private System.Windows.Forms.ComboBox cbxRunCategory;
        private System.Windows.Forms.TextBox tbxTimeOffset;
        private System.Windows.Forms.TextBox tbxAttempts;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ToolStripMenuItem downloadBoxartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFromURLMenuItem;
        private System.Windows.Forms.Button btnAddComparison;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage RealTime;
        private System.Windows.Forms.TabPage GameTime;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Button btnActivate;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.Button btnImportComparison;
        private System.Windows.Forms.ContextMenuStrip ImportComparisonMenu;
        private System.Windows.Forms.ToolStripMenuItem fromFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromURLToolStripMenuItem;
        private System.Windows.Forms.Button btnOther;
        private System.Windows.Forms.ContextMenuStrip OtherMenu;
        private System.Windows.Forms.ToolStripMenuItem clearHistoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearTimesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cleanSumOfBestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromSpeedruncomToolStripMenuItem;
        private System.Windows.Forms.TabPage Metadata;
        private MetadataControl metadataControl;
        private System.Windows.Forms.Button btnWebsite;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ToolStripMenuItem downloadIconToolStripMenuItem;
    }
}