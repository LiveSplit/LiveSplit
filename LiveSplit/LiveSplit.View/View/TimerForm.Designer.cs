namespace LiveSplit.View
{
    partial class TimerForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimerForm));
            this.RightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editSplitsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSplitsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSplitsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSplitsAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeSplitsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.controlMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.shareMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.racingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newRaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.editLayoutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openLayoutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveLayoutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveLayoutAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.settingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoSplitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skipSplitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoPausesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hotkeysMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.comparisonMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RightClickMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // RightClickMenu
            // 
            this.RightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editSplitsMenuItem,
            this.openSplitsMenuItem,
            this.saveSplitsMenuItem,
            this.saveSplitsAsMenuItem,
            this.closeSplitsMenuItem,
            this.toolStripSeparator5,
            this.controlMenuItem,
            this.comparisonMenuItem,
            this.toolStripSeparator1,
            this.shareMenuItem,
            this.racingMenuItem,
            this.toolStripSeparator3,
            this.editLayoutMenuItem,
            this.openLayoutMenuItem,
            this.saveLayoutMenuItem,
            this.saveLayoutAsMenuItem,
            this.toolStripSeparator2,
            this.settingsMenuItem,
            this.toolStripSeparator4,
            this.aboutMenuItem,
            this.exitMenuItem});
            this.RightClickMenu.Name = "RightClickMenu";
            this.RightClickMenu.Size = new System.Drawing.Size(167, 408);
            this.RightClickMenu.Opening += new System.ComponentModel.CancelEventHandler(this.RightClickMenu_Opening);
            // 
            // editSplitsMenuItem
            // 
            this.editSplitsMenuItem.Name = "editSplitsMenuItem";
            this.editSplitsMenuItem.Size = new System.Drawing.Size(166, 22);
            this.editSplitsMenuItem.Text = "Edit Splits...";
            this.editSplitsMenuItem.Click += new System.EventHandler(this.editSplitsMenuItem_Click);
            // 
            // openSplitsMenuItem
            // 
            this.openSplitsMenuItem.Name = "openSplitsMenuItem";
            this.openSplitsMenuItem.Size = new System.Drawing.Size(166, 22);
            this.openSplitsMenuItem.Text = "Open Splits";
            // 
            // saveSplitsMenuItem
            // 
            this.saveSplitsMenuItem.Name = "saveSplitsMenuItem";
            this.saveSplitsMenuItem.Size = new System.Drawing.Size(166, 22);
            this.saveSplitsMenuItem.Text = "Save Splits";
            this.saveSplitsMenuItem.Click += new System.EventHandler(this.saveSplitsMenuItem_Click);
            // 
            // saveSplitsAsMenuItem
            // 
            this.saveSplitsAsMenuItem.Name = "saveSplitsAsMenuItem";
            this.saveSplitsAsMenuItem.Size = new System.Drawing.Size(166, 22);
            this.saveSplitsAsMenuItem.Text = "Save Splits As...";
            this.saveSplitsAsMenuItem.Click += new System.EventHandler(this.saveAsMenuItem_Click);
            // 
            // closeSplitsMenuItem
            // 
            this.closeSplitsMenuItem.Name = "closeSplitsMenuItem";
            this.closeSplitsMenuItem.Size = new System.Drawing.Size(166, 22);
            this.closeSplitsMenuItem.Text = "Close Splits";
            this.closeSplitsMenuItem.Click += new System.EventHandler(this.closeSplitsMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(163, 6);
            // 
            // controlMenuItem
            // 
            this.controlMenuItem.Name = "controlMenuItem";
            this.controlMenuItem.Size = new System.Drawing.Size(166, 22);
            this.controlMenuItem.Text = "Control";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(163, 6);
            // 
            // shareMenuItem
            // 
            this.shareMenuItem.Name = "shareMenuItem";
            this.shareMenuItem.Size = new System.Drawing.Size(166, 22);
            this.shareMenuItem.Text = "Share...";
            this.shareMenuItem.Click += new System.EventHandler(this.shareMenuItem_Click);
            // 
            // racingMenuItem
            // 
            this.racingMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newRaceToolStripMenuItem});
            this.racingMenuItem.Name = "racingMenuItem";
            this.racingMenuItem.Size = new System.Drawing.Size(166, 22);
            this.racingMenuItem.Text = "Races";
            this.racingMenuItem.MouseHover += new System.EventHandler(this.racingMenuItem_MouseHover);
            // 
            // newRaceToolStripMenuItem
            // 
            this.newRaceToolStripMenuItem.Name = "newRaceToolStripMenuItem";
            this.newRaceToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.newRaceToolStripMenuItem.Text = "New Race...";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(163, 6);
            // 
            // editLayoutMenuItem
            // 
            this.editLayoutMenuItem.Name = "editLayoutMenuItem";
            this.editLayoutMenuItem.Size = new System.Drawing.Size(166, 22);
            this.editLayoutMenuItem.Text = "Edit Layout...";
            this.editLayoutMenuItem.Click += new System.EventHandler(this.editLayoutMenuItem_Click);
            // 
            // openLayoutMenuItem
            // 
            this.openLayoutMenuItem.Name = "openLayoutMenuItem";
            this.openLayoutMenuItem.Size = new System.Drawing.Size(166, 22);
            this.openLayoutMenuItem.Text = "Open Layout";
            // 
            // saveLayoutMenuItem
            // 
            this.saveLayoutMenuItem.Name = "saveLayoutMenuItem";
            this.saveLayoutMenuItem.Size = new System.Drawing.Size(166, 22);
            this.saveLayoutMenuItem.Text = "Save Layout";
            this.saveLayoutMenuItem.Click += new System.EventHandler(this.saveLayoutMenuItem_Click);
            // 
            // saveLayoutAsMenuItem
            // 
            this.saveLayoutAsMenuItem.Name = "saveLayoutAsMenuItem";
            this.saveLayoutAsMenuItem.Size = new System.Drawing.Size(166, 22);
            this.saveLayoutAsMenuItem.Text = "Save Layout As...";
            this.saveLayoutAsMenuItem.Click += new System.EventHandler(this.saveLayoutAsMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(163, 6);
            // 
            // settingsMenuItem
            // 
            this.settingsMenuItem.Name = "settingsMenuItem";
            this.settingsMenuItem.Size = new System.Drawing.Size(166, 22);
            this.settingsMenuItem.Text = "Settings";
            this.settingsMenuItem.Click += new System.EventHandler(this.settingsMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(163, 6);
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Size = new System.Drawing.Size(166, 22);
            this.aboutMenuItem.Text = "About";
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(166, 22);
            this.exitMenuItem.Text = "Exit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // splitMenuItem
            // 
            this.splitMenuItem.Name = "splitMenuItem";
            this.splitMenuItem.Size = new System.Drawing.Size(152, 22);
            this.splitMenuItem.Text = "Start";
            this.splitMenuItem.Click += new System.EventHandler(this.splitMenuItem_Click);
            // 
            // resetMenuItem
            // 
            this.resetMenuItem.Enabled = false;
            this.resetMenuItem.Name = "resetMenuItem";
            this.resetMenuItem.Size = new System.Drawing.Size(152, 22);
            this.resetMenuItem.Text = "Reset";
            this.resetMenuItem.Click += new System.EventHandler(this.resetMenuItem_Click);
            // 
            // undoSplitMenuItem
            // 
            this.undoSplitMenuItem.Enabled = false;
            this.undoSplitMenuItem.Name = "undoSplitMenuItem";
            this.undoSplitMenuItem.Size = new System.Drawing.Size(152, 22);
            this.undoSplitMenuItem.Text = "Undo Split";
            this.undoSplitMenuItem.Click += new System.EventHandler(this.undoSplitMenuItem_Click);
            // 
            // skipSplitMenuItem
            // 
            this.skipSplitMenuItem.Enabled = false;
            this.skipSplitMenuItem.Name = "skipSplitMenuItem";
            this.skipSplitMenuItem.Size = new System.Drawing.Size(152, 22);
            this.skipSplitMenuItem.Text = "Skip Split";
            this.skipSplitMenuItem.Click += new System.EventHandler(this.skipSplitMenuItem_Click);
            // 
            // pauseMenuItem
            // 
            this.pauseMenuItem.Enabled = false;
            this.pauseMenuItem.Name = "pauseMenuItem";
            this.pauseMenuItem.Size = new System.Drawing.Size(152, 22);
            this.pauseMenuItem.Text = "Pause";
            this.pauseMenuItem.Click += new System.EventHandler(this.pauseMenuItem_Click);
            // 
            // undoPausesMenuItem
            // 
            this.undoPausesMenuItem.Enabled = false;
            this.undoPausesMenuItem.Name = "undoPausesMenuItem";
            this.undoPausesMenuItem.Size = new System.Drawing.Size(152, 22);
            this.undoPausesMenuItem.Text = "Undo All Pauses";
            this.undoPausesMenuItem.Click += new System.EventHandler(this.undoPausesMenuItem_Click);
            // 
            // hotkeysMenuItem
            // 
            this.hotkeysMenuItem.Enabled = true;
            this.hotkeysMenuItem.Name = "hotkeysMenuItem";
            this.hotkeysMenuItem.Size = new System.Drawing.Size(152, 22);
            this.hotkeysMenuItem.Text = "Global Hotkeys";
            this.hotkeysMenuItem.Click += new System.EventHandler(this.hotkeysMenuItem_Click);
            // 
            // comparisonMenuItem
            // 
            this.comparisonMenuItem.Name = "comparisonMenuItem";
            this.comparisonMenuItem.Size = new System.Drawing.Size(166, 22);
            this.comparisonMenuItem.Text = "Compare Against";
            // 
            // TimerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(150, 150);
            this.ContextMenuStrip = this.RightClickMenu;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TimerForm";
            this.Text = "LiveSplit";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TimerForm_FormClosing);
            this.Shown += new System.EventHandler(this.TimerForm_Shown);
            this.ResizeBegin += new System.EventHandler(this.TimerForm_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.TimerForm_ResizeEnd);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.TimerForm_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TimerForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TimerForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TimerForm_MouseUp);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.TimerForm_MouseWheel);
            this.RightClickMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip RightClickMenu;
        private System.Windows.Forms.ToolStripMenuItem openSplitsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editSplitsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSplitsMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem openLayoutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveLayoutAsMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem settingsMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeSplitsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSplitsAsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editLayoutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveLayoutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shareMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem racingMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newRaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem controlMenuItem;
        private System.Windows.Forms.ToolStripMenuItem splitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoSplitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem skipSplitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pauseMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoPausesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hotkeysMenuItem;
        private System.Windows.Forms.ToolStripMenuItem comparisonMenuItem;

    }
}

