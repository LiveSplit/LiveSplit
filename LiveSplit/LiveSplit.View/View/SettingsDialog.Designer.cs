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
            this.btnOK = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.btnOBSInstall = new System.Windows.Forms.Button();
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbxRaceViewer = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.btnChooseComparisons = new System.Windows.Forms.Button();
            this.chkSimpleSOB = new System.Windows.Forms.CheckBox();
            this.chkWarnOnReset = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.btnOK, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnOBSInstall, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.cbxRaceViewer, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label11, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label12, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnChooseComparisons, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.chkSimpleSOB, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.chkWarnOnReset, 1, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.tableLayoutPanel1.SetColumnSpan(this.btnOK, 2);
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // btnOBSInstall
            // 
            resources.ApplyResources(this.btnOBSInstall, "btnOBSInstall");
            this.tableLayoutPanel1.SetColumnSpan(this.btnOBSInstall, 3);
            this.btnOBSInstall.Name = "btnOBSInstall";
            this.btnOBSInstall.UseVisualStyleBackColor = true;
            this.btnOBSInstall.Click += new System.EventHandler(this.btnOBSInstall_Click);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 4);
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
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
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // chkDeactivateForOtherPrograms
            // 
            resources.ApplyResources(this.chkDeactivateForOtherPrograms, "chkDeactivateForOtherPrograms");
            this.tableLayoutPanel2.SetColumnSpan(this.chkDeactivateForOtherPrograms, 2);
            this.chkDeactivateForOtherPrograms.Name = "chkDeactivateForOtherPrograms";
            this.chkDeactivateForOtherPrograms.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // chkGlobalHotkeys
            // 
            resources.ApplyResources(this.chkGlobalHotkeys, "chkGlobalHotkeys");
            this.chkGlobalHotkeys.Name = "chkGlobalHotkeys";
            this.chkGlobalHotkeys.UseVisualStyleBackColor = true;
            this.chkGlobalHotkeys.CheckedChanged += new System.EventHandler(this.chkGlobalHotkeys_CheckedChanged);
            // 
            // chkDoubleTap
            // 
            resources.ApplyResources(this.chkDoubleTap, "chkDoubleTap");
            this.chkDoubleTap.Name = "chkDoubleTap";
            this.chkDoubleTap.UseVisualStyleBackColor = true;
            // 
            // txtStartSplit
            // 
            resources.ApplyResources(this.txtStartSplit, "txtStartSplit");
            this.tableLayoutPanel2.SetColumnSpan(this.txtStartSplit, 2);
            this.txtStartSplit.Name = "txtStartSplit";
            this.txtStartSplit.ReadOnly = true;
            this.txtStartSplit.Enter += new System.EventHandler(this.Split_Set_Enter);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // txtReset
            // 
            resources.ApplyResources(this.txtReset, "txtReset");
            this.tableLayoutPanel2.SetColumnSpan(this.txtReset, 2);
            this.txtReset.Name = "txtReset";
            this.txtReset.ReadOnly = true;
            this.txtReset.Enter += new System.EventHandler(this.Reset_Set_Enter);
            // 
            // txtPause
            // 
            resources.ApplyResources(this.txtPause, "txtPause");
            this.tableLayoutPanel2.SetColumnSpan(this.txtPause, 2);
            this.txtPause.Name = "txtPause";
            this.txtPause.ReadOnly = true;
            this.txtPause.Enter += new System.EventHandler(this.Pause_Set_Enter);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // txtSkip
            // 
            resources.ApplyResources(this.txtSkip, "txtSkip");
            this.tableLayoutPanel2.SetColumnSpan(this.txtSkip, 2);
            this.txtSkip.Name = "txtSkip";
            this.txtSkip.ReadOnly = true;
            this.txtSkip.Enter += new System.EventHandler(this.Skip_Set_Enter);
            // 
            // txtUndo
            // 
            resources.ApplyResources(this.txtUndo, "txtUndo");
            this.tableLayoutPanel2.SetColumnSpan(this.txtUndo, 2);
            this.txtUndo.Name = "txtUndo";
            this.txtUndo.ReadOnly = true;
            this.txtUndo.Enter += new System.EventHandler(this.Undo_Set_Enter);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // txtToggle
            // 
            resources.ApplyResources(this.txtToggle, "txtToggle");
            this.tableLayoutPanel2.SetColumnSpan(this.txtToggle, 2);
            this.txtToggle.Name = "txtToggle";
            this.txtToggle.ReadOnly = true;
            this.txtToggle.Enter += new System.EventHandler(this.Toggle_Set_Enter);
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // txtSwitchPrevious
            // 
            resources.ApplyResources(this.txtSwitchPrevious, "txtSwitchPrevious");
            this.tableLayoutPanel2.SetColumnSpan(this.txtSwitchPrevious, 2);
            this.txtSwitchPrevious.Name = "txtSwitchPrevious";
            this.txtSwitchPrevious.ReadOnly = true;
            this.txtSwitchPrevious.Enter += new System.EventHandler(this.Switch_Previous_Set_Enter);
            // 
            // txtSwitchNext
            // 
            resources.ApplyResources(this.txtSwitchNext, "txtSwitchNext");
            this.tableLayoutPanel2.SetColumnSpan(this.txtSwitchNext, 2);
            this.txtSwitchNext.Name = "txtSwitchNext";
            this.txtSwitchNext.ReadOnly = true;
            this.txtSwitchNext.Enter += new System.EventHandler(this.Switch_Next_Set_Enter);
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // txtDelay
            // 
            resources.ApplyResources(this.txtDelay, "txtDelay");
            this.txtDelay.Name = "txtDelay";
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.tableLayoutPanel1.SetColumnSpan(this.btnCancel, 2);
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cbxRaceViewer
            // 
            resources.ApplyResources(this.cbxRaceViewer, "cbxRaceViewer");
            this.tableLayoutPanel1.SetColumnSpan(this.cbxRaceViewer, 3);
            this.cbxRaceViewer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxRaceViewer.FormattingEnabled = true;
            this.cbxRaceViewer.Items.AddRange(new object[] {
            resources.GetString("cbxRaceViewer.Items"),
            resources.GetString("cbxRaceViewer.Items1"),
            resources.GetString("cbxRaceViewer.Items2"),
            resources.GetString("cbxRaceViewer.Items3")});
            this.cbxRaceViewer.Name = "cbxRaceViewer";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // btnChooseComparisons
            // 
            resources.ApplyResources(this.btnChooseComparisons, "btnChooseComparisons");
            this.tableLayoutPanel1.SetColumnSpan(this.btnChooseComparisons, 3);
            this.btnChooseComparisons.Name = "btnChooseComparisons";
            this.btnChooseComparisons.UseVisualStyleBackColor = true;
            this.btnChooseComparisons.Click += new System.EventHandler(this.btnChooseComparisons_Click);
            // 
            // chkSimpleSOB
            // 
            resources.ApplyResources(this.chkSimpleSOB, "chkSimpleSOB");
            this.chkSimpleSOB.Name = "chkSimpleSOB";
            this.chkSimpleSOB.UseVisualStyleBackColor = true;
            this.chkSimpleSOB.CheckedChanged += new System.EventHandler(this.chkSimpleSOB_CheckedChanged);
            // 
            // chkWarnOnReset
            // 
            resources.ApplyResources(this.chkWarnOnReset, "chkWarnOnReset");
            this.tableLayoutPanel1.SetColumnSpan(this.chkWarnOnReset, 3);
            this.chkWarnOnReset.Name = "chkWarnOnReset";
            this.chkWarnOnReset.UseVisualStyleBackColor = true;
            // 
            // SettingsDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SettingsDialog";
            this.Load += new System.EventHandler(this.SettingsDialog_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
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
        private System.Windows.Forms.Button btnOBSInstall;
        private System.Windows.Forms.Label label5;
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
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnChooseComparisons;
    }
}