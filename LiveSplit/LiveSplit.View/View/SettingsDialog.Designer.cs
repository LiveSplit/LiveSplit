using LiveSplit.Options;

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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnChooseRaceProvider = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.btnLogOut = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.chkDeactivateForOtherPrograms = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkGlobalHotkeys = new System.Windows.Forms.CheckBox();
            this.chkDoubleTap = new System.Windows.Forms.CheckBox();
            this.txtStartSplit = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtReset = new System.Windows.Forms.TextBox();
            this.txtPause = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSkip = new System.Windows.Forms.TextBox();
            this.txtUndo = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtToggle = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtSwitchPrevious = new System.Windows.Forms.TextBox();
            this.txtSwitchNext = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtDelay = new System.Windows.Forms.TextBox();
            this.grpHotkeyProfiles = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.lblProfile = new System.Windows.Forms.Label();
            this.cmbHotkeyProfiles = new System.Windows.Forms.ComboBox();
            this.btnRemoveProfile = new System.Windows.Forms.Button();
            this.btnRenameProfile = new System.Windows.Forms.Button();
            this.btnNewProfile = new System.Windows.Forms.Button();
            this.chkAllowGamepads = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbxRaceViewer = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.btnChooseComparisons = new System.Windows.Forms.Button();
            this.chkSimpleSOB = new System.Windows.Forms.CheckBox();
            this.chkWarnOnReset = new System.Windows.Forms.CheckBox();
            this.panelRefreshRate = new System.Windows.Forms.Panel();
            this.txtRefreshRate = new System.Windows.Forms.TextBox();
            this.labelRefreshRate = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.grpHotkeyProfiles.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.panelRefreshRate.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 245F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 73F));
            this.tableLayoutPanel1.Controls.Add(this.comboBox1, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.btnChooseRaceProvider, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.btnLogOut, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 2, 8);
            this.tableLayoutPanel1.Controls.Add(this.cbxRaceViewer, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label11, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label12, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label13, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnChooseComparisons, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.chkSimpleSOB, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.chkWarnOnReset, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.panelRefreshRate, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.btnOK, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.label14, 0, 7);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(9, 8);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 9;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 14F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(507, 762);
            this.tableLayoutPanel1.TabIndex = 0;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // btnChooseRaceProvider
            // 
            this.btnChooseRaceProvider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.btnChooseRaceProvider, 3);
            this.btnChooseRaceProvider.Location = new System.Drawing.Point(249, 561);
            this.btnChooseRaceProvider.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnChooseRaceProvider.Name = "btnChooseRaceProvider";
            this.btnChooseRaceProvider.Size = new System.Drawing.Size(254, 27);
            this.btnChooseRaceProvider.TabIndex = 6;
            this.btnChooseRaceProvider.Text = "Manage Racing Services...";
            this.btnChooseRaceProvider.UseVisualStyleBackColor = true;
            this.btnChooseRaceProvider.Click += new System.EventHandler(this.btnChooseRaceProvider_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.btnOK, 2);
            this.btnOK.Location = new System.Drawing.Point(221, 604);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 633);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(237, 15);
            this.label5.TabIndex = 10;
            this.label5.Text = "Saved Accounts:";
            // 
            // btnLogOut
            // 
            this.btnLogOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.btnLogOut, 3);
            this.btnLogOut.Location = new System.Drawing.Point(249, 627);
            this.btnLogOut.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnLogOut.Name = "btnLogOut";
            this.btnLogOut.Size = new System.Drawing.Size(254, 27);
            this.btnLogOut.TabIndex = 8;
            this.btnLogOut.Text = "Log Out of All Accounts";
            this.btnLogOut.UseVisualStyleBackColor = true;
            this.btnLogOut.Click += new System.EventHandler(this.btnLogOut_Click);
            // 
            // groupBox1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 4);
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(4, 3);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Size = new System.Drawing.Size(499, 486);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Hotkeys";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 237F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel2.Controls.Add(this.chkDeactivateForOtherPrograms, 1, 8);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.chkGlobalHotkeys, 0, 8);
            this.tableLayoutPanel2.Controls.Add(this.chkDoubleTap, 0, 9);
            this.tableLayoutPanel2.Controls.Add(this.txtStartSplit, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.txtReset, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.txtPause, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.txtSkip, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.txtUndo, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.label7, 0, 7);
            this.tableLayoutPanel2.Controls.Add(this.txtToggle, 1, 7);
            this.tableLayoutPanel2.Controls.Add(this.label8, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.label9, 0, 6);
            this.tableLayoutPanel2.Controls.Add(this.txtSwitchPrevious, 1, 5);
            this.tableLayoutPanel2.Controls.Add(this.txtSwitchNext, 1, 6);
            this.tableLayoutPanel2.Controls.Add(this.label10, 1, 9);
            this.tableLayoutPanel2.Controls.Add(this.txtDelay, 2, 9);
            this.tableLayoutPanel2.Controls.Add(this.grpHotkeyProfiles, 0, 11);
            this.tableLayoutPanel2.Controls.Add(this.chkAllowGamepads, 0, 10);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(4, 21);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 12;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 96F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(491, 462);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // chkDeactivateForOtherPrograms
            // 
            this.chkDeactivateForOtherPrograms.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.chkDeactivateForOtherPrograms.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.chkDeactivateForOtherPrograms, 2);
            this.chkDeactivateForOtherPrograms.Location = new System.Drawing.Point(246, 267);
            this.chkDeactivateForOtherPrograms.Margin = new System.Windows.Forms.Padding(9, 3, 4, 3);
            this.chkDeactivateForOtherPrograms.Name = "chkDeactivateForOtherPrograms";
            this.chkDeactivateForOtherPrograms.Size = new System.Drawing.Size(119, 27);
            this.chkDeactivateForOtherPrograms.TabIndex = 10;
            this.chkDeactivateForOtherPrograms.Text = "Deactivate For Other Programs";
            this.chkDeactivateForOtherPrograms.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(229, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "Start / Split:";
            // 
            // chkGlobalHotkeys
            // 
            this.chkGlobalHotkeys.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.chkGlobalHotkeys.AutoSize = true;
            this.chkGlobalHotkeys.Location = new System.Drawing.Point(9, 267);
            this.chkGlobalHotkeys.Margin = new System.Windows.Forms.Padding(9, 3, 4, 3);
            this.chkGlobalHotkeys.Name = "chkGlobalHotkeys";
            this.chkGlobalHotkeys.Size = new System.Drawing.Size(89, 27);
            this.chkGlobalHotkeys.TabIndex = 9;
            this.chkGlobalHotkeys.Text = "Global Hotkeys";
            this.chkGlobalHotkeys.UseVisualStyleBackColor = true;
            this.chkGlobalHotkeys.CheckedChanged += new System.EventHandler(this.chkGlobalHotkeys_CheckedChanged);
            // 
            // chkDoubleTap
            // 
            this.chkDoubleTap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkDoubleTap.AutoSize = true;
            this.chkDoubleTap.Location = new System.Drawing.Point(9, 304);
            this.chkDoubleTap.Margin = new System.Windows.Forms.Padding(9, 3, 4, 3);
            this.chkDoubleTap.Name = "chkDoubleTap";
            this.chkDoubleTap.Size = new System.Drawing.Size(224, 19);
            this.chkDoubleTap.TabIndex = 11;
            this.chkDoubleTap.Text = "Double Tap Prevention";
            this.chkDoubleTap.UseVisualStyleBackColor = true;
            // 
            // txtStartSplit
            // 
            this.txtStartSplit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.SetColumnSpan(this.txtStartSplit, 2);
            this.txtStartSplit.Location = new System.Drawing.Point(241, 4);
            this.txtStartSplit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtStartSplit.Name = "txtStartSplit";
            this.txtStartSplit.ReadOnly = true;
            this.txtStartSplit.Size = new System.Drawing.Size(246, 25);
            this.txtStartSplit.TabIndex = 1;
            this.txtStartSplit.Enter += new System.EventHandler(this.Split_Set_Enter);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 42);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(229, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Reset:";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 141);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(229, 15);
            this.label6.TabIndex = 8;
            this.label6.Text = "Pause:";
            // 
            // txtReset
            // 
            this.txtReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.SetColumnSpan(this.txtReset, 2);
            this.txtReset.Location = new System.Drawing.Point(241, 37);
            this.txtReset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtReset.Name = "txtReset";
            this.txtReset.ReadOnly = true;
            this.txtReset.Size = new System.Drawing.Size(246, 25);
            this.txtReset.TabIndex = 2;
            this.txtReset.Enter += new System.EventHandler(this.Reset_Set_Enter);
            // 
            // txtPause
            // 
            this.txtPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.SetColumnSpan(this.txtPause, 2);
            this.txtPause.Location = new System.Drawing.Point(241, 136);
            this.txtPause.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtPause.Name = "txtPause";
            this.txtPause.ReadOnly = true;
            this.txtPause.Size = new System.Drawing.Size(246, 25);
            this.txtPause.TabIndex = 5;
            this.txtPause.Enter += new System.EventHandler(this.Pause_Set_Enter);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 108);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(229, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "Skip Split:";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 75);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(229, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "Undo Split:";
            // 
            // txtSkip
            // 
            this.txtSkip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.SetColumnSpan(this.txtSkip, 2);
            this.txtSkip.Location = new System.Drawing.Point(241, 103);
            this.txtSkip.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtSkip.Name = "txtSkip";
            this.txtSkip.ReadOnly = true;
            this.txtSkip.Size = new System.Drawing.Size(246, 25);
            this.txtSkip.TabIndex = 4;
            this.txtSkip.Enter += new System.EventHandler(this.Skip_Set_Enter);
            // 
            // txtUndo
            // 
            this.txtUndo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.SetColumnSpan(this.txtUndo, 2);
            this.txtUndo.Location = new System.Drawing.Point(241, 70);
            this.txtUndo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtUndo.Name = "txtUndo";
            this.txtUndo.ReadOnly = true;
            this.txtUndo.Size = new System.Drawing.Size(246, 25);
            this.txtUndo.TabIndex = 3;
            this.txtUndo.Enter += new System.EventHandler(this.Undo_Set_Enter);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 240);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(229, 15);
            this.label7.TabIndex = 9;
            this.label7.Text = "Toggle Global Hotkeys:";
            // 
            // txtToggle
            // 
            this.txtToggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.SetColumnSpan(this.txtToggle, 2);
            this.txtToggle.Location = new System.Drawing.Point(241, 235);
            this.txtToggle.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtToggle.Name = "txtToggle";
            this.txtToggle.ReadOnly = true;
            this.txtToggle.Size = new System.Drawing.Size(246, 25);
            this.txtToggle.TabIndex = 8;
            this.txtToggle.Enter += new System.EventHandler(this.Toggle_Set_Enter);
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(4, 174);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(229, 15);
            this.label8.TabIndex = 10;
            this.label8.Text = "Switch Comparison (Previous):";
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(4, 207);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(229, 15);
            this.label9.TabIndex = 12;
            this.label9.Text = "Switch Comparison (Next):";
            // 
            // txtSwitchPrevious
            // 
            this.txtSwitchPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.SetColumnSpan(this.txtSwitchPrevious, 2);
            this.txtSwitchPrevious.Location = new System.Drawing.Point(241, 169);
            this.txtSwitchPrevious.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtSwitchPrevious.Name = "txtSwitchPrevious";
            this.txtSwitchPrevious.ReadOnly = true;
            this.txtSwitchPrevious.Size = new System.Drawing.Size(246, 25);
            this.txtSwitchPrevious.TabIndex = 6;
            this.txtSwitchPrevious.Enter += new System.EventHandler(this.Switch_Previous_Set_Enter);
            // 
            // txtSwitchNext
            // 
            this.txtSwitchNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.SetColumnSpan(this.txtSwitchNext, 2);
            this.txtSwitchNext.Location = new System.Drawing.Point(241, 202);
            this.txtSwitchNext.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtSwitchNext.Name = "txtSwitchNext";
            this.txtSwitchNext.ReadOnly = true;
            this.txtSwitchNext.Size = new System.Drawing.Size(246, 25);
            this.txtSwitchNext.TabIndex = 7;
            this.txtSwitchNext.Enter += new System.EventHandler(this.Switch_Next_Set_Enter);
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(241, 306);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(171, 15);
            this.label10.TabIndex = 14;
            this.label10.Text = "Hotkey Delay (Seconds):";
            // 
            // txtDelay
            // 
            this.txtDelay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDelay.Location = new System.Drawing.Point(420, 301);
            this.txtDelay.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtDelay.Name = "txtDelay";
            this.txtDelay.Size = new System.Drawing.Size(67, 25);
            this.txtDelay.TabIndex = 12;
            this.txtDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // grpHotkeyProfiles
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.grpHotkeyProfiles, 3);
            this.grpHotkeyProfiles.Controls.Add(this.tableLayoutPanel3);
            this.grpHotkeyProfiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpHotkeyProfiles.Location = new System.Drawing.Point(4, 366);
            this.grpHotkeyProfiles.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpHotkeyProfiles.Name = "grpHotkeyProfiles";
            this.grpHotkeyProfiles.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpHotkeyProfiles.Size = new System.Drawing.Size(483, 93);
            this.grpHotkeyProfiles.TabIndex = 13;
            this.grpHotkeyProfiles.TabStop = false;
            this.grpHotkeyProfiles.Text = "Hotkey Profiles";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 89.11917F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10.88083F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 108F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 113F));
            this.tableLayoutPanel3.Controls.Add(this.lblProfile, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.cmbHotkeyProfiles, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnRemoveProfile, 3, 1);
            this.tableLayoutPanel3.Controls.Add(this.btnRenameProfile, 2, 1);
            this.tableLayoutPanel3.Controls.Add(this.btnNewProfile, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(4, 21);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(475, 69);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // lblProfile
            // 
            this.lblProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProfile.AutoSize = true;
            this.lblProfile.Location = new System.Drawing.Point(4, 9);
            this.lblProfile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblProfile.Name = "lblProfile";
            this.lblProfile.Size = new System.Drawing.Size(218, 15);
            this.lblProfile.TabIndex = 0;
            this.lblProfile.Text = "Active Hotkey Profile:";
            // 
            // cmbHotkeyProfiles
            // 
            this.cmbHotkeyProfiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.SetColumnSpan(this.cmbHotkeyProfiles, 3);
            this.cmbHotkeyProfiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHotkeyProfiles.FormattingEnabled = true;
            this.cmbHotkeyProfiles.Location = new System.Drawing.Point(230, 5);
            this.cmbHotkeyProfiles.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cmbHotkeyProfiles.Name = "cmbHotkeyProfiles";
            this.cmbHotkeyProfiles.Size = new System.Drawing.Size(241, 23);
            this.cmbHotkeyProfiles.TabIndex = 0;
            this.cmbHotkeyProfiles.SelectedIndexChanged += new System.EventHandler(this.cmbHotkeyProfiles_SelectedIndexChanged);
            // 
            // btnRemoveProfile
            // 
            this.btnRemoveProfile.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnRemoveProfile.Location = new System.Drawing.Point(371, 37);
            this.btnRemoveProfile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnRemoveProfile.Name = "btnRemoveProfile";
            this.btnRemoveProfile.Size = new System.Drawing.Size(100, 27);
            this.btnRemoveProfile.TabIndex = 3;
            this.btnRemoveProfile.Text = "Remove";
            this.btnRemoveProfile.UseVisualStyleBackColor = true;
            this.btnRemoveProfile.Click += new System.EventHandler(this.btnRemoveProfile_Click);
            // 
            // btnRenameProfile
            // 
            this.btnRenameProfile.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnRenameProfile.Location = new System.Drawing.Point(257, 37);
            this.btnRenameProfile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnRenameProfile.Name = "btnRenameProfile";
            this.btnRenameProfile.Size = new System.Drawing.Size(100, 27);
            this.btnRenameProfile.TabIndex = 2;
            this.btnRenameProfile.Text = "Rename";
            this.btnRenameProfile.UseVisualStyleBackColor = true;
            this.btnRenameProfile.Click += new System.EventHandler(this.btnRenameProfile_Click);
            // 
            // btnNewProfile
            // 
            this.btnNewProfile.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.tableLayoutPanel3.SetColumnSpan(this.btnNewProfile, 2);
            this.btnNewProfile.Location = new System.Drawing.Point(149, 37);
            this.btnNewProfile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnNewProfile.Name = "btnNewProfile";
            this.btnNewProfile.Size = new System.Drawing.Size(100, 27);
            this.btnNewProfile.TabIndex = 1;
            this.btnNewProfile.Text = "New";
            this.btnNewProfile.UseVisualStyleBackColor = true;
            this.btnNewProfile.Click += new System.EventHandler(this.btnNewProfile_Click);
            // 
            // chkAllowGamepads
            // 
            this.chkAllowGamepads.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAllowGamepads.AutoSize = true;
            this.chkAllowGamepads.Location = new System.Drawing.Point(9, 337);
            this.chkAllowGamepads.Margin = new System.Windows.Forms.Padding(9, 3, 4, 3);
            this.chkAllowGamepads.Name = "chkAllowGamepads";
            this.chkAllowGamepads.Size = new System.Drawing.Size(224, 19);
            this.chkAllowGamepads.TabIndex = 15;
            this.chkAllowGamepads.Text = "Allow Gamepads as Hotkeys";
            this.chkAllowGamepads.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.btnCancel, 2);
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(403, 732);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 27);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cbxRaceViewer
            // 
            this.cbxRaceViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.cbxRaceViewer, 3);
            this.cbxRaceViewer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxRaceViewer.FormattingEnabled = true;
            this.cbxRaceViewer.Items.AddRange(new object[] {
            "SpeedRunsLive",
            "MultiTwitch",
            "Kadgar",
            "Speedrun.tv"});
            this.cbxRaceViewer.Location = new System.Drawing.Point(249, 530);
            this.cbxRaceViewer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbxRaceViewer.Name = "cbxRaceViewer";
            this.cbxRaceViewer.Size = new System.Drawing.Size(254, 23);
            this.cbxRaceViewer.TabIndex = 5;
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(4, 534);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(237, 15);
            this.label11.TabIndex = 15;
            this.label11.Text = "Race Viewer:";
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(4, 600);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(237, 15);
            this.label12.TabIndex = 17;
            this.label12.Text = "Active Comparisons:";
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(4, 567);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(237, 15);
            this.label13.TabIndex = 18;
            this.label13.Text = "Racing Services:";
            // 
            // btnChooseComparisons
            // 
            this.btnChooseComparisons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.btnChooseComparisons, 3);
            this.btnChooseComparisons.Location = new System.Drawing.Point(249, 594);
            this.btnChooseComparisons.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnChooseComparisons.Name = "btnChooseComparisons";
            this.btnChooseComparisons.Size = new System.Drawing.Size(254, 27);
            this.btnChooseComparisons.TabIndex = 7;
            this.btnChooseComparisons.Text = "Choose Active Comparisons...";
            this.btnChooseComparisons.UseVisualStyleBackColor = true;
            this.btnChooseComparisons.Click += new System.EventHandler(this.btnChooseComparisons_Click);
            // 
            // chkSimpleSOB
            // 
            this.chkSimpleSOB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSimpleSOB.AutoSize = true;
            this.chkSimpleSOB.Location = new System.Drawing.Point(9, 499);
            this.chkSimpleSOB.Margin = new System.Windows.Forms.Padding(9, 3, 4, 3);
            this.chkSimpleSOB.Name = "chkSimpleSOB";
            this.chkSimpleSOB.Size = new System.Drawing.Size(232, 19);
            this.chkSimpleSOB.TabIndex = 3;
            this.chkSimpleSOB.Text = "Simple Sum of Best Calculation";
            this.chkSimpleSOB.UseVisualStyleBackColor = true;
            this.chkSimpleSOB.CheckedChanged += new System.EventHandler(this.chkSimpleSOB_CheckedChanged);
            // 
            // chkWarnOnReset
            // 
            this.chkWarnOnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkWarnOnReset.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.chkWarnOnReset, 3);
            this.chkWarnOnReset.Location = new System.Drawing.Point(254, 499);
            this.chkWarnOnReset.Margin = new System.Windows.Forms.Padding(9, 3, 4, 3);
            this.chkWarnOnReset.Name = "chkWarnOnReset";
            this.chkWarnOnReset.Size = new System.Drawing.Size(249, 19);
            this.chkWarnOnReset.TabIndex = 4;
            this.chkWarnOnReset.Text = "Warn On Reset If Better Times";
            this.chkWarnOnReset.UseVisualStyleBackColor = true;
            // 
            // panelRefreshRate
            // 
            this.panelRefreshRate.Controls.Add(this.txtRefreshRate);
            this.panelRefreshRate.Controls.Add(this.labelRefreshRate);
            this.panelRefreshRate.Location = new System.Drawing.Point(0, 657);
            this.panelRefreshRate.Margin = new System.Windows.Forms.Padding(0);
            this.panelRefreshRate.Name = "panelRefreshRate";
            this.panelRefreshRate.Size = new System.Drawing.Size(245, 33);
            this.panelRefreshRate.TabIndex = 19;
            // 
            // txtRefreshRate
            // 
            this.txtRefreshRate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txtRefreshRate.Location = new System.Drawing.Point(171, 7);
            this.txtRefreshRate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtRefreshRate.Name = "txtRefreshRate";
            this.txtRefreshRate.Size = new System.Drawing.Size(67, 25);
            this.txtRefreshRate.TabIndex = 15;
            this.txtRefreshRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelRefreshRate
            // 
            this.labelRefreshRate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRefreshRate.AutoSize = true;
            this.labelRefreshRate.Location = new System.Drawing.Point(4, 9);
            this.labelRefreshRate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelRefreshRate.Name = "labelRefreshRate";
            this.labelRefreshRate.Size = new System.Drawing.Size(151, 15);
            this.labelRefreshRate.TabIndex = 19;
            this.labelRefreshRate.Text = "Refresh Rate (Hz):";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.btnOK, 2);
            this.btnOK.Location = new System.Drawing.Point(295, 732);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 27);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 703);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(239, 15);
            this.label14.TabIndex = 20;
            this.label14.Text = "Language:";
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(249, 699);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(146, 23);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // SettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 778);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "SettingsDialog";
            this.Padding = new System.Windows.Forms.Padding(9, 8, 9, 8);
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsDialog_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.grpHotkeyProfiles.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.panelRefreshRate.ResumeLayout(false);
            this.panelRefreshRate.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox txtStartSplit;
        private System.Windows.Forms.TextBox txtReset;
        private System.Windows.Forms.TextBox txtSkip;
        private System.Windows.Forms.TextBox txtUndo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkGlobalHotkeys;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtPause;
        private System.Windows.Forms.CheckBox chkWarnOnReset;
        private System.Windows.Forms.TextBox txtToggle;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chkDoubleTap;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtSwitchPrevious;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtSwitchNext;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtDelay;
        private System.Windows.Forms.ComboBox cbxRaceViewer;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox chkDeactivateForOtherPrograms;
        private System.Windows.Forms.CheckBox chkSimpleSOB;
        private System.Windows.Forms.Button btnChooseComparisons;
        private System.Windows.Forms.GroupBox grpHotkeyProfiles;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label lblProfile;
        private System.Windows.Forms.ComboBox cmbHotkeyProfiles;
        private System.Windows.Forms.Button btnRemoveProfile;
        private System.Windows.Forms.Button btnRenameProfile;
        private System.Windows.Forms.Button btnNewProfile;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnLogOut;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnChooseRaceProvider;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox chkAllowGamepads;
        private System.Windows.Forms.Label labelRefreshRate;
        private System.Windows.Forms.Panel panelRefreshRate;
        private System.Windows.Forms.TextBox txtRefreshRate;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}
