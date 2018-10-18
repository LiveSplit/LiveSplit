namespace LiveSplit.View
{
    partial class SettingsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsDialog));
            this.tplCore = new System.Windows.Forms.TableLayoutPanel();
            this.labelRefreshRate = new System.Windows.Forms.Label();
            this.labelLogOut = new System.Windows.Forms.Label();
            this.btnLogOut = new System.Windows.Forms.Button();
            this.grpHotkey = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.chkDeactivateForOtherPrograms = new System.Windows.Forms.CheckBox();
            this.labelStartSplit = new System.Windows.Forms.Label();
            this.chkGlobalHotkeys = new System.Windows.Forms.CheckBox();
            this.chkDoubleTap = new System.Windows.Forms.CheckBox();
            this.txtStartSplit = new System.Windows.Forms.TextBox();
            this.labelReset = new System.Windows.Forms.Label();
            this.labelPause = new System.Windows.Forms.Label();
            this.txtReset = new System.Windows.Forms.TextBox();
            this.txtPause = new System.Windows.Forms.TextBox();
            this.labelSkip = new System.Windows.Forms.Label();
            this.labelUndo = new System.Windows.Forms.Label();
            this.txtSkip = new System.Windows.Forms.TextBox();
            this.txtUndo = new System.Windows.Forms.TextBox();
            this.labelToggle = new System.Windows.Forms.Label();
            this.txtToggle = new System.Windows.Forms.TextBox();
            this.labelSwitchPrevious = new System.Windows.Forms.Label();
            this.labelSwitchNext = new System.Windows.Forms.Label();
            this.txtSwitchPrevious = new System.Windows.Forms.TextBox();
            this.txtSwitchNext = new System.Windows.Forms.TextBox();
            this.grpHotkeyProfiles = new System.Windows.Forms.GroupBox();
            this.tlpHotkeyProfiles = new System.Windows.Forms.TableLayoutPanel();
            this.tlpActiveHotkeyProfile = new System.Windows.Forms.TableLayoutPanel();
            this.labelActiveHotkeyProfiles = new System.Windows.Forms.Label();
            this.cmbHotkeyProfiles = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnRemoveProfile = new System.Windows.Forms.Button();
            this.btnNewProfile = new System.Windows.Forms.Button();
            this.btnRenameProfile = new System.Windows.Forms.Button();
            this.panelDelay = new System.Windows.Forms.Panel();
            this.labelDelay = new System.Windows.Forms.Label();
            this.txtDelay = new System.Windows.Forms.TextBox();
            this.cbxRaceViewer = new System.Windows.Forms.ComboBox();
            this.labelRaceViewer = new System.Windows.Forms.Label();
            this.labelChooseComparisons = new System.Windows.Forms.Label();
            this.btnChooseComparisons = new System.Windows.Forms.Button();
            this.chkSimpleSOB = new System.Windows.Forms.CheckBox();
            this.chkWarnOnReset = new System.Windows.Forms.CheckBox();
            this.panelOkCancel = new System.Windows.Forms.Panel();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panelRefreshRate = new System.Windows.Forms.Panel();
            this.txtRefreshRate = new System.Windows.Forms.TextBox();
            this.tplCore.SuspendLayout();
            this.grpHotkey.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.grpHotkeyProfiles.SuspendLayout();
            this.tlpHotkeyProfiles.SuspendLayout();
            this.tlpActiveHotkeyProfile.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelDelay.SuspendLayout();
            this.panelOkCancel.SuspendLayout();
            this.panelRefreshRate.SuspendLayout();
            this.SuspendLayout();
            // 
            // tplCore
            // 
            this.tplCore.ColumnCount = 2;
            this.tplCore.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 184F));
            this.tplCore.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tplCore.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tplCore.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tplCore.Controls.Add(this.labelLogOut, 0, 4);
            this.tplCore.Controls.Add(this.btnLogOut, 1, 4);
            this.tplCore.Controls.Add(this.grpHotkey, 0, 0);
            this.tplCore.Controls.Add(this.cbxRaceViewer, 1, 2);
            this.tplCore.Controls.Add(this.labelRaceViewer, 0, 2);
            this.tplCore.Controls.Add(this.labelChooseComparisons, 0, 3);
            this.tplCore.Controls.Add(this.btnChooseComparisons, 1, 3);
            this.tplCore.Controls.Add(this.chkSimpleSOB, 0, 1);
            this.tplCore.Controls.Add(this.chkWarnOnReset, 1, 1);
            this.tplCore.Controls.Add(this.panelOkCancel, 1, 5);
            this.tplCore.Controls.Add(this.panelRefreshRate, 0, 5);
            this.tplCore.Location = new System.Drawing.Point(7, 7);
            this.tplCore.Name = "tplCore";
            this.tplCore.RowCount = 6;
            this.tplCore.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tplCore.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tplCore.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tplCore.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tplCore.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tplCore.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tplCore.Size = new System.Drawing.Size(380, 544);
            this.tplCore.TabIndex = 0;
            // 
            // labelRefreshRate
            // 
            this.labelRefreshRate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRefreshRate.AutoSize = true;
            this.labelRefreshRate.Location = new System.Drawing.Point(3, 9);
            this.labelRefreshRate.Name = "labelRefreshRate";
            this.labelRefreshRate.Size = new System.Drawing.Size(73, 13);
            this.labelRefreshRate.TabIndex = 19;
            this.labelRefreshRate.Text = "Refresh Rate:";
            // 
            // labelLogOut
            // 
            this.labelLogOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelLogOut.AutoSize = true;
            this.labelLogOut.Location = new System.Drawing.Point(3, 493);
            this.labelLogOut.Name = "labelLogOut";
            this.labelLogOut.Size = new System.Drawing.Size(178, 13);
            this.labelLogOut.TabIndex = 10;
            this.labelLogOut.Text = "Saved Accounts:";
            // 
            // btnLogOut
            // 
            this.btnLogOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogOut.Location = new System.Drawing.Point(187, 488);
            this.btnLogOut.Name = "btnLogOut";
            this.btnLogOut.Size = new System.Drawing.Size(190, 23);
            this.btnLogOut.TabIndex = 5;
            this.btnLogOut.Text = "Log Out of All Accounts";
            this.btnLogOut.UseVisualStyleBackColor = true;
            this.btnLogOut.Click += new System.EventHandler(this.btnLogOut_Click);
            // 
            // grpHotkey
            // 
            this.grpHotkey.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tplCore.SetColumnSpan(this.grpHotkey, 2);
            this.grpHotkey.Controls.Add(this.tableLayoutPanel2);
            this.grpHotkey.Location = new System.Drawing.Point(3, 3);
            this.grpHotkey.Name = "grpHotkey";
            this.grpHotkey.Size = new System.Drawing.Size(374, 392);
            this.grpHotkey.TabIndex = 0;
            this.grpHotkey.TabStop = false;
            this.grpHotkey.Text = "Hotkeys";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.chkDeactivateForOtherPrograms, 1, 8);
            this.tableLayoutPanel2.Controls.Add(this.labelStartSplit, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.chkGlobalHotkeys, 0, 8);
            this.tableLayoutPanel2.Controls.Add(this.chkDoubleTap, 0, 9);
            this.tableLayoutPanel2.Controls.Add(this.txtStartSplit, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.labelReset, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.labelPause, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.txtReset, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.txtPause, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.labelSkip, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.labelUndo, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.txtSkip, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.txtUndo, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.labelToggle, 0, 7);
            this.tableLayoutPanel2.Controls.Add(this.txtToggle, 1, 7);
            this.tableLayoutPanel2.Controls.Add(this.labelSwitchPrevious, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.labelSwitchNext, 0, 6);
            this.tableLayoutPanel2.Controls.Add(this.txtSwitchPrevious, 1, 5);
            this.tableLayoutPanel2.Controls.Add(this.txtSwitchNext, 1, 6);
            this.tableLayoutPanel2.Controls.Add(this.grpHotkeyProfiles, 0, 10);
            this.tableLayoutPanel2.Controls.Add(this.panelDelay, 1, 9);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 11;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 83F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(368, 373);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // chkDeactivateForOtherPrograms
            // 
            this.chkDeactivateForOtherPrograms.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkDeactivateForOtherPrograms.Location = new System.Drawing.Point(187, 238);
            this.chkDeactivateForOtherPrograms.Margin = new System.Windows.Forms.Padding(7, 3, 3, 3);
            this.chkDeactivateForOtherPrograms.Name = "chkDeactivateForOtherPrograms";
            this.chkDeactivateForOtherPrograms.Size = new System.Drawing.Size(178, 17);
            this.chkDeactivateForOtherPrograms.TabIndex = 9;
            this.chkDeactivateForOtherPrograms.Text = "Deactivate For Other Programs";
            this.chkDeactivateForOtherPrograms.UseVisualStyleBackColor = true;
            // 
            // labelStartSplit
            // 
            this.labelStartSplit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelStartSplit.AutoSize = true;
            this.labelStartSplit.Location = new System.Drawing.Point(3, 8);
            this.labelStartSplit.Name = "labelStartSplit";
            this.labelStartSplit.Size = new System.Drawing.Size(174, 13);
            this.labelStartSplit.TabIndex = 4;
            this.labelStartSplit.Text = "Start / Split:";
            // 
            // chkGlobalHotkeys
            // 
            this.chkGlobalHotkeys.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkGlobalHotkeys.Location = new System.Drawing.Point(7, 238);
            this.chkGlobalHotkeys.Margin = new System.Windows.Forms.Padding(7, 3, 3, 3);
            this.chkGlobalHotkeys.Name = "chkGlobalHotkeys";
            this.chkGlobalHotkeys.Size = new System.Drawing.Size(170, 17);
            this.chkGlobalHotkeys.TabIndex = 8;
            this.chkGlobalHotkeys.Text = "Global Hotkeys";
            this.chkGlobalHotkeys.UseVisualStyleBackColor = true;
            this.chkGlobalHotkeys.CheckedChanged += new System.EventHandler(this.chkGlobalHotkeys_CheckedChanged);
            // 
            // chkDoubleTap
            // 
            this.chkDoubleTap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkDoubleTap.Location = new System.Drawing.Point(7, 267);
            this.chkDoubleTap.Margin = new System.Windows.Forms.Padding(7, 3, 3, 3);
            this.chkDoubleTap.Name = "chkDoubleTap";
            this.chkDoubleTap.Size = new System.Drawing.Size(170, 17);
            this.chkDoubleTap.TabIndex = 10;
            this.chkDoubleTap.Text = "Double Tap Prevention";
            this.chkDoubleTap.UseVisualStyleBackColor = true;
            // 
            // txtStartSplit
            // 
            this.txtStartSplit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStartSplit.Location = new System.Drawing.Point(183, 4);
            this.txtStartSplit.Name = "txtStartSplit";
            this.txtStartSplit.ReadOnly = true;
            this.txtStartSplit.Size = new System.Drawing.Size(182, 20);
            this.txtStartSplit.TabIndex = 0;
            this.txtStartSplit.Enter += new System.EventHandler(this.Split_Set_Enter);
            // 
            // labelReset
            // 
            this.labelReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelReset.AutoSize = true;
            this.labelReset.Location = new System.Drawing.Point(3, 37);
            this.labelReset.Name = "labelReset";
            this.labelReset.Size = new System.Drawing.Size(174, 13);
            this.labelReset.TabIndex = 5;
            this.labelReset.Text = "Reset:";
            // 
            // labelPause
            // 
            this.labelPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPause.AutoSize = true;
            this.labelPause.Location = new System.Drawing.Point(3, 124);
            this.labelPause.Name = "labelPause";
            this.labelPause.Size = new System.Drawing.Size(174, 13);
            this.labelPause.TabIndex = 8;
            this.labelPause.Text = "Pause:";
            // 
            // txtReset
            // 
            this.txtReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReset.Location = new System.Drawing.Point(183, 33);
            this.txtReset.Name = "txtReset";
            this.txtReset.ReadOnly = true;
            this.txtReset.Size = new System.Drawing.Size(182, 20);
            this.txtReset.TabIndex = 1;
            this.txtReset.Enter += new System.EventHandler(this.Reset_Set_Enter);
            // 
            // txtPause
            // 
            this.txtPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPause.Location = new System.Drawing.Point(183, 120);
            this.txtPause.Name = "txtPause";
            this.txtPause.ReadOnly = true;
            this.txtPause.Size = new System.Drawing.Size(182, 20);
            this.txtPause.TabIndex = 4;
            this.txtPause.Enter += new System.EventHandler(this.Pause_Set_Enter);
            // 
            // labelSkip
            // 
            this.labelSkip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSkip.AutoSize = true;
            this.labelSkip.Location = new System.Drawing.Point(3, 95);
            this.labelSkip.Name = "labelSkip";
            this.labelSkip.Size = new System.Drawing.Size(174, 13);
            this.labelSkip.TabIndex = 6;
            this.labelSkip.Text = "Skip Split:";
            // 
            // labelUndo
            // 
            this.labelUndo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelUndo.AutoSize = true;
            this.labelUndo.Location = new System.Drawing.Point(3, 66);
            this.labelUndo.Name = "labelUndo";
            this.labelUndo.Size = new System.Drawing.Size(174, 13);
            this.labelUndo.TabIndex = 7;
            this.labelUndo.Text = "Undo Split:";
            // 
            // txtSkip
            // 
            this.txtSkip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSkip.Location = new System.Drawing.Point(183, 91);
            this.txtSkip.Name = "txtSkip";
            this.txtSkip.ReadOnly = true;
            this.txtSkip.Size = new System.Drawing.Size(182, 20);
            this.txtSkip.TabIndex = 3;
            this.txtSkip.Enter += new System.EventHandler(this.Skip_Set_Enter);
            // 
            // txtUndo
            // 
            this.txtUndo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUndo.Location = new System.Drawing.Point(183, 62);
            this.txtUndo.Name = "txtUndo";
            this.txtUndo.ReadOnly = true;
            this.txtUndo.Size = new System.Drawing.Size(182, 20);
            this.txtUndo.TabIndex = 2;
            this.txtUndo.Enter += new System.EventHandler(this.Undo_Set_Enter);
            // 
            // labelToggle
            // 
            this.labelToggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelToggle.AutoSize = true;
            this.labelToggle.Location = new System.Drawing.Point(3, 211);
            this.labelToggle.Name = "labelToggle";
            this.labelToggle.Size = new System.Drawing.Size(174, 13);
            this.labelToggle.TabIndex = 9;
            this.labelToggle.Text = "Toggle Global Hotkeys:";
            // 
            // txtToggle
            // 
            this.txtToggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtToggle.Location = new System.Drawing.Point(183, 207);
            this.txtToggle.Name = "txtToggle";
            this.txtToggle.ReadOnly = true;
            this.txtToggle.Size = new System.Drawing.Size(182, 20);
            this.txtToggle.TabIndex = 7;
            this.txtToggle.Enter += new System.EventHandler(this.Toggle_Set_Enter);
            // 
            // labelSwitchPrevious
            // 
            this.labelSwitchPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSwitchPrevious.AutoSize = true;
            this.labelSwitchPrevious.Location = new System.Drawing.Point(3, 153);
            this.labelSwitchPrevious.Name = "labelSwitchPrevious";
            this.labelSwitchPrevious.Size = new System.Drawing.Size(174, 13);
            this.labelSwitchPrevious.TabIndex = 10;
            this.labelSwitchPrevious.Text = "Switch Comparison (Previous):";
            // 
            // labelSwitchNext
            // 
            this.labelSwitchNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSwitchNext.AutoSize = true;
            this.labelSwitchNext.Location = new System.Drawing.Point(3, 182);
            this.labelSwitchNext.Name = "labelSwitchNext";
            this.labelSwitchNext.Size = new System.Drawing.Size(174, 13);
            this.labelSwitchNext.TabIndex = 12;
            this.labelSwitchNext.Text = "Switch Comparison (Next):";
            // 
            // txtSwitchPrevious
            // 
            this.txtSwitchPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSwitchPrevious.Location = new System.Drawing.Point(183, 149);
            this.txtSwitchPrevious.Name = "txtSwitchPrevious";
            this.txtSwitchPrevious.ReadOnly = true;
            this.txtSwitchPrevious.Size = new System.Drawing.Size(182, 20);
            this.txtSwitchPrevious.TabIndex = 5;
            this.txtSwitchPrevious.Enter += new System.EventHandler(this.Switch_Previous_Set_Enter);
            // 
            // txtSwitchNext
            // 
            this.txtSwitchNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSwitchNext.Location = new System.Drawing.Point(183, 178);
            this.txtSwitchNext.Name = "txtSwitchNext";
            this.txtSwitchNext.ReadOnly = true;
            this.txtSwitchNext.Size = new System.Drawing.Size(182, 20);
            this.txtSwitchNext.TabIndex = 6;
            this.txtSwitchNext.Enter += new System.EventHandler(this.Switch_Next_Set_Enter);
            // 
            // grpHotkeyProfiles
            // 
            this.grpHotkeyProfiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.SetColumnSpan(this.grpHotkeyProfiles, 2);
            this.grpHotkeyProfiles.Controls.Add(this.tlpHotkeyProfiles);
            this.grpHotkeyProfiles.Location = new System.Drawing.Point(3, 293);
            this.grpHotkeyProfiles.Name = "grpHotkeyProfiles";
            this.grpHotkeyProfiles.Size = new System.Drawing.Size(362, 77);
            this.grpHotkeyProfiles.TabIndex = 12;
            this.grpHotkeyProfiles.TabStop = false;
            this.grpHotkeyProfiles.Text = "Hotkey Profiles";
            // 
            // tlpHotkeyProfiles
            // 
            this.tlpHotkeyProfiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpHotkeyProfiles.ColumnCount = 1;
            this.tlpHotkeyProfiles.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpHotkeyProfiles.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpHotkeyProfiles.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpHotkeyProfiles.Controls.Add(this.tlpActiveHotkeyProfile, 0, 0);
            this.tlpHotkeyProfiles.Controls.Add(this.panel1, 0, 1);
            this.tlpHotkeyProfiles.Location = new System.Drawing.Point(3, 16);
            this.tlpHotkeyProfiles.Name = "tlpHotkeyProfiles";
            this.tlpHotkeyProfiles.RowCount = 2;
            this.tlpHotkeyProfiles.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tlpHotkeyProfiles.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tlpHotkeyProfiles.Size = new System.Drawing.Size(356, 58);
            this.tlpHotkeyProfiles.TabIndex = 0;
            // 
            // tlpActiveHotkeyProfile
            // 
            this.tlpActiveHotkeyProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpActiveHotkeyProfile.ColumnCount = 2;
            this.tlpActiveHotkeyProfile.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpActiveHotkeyProfile.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpActiveHotkeyProfile.Controls.Add(this.labelActiveHotkeyProfiles, 0, 0);
            this.tlpActiveHotkeyProfile.Controls.Add(this.cmbHotkeyProfiles, 1, 0);
            this.tlpActiveHotkeyProfile.Location = new System.Drawing.Point(0, 0);
            this.tlpActiveHotkeyProfile.Margin = new System.Windows.Forms.Padding(0);
            this.tlpActiveHotkeyProfile.Name = "tlpActiveHotkeyProfile";
            this.tlpActiveHotkeyProfile.RowCount = 1;
            this.tlpActiveHotkeyProfile.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpActiveHotkeyProfile.Size = new System.Drawing.Size(356, 29);
            this.tlpActiveHotkeyProfile.TabIndex = 0;
            // 
            // labelActiveHotkeyProfiles
            // 
            this.labelActiveHotkeyProfiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelActiveHotkeyProfiles.AutoSize = true;
            this.labelActiveHotkeyProfiles.Location = new System.Drawing.Point(3, 8);
            this.labelActiveHotkeyProfiles.Name = "labelActiveHotkeyProfiles";
            this.labelActiveHotkeyProfiles.Size = new System.Drawing.Size(172, 13);
            this.labelActiveHotkeyProfiles.TabIndex = 0;
            this.labelActiveHotkeyProfiles.Text = "Active Hotkey Profile:";
            // 
            // cmbHotkeyProfiles
            // 
            this.cmbHotkeyProfiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbHotkeyProfiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHotkeyProfiles.FormattingEnabled = true;
            this.cmbHotkeyProfiles.Location = new System.Drawing.Point(181, 4);
            this.cmbHotkeyProfiles.Name = "cmbHotkeyProfiles";
            this.cmbHotkeyProfiles.Size = new System.Drawing.Size(172, 21);
            this.cmbHotkeyProfiles.TabIndex = 0;
            this.cmbHotkeyProfiles.SelectedIndexChanged += new System.EventHandler(this.cmbHotkeyProfiles_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.btnRemoveProfile);
            this.panel1.Controls.Add(this.btnNewProfile);
            this.panel1.Controls.Add(this.btnRenameProfile);
            this.panel1.Location = new System.Drawing.Point(0, 29);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(356, 29);
            this.panel1.TabIndex = 1;
            // 
            // btnRemoveProfile
            // 
            this.btnRemoveProfile.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnRemoveProfile.Location = new System.Drawing.Point(278, 3);
            this.btnRemoveProfile.Name = "btnRemoveProfile";
            this.btnRemoveProfile.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveProfile.TabIndex = 3;
            this.btnRemoveProfile.Text = "Remove";
            this.btnRemoveProfile.UseVisualStyleBackColor = true;
            this.btnRemoveProfile.Click += new System.EventHandler(this.btnRemoveProfile_Click);
            // 
            // btnNewProfile
            // 
            this.btnNewProfile.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnNewProfile.Location = new System.Drawing.Point(116, 3);
            this.btnNewProfile.Name = "btnNewProfile";
            this.btnNewProfile.Size = new System.Drawing.Size(75, 23);
            this.btnNewProfile.TabIndex = 1;
            this.btnNewProfile.Text = "New";
            this.btnNewProfile.UseVisualStyleBackColor = true;
            this.btnNewProfile.Click += new System.EventHandler(this.btnNewProfile_Click);
            // 
            // btnRenameProfile
            // 
            this.btnRenameProfile.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnRenameProfile.Location = new System.Drawing.Point(197, 3);
            this.btnRenameProfile.Name = "btnRenameProfile";
            this.btnRenameProfile.Size = new System.Drawing.Size(75, 23);
            this.btnRenameProfile.TabIndex = 2;
            this.btnRenameProfile.Text = "Rename";
            this.btnRenameProfile.UseVisualStyleBackColor = true;
            this.btnRenameProfile.Click += new System.EventHandler(this.btnRenameProfile_Click);
            // 
            // panelDelay
            // 
            this.panelDelay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDelay.Controls.Add(this.labelDelay);
            this.panelDelay.Controls.Add(this.txtDelay);
            this.panelDelay.Location = new System.Drawing.Point(180, 261);
            this.panelDelay.Margin = new System.Windows.Forms.Padding(0);
            this.panelDelay.Name = "panelDelay";
            this.panelDelay.Size = new System.Drawing.Size(188, 29);
            this.panelDelay.TabIndex = 13;
            // 
            // labelDelay
            // 
            this.labelDelay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.labelDelay.Location = new System.Drawing.Point(3, 7);
            this.labelDelay.Name = "labelDelay";
            this.labelDelay.Size = new System.Drawing.Size(125, 13);
            this.labelDelay.TabIndex = 14;
            this.labelDelay.Text = "Hotkey Delay (Seconds):";
            // 
            // txtDelay
            // 
            this.txtDelay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txtDelay.Location = new System.Drawing.Point(134, 4);
            this.txtDelay.Name = "txtDelay";
            this.txtDelay.Size = new System.Drawing.Size(51, 20);
            this.txtDelay.TabIndex = 11;
            this.txtDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // cbxRaceViewer
            // 
            this.cbxRaceViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxRaceViewer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxRaceViewer.FormattingEnabled = true;
            this.cbxRaceViewer.Items.AddRange(new object[] {
            "SpeedRunsLive",
            "MultiTwitch",
            "Kadgar",
            "Speedrun.tv"});
            this.cbxRaceViewer.Location = new System.Drawing.Point(187, 431);
            this.cbxRaceViewer.Name = "cbxRaceViewer";
            this.cbxRaceViewer.Size = new System.Drawing.Size(190, 21);
            this.cbxRaceViewer.TabIndex = 3;
            // 
            // labelRaceViewer
            // 
            this.labelRaceViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRaceViewer.AutoSize = true;
            this.labelRaceViewer.Location = new System.Drawing.Point(3, 435);
            this.labelRaceViewer.Name = "labelRaceViewer";
            this.labelRaceViewer.Size = new System.Drawing.Size(178, 13);
            this.labelRaceViewer.TabIndex = 15;
            this.labelRaceViewer.Text = "Race Viewer:";
            // 
            // labelChooseComparisons
            // 
            this.labelChooseComparisons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelChooseComparisons.AutoSize = true;
            this.labelChooseComparisons.Location = new System.Drawing.Point(3, 464);
            this.labelChooseComparisons.Name = "labelChooseComparisons";
            this.labelChooseComparisons.Size = new System.Drawing.Size(178, 13);
            this.labelChooseComparisons.TabIndex = 17;
            this.labelChooseComparisons.Text = "Active Comparisons:";
            // 
            // btnChooseComparisons
            // 
            this.btnChooseComparisons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChooseComparisons.Location = new System.Drawing.Point(187, 459);
            this.btnChooseComparisons.Name = "btnChooseComparisons";
            this.btnChooseComparisons.Size = new System.Drawing.Size(190, 23);
            this.btnChooseComparisons.TabIndex = 4;
            this.btnChooseComparisons.Text = "Choose Active Comparisons...";
            this.btnChooseComparisons.UseVisualStyleBackColor = true;
            this.btnChooseComparisons.Click += new System.EventHandler(this.btnChooseComparisons_Click);
            // 
            // chkSimpleSOB
            // 
            this.chkSimpleSOB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSimpleSOB.AutoSize = true;
            this.chkSimpleSOB.Location = new System.Drawing.Point(7, 404);
            this.chkSimpleSOB.Margin = new System.Windows.Forms.Padding(7, 3, 3, 3);
            this.chkSimpleSOB.Name = "chkSimpleSOB";
            this.chkSimpleSOB.Size = new System.Drawing.Size(174, 17);
            this.chkSimpleSOB.TabIndex = 1;
            this.chkSimpleSOB.Text = "Simple Sum of Best Calculation";
            this.chkSimpleSOB.UseVisualStyleBackColor = true;
            this.chkSimpleSOB.CheckedChanged += new System.EventHandler(this.chkSimpleSOB_CheckedChanged);
            // 
            // chkWarnOnReset
            // 
            this.chkWarnOnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkWarnOnReset.AutoSize = true;
            this.chkWarnOnReset.Location = new System.Drawing.Point(191, 404);
            this.chkWarnOnReset.Margin = new System.Windows.Forms.Padding(7, 3, 3, 3);
            this.chkWarnOnReset.Name = "chkWarnOnReset";
            this.chkWarnOnReset.Size = new System.Drawing.Size(186, 17);
            this.chkWarnOnReset.TabIndex = 2;
            this.chkWarnOnReset.Text = "Warn On Reset If Better Times";
            this.chkWarnOnReset.UseVisualStyleBackColor = true;
            // 
            // panelOkCancel
            // 
            this.panelOkCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelOkCancel.Controls.Add(this.btnOK);
            this.panelOkCancel.Controls.Add(this.btnCancel);
            this.panelOkCancel.Location = new System.Drawing.Point(184, 514);
            this.panelOkCancel.Margin = new System.Windows.Forms.Padding(0);
            this.panelOkCancel.Name = "panelOkCancel";
            this.panelOkCancel.Size = new System.Drawing.Size(196, 30);
            this.panelOkCancel.TabIndex = 18;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnOK.Location = new System.Drawing.Point(35, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(116, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(77, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // panelRefreshRate
            // 
            this.panelRefreshRate.Controls.Add(this.txtRefreshRate);
            this.panelRefreshRate.Controls.Add(this.labelRefreshRate);
            this.panelRefreshRate.Location = new System.Drawing.Point(0, 514);
            this.panelRefreshRate.Margin = new System.Windows.Forms.Padding(0);
            this.panelRefreshRate.Name = "panelRefreshRate";
            this.panelRefreshRate.Size = new System.Drawing.Size(184, 30);
            this.panelRefreshRate.TabIndex = 19;
            // 
            // txtRefreshRate
            // 
            this.txtRefreshRate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txtRefreshRate.Location = new System.Drawing.Point(128, 6);
            this.txtRefreshRate.Name = "txtRefreshRate";
            this.txtRefreshRate.Size = new System.Drawing.Size(51, 20);
            this.txtRefreshRate.TabIndex = 15;
            this.txtRefreshRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // SettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 559);
            this.Controls.Add(this.tplCore);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsDialog";
            this.Padding = new System.Windows.Forms.Padding(7);
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsDialog_Load);
            this.tplCore.ResumeLayout(false);
            this.tplCore.PerformLayout();
            this.grpHotkey.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.grpHotkeyProfiles.ResumeLayout(false);
            this.tlpHotkeyProfiles.ResumeLayout(false);
            this.tlpActiveHotkeyProfile.ResumeLayout(false);
            this.tlpActiveHotkeyProfile.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panelDelay.ResumeLayout(false);
            this.panelDelay.PerformLayout();
            this.panelOkCancel.ResumeLayout(false);
            this.panelRefreshRate.ResumeLayout(false);
            this.panelRefreshRate.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tplCore;
        private System.Windows.Forms.TextBox txtStartSplit;
        private System.Windows.Forms.TextBox txtReset;
        private System.Windows.Forms.TextBox txtSkip;
        private System.Windows.Forms.TextBox txtUndo;
        private System.Windows.Forms.Label labelUndo;
        private System.Windows.Forms.Label labelSkip;
        private System.Windows.Forms.Label labelReset;
        private System.Windows.Forms.Label labelStartSplit;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkGlobalHotkeys;
        private System.Windows.Forms.Label labelPause;
        private System.Windows.Forms.TextBox txtPause;
        private System.Windows.Forms.CheckBox chkWarnOnReset;
        private System.Windows.Forms.TextBox txtToggle;
        private System.Windows.Forms.Label labelToggle;
        private System.Windows.Forms.CheckBox chkDoubleTap;
        private System.Windows.Forms.GroupBox grpHotkey;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label labelSwitchPrevious;
        private System.Windows.Forms.TextBox txtSwitchPrevious;
        private System.Windows.Forms.Label labelSwitchNext;
        private System.Windows.Forms.TextBox txtSwitchNext;
        private System.Windows.Forms.Label labelDelay;
        private System.Windows.Forms.TextBox txtDelay;
        private System.Windows.Forms.ComboBox cbxRaceViewer;
        private System.Windows.Forms.Label labelRaceViewer;
        private System.Windows.Forms.CheckBox chkDeactivateForOtherPrograms;
        private System.Windows.Forms.CheckBox chkSimpleSOB;
        private System.Windows.Forms.Button btnChooseComparisons;
        private System.Windows.Forms.GroupBox grpHotkeyProfiles;
        private System.Windows.Forms.TableLayoutPanel tlpHotkeyProfiles;
        private System.Windows.Forms.Label labelActiveHotkeyProfiles;
        private System.Windows.Forms.ComboBox cmbHotkeyProfiles;
        private System.Windows.Forms.Button btnRemoveProfile;
        private System.Windows.Forms.Button btnRenameProfile;
        private System.Windows.Forms.Button btnNewProfile;
        private System.Windows.Forms.Label labelLogOut;
        private System.Windows.Forms.Button btnLogOut;
        private System.Windows.Forms.Label labelChooseComparisons;
        private System.Windows.Forms.Panel panelDelay;
        private System.Windows.Forms.Panel panelOkCancel;
        private System.Windows.Forms.TableLayoutPanel tlpActiveHotkeyProfile;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelRefreshRate;
        private System.Windows.Forms.Panel panelRefreshRate;
        private System.Windows.Forms.TextBox txtRefreshRate;
    }
}