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
            this.comparisonMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.hotkeysMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            resources.ApplyResources(this.RightClickMenu, "RightClickMenu");
            this.RightClickMenu.Opening += new System.ComponentModel.CancelEventHandler(this.RightClickMenu_Opening);
            // 
            // editSplitsMenuItem
            // 
            this.editSplitsMenuItem.Name = "editSplitsMenuItem";
            resources.ApplyResources(this.editSplitsMenuItem, "editSplitsMenuItem");
            this.editSplitsMenuItem.Click += new System.EventHandler(this.editSplitsMenuItem_Click);
            // 
            // openSplitsMenuItem
            // 
            this.openSplitsMenuItem.Name = "openSplitsMenuItem";
            resources.ApplyResources(this.openSplitsMenuItem, "openSplitsMenuItem");
            // 
            // saveSplitsMenuItem
            // 
            this.saveSplitsMenuItem.Name = "saveSplitsMenuItem";
            resources.ApplyResources(this.saveSplitsMenuItem, "saveSplitsMenuItem");
            this.saveSplitsMenuItem.Click += new System.EventHandler(this.saveSplitsMenuItem_Click);
            // 
            // saveSplitsAsMenuItem
            // 
            this.saveSplitsAsMenuItem.Name = "saveSplitsAsMenuItem";
            resources.ApplyResources(this.saveSplitsAsMenuItem, "saveSplitsAsMenuItem");
            this.saveSplitsAsMenuItem.Click += new System.EventHandler(this.saveAsMenuItem_Click);
            // 
            // closeSplitsMenuItem
            // 
            this.closeSplitsMenuItem.Name = "closeSplitsMenuItem";
            resources.ApplyResources(this.closeSplitsMenuItem, "closeSplitsMenuItem");
            this.closeSplitsMenuItem.Click += new System.EventHandler(this.closeSplitsMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // controlMenuItem
            // 
            this.controlMenuItem.Name = "controlMenuItem";
            resources.ApplyResources(this.controlMenuItem, "controlMenuItem");
            // 
            // comparisonMenuItem
            // 
            this.comparisonMenuItem.Name = "comparisonMenuItem";
            resources.ApplyResources(this.comparisonMenuItem, "comparisonMenuItem");
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // shareMenuItem
            // 
            this.shareMenuItem.Name = "shareMenuItem";
            resources.ApplyResources(this.shareMenuItem, "shareMenuItem");
            this.shareMenuItem.Click += new System.EventHandler(this.shareMenuItem_Click);
            // 
            // racingMenuItem
            // 
            this.racingMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newRaceToolStripMenuItem});
            this.racingMenuItem.Name = "racingMenuItem";
            resources.ApplyResources(this.racingMenuItem, "racingMenuItem");
            this.racingMenuItem.MouseHover += new System.EventHandler(this.racingMenuItem_MouseHover);
            // 
            // newRaceToolStripMenuItem
            // 
            this.newRaceToolStripMenuItem.Name = "newRaceToolStripMenuItem";
            resources.ApplyResources(this.newRaceToolStripMenuItem, "newRaceToolStripMenuItem");
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // editLayoutMenuItem
            // 
            this.editLayoutMenuItem.Name = "editLayoutMenuItem";
            resources.ApplyResources(this.editLayoutMenuItem, "editLayoutMenuItem");
            this.editLayoutMenuItem.Click += new System.EventHandler(this.editLayoutMenuItem_Click);
            // 
            // openLayoutMenuItem
            // 
            this.openLayoutMenuItem.Name = "openLayoutMenuItem";
            resources.ApplyResources(this.openLayoutMenuItem, "openLayoutMenuItem");
            // 
            // saveLayoutMenuItem
            // 
            this.saveLayoutMenuItem.Name = "saveLayoutMenuItem";
            resources.ApplyResources(this.saveLayoutMenuItem, "saveLayoutMenuItem");
            this.saveLayoutMenuItem.Click += new System.EventHandler(this.saveLayoutMenuItem_Click);
            // 
            // saveLayoutAsMenuItem
            // 
            this.saveLayoutAsMenuItem.Name = "saveLayoutAsMenuItem";
            resources.ApplyResources(this.saveLayoutAsMenuItem, "saveLayoutAsMenuItem");
            this.saveLayoutAsMenuItem.Click += new System.EventHandler(this.saveLayoutAsMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // settingsMenuItem
            // 
            this.settingsMenuItem.Name = "settingsMenuItem";
            resources.ApplyResources(this.settingsMenuItem, "settingsMenuItem");
            this.settingsMenuItem.Click += new System.EventHandler(this.settingsMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Name = "aboutMenuItem";
            resources.ApplyResources(this.aboutMenuItem, "aboutMenuItem");
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            resources.ApplyResources(this.exitMenuItem, "exitMenuItem");
            this.exitMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // splitMenuItem
            // 
            this.splitMenuItem.Name = "splitMenuItem";
            resources.ApplyResources(this.splitMenuItem, "splitMenuItem");
            this.splitMenuItem.Click += new System.EventHandler(this.splitMenuItem_Click);
            // 
            // resetMenuItem
            // 
            resources.ApplyResources(this.resetMenuItem, "resetMenuItem");
            this.resetMenuItem.Name = "resetMenuItem";
            this.resetMenuItem.Click += new System.EventHandler(this.resetMenuItem_Click);
            // 
            // undoSplitMenuItem
            // 
            resources.ApplyResources(this.undoSplitMenuItem, "undoSplitMenuItem");
            this.undoSplitMenuItem.Name = "undoSplitMenuItem";
            this.undoSplitMenuItem.Click += new System.EventHandler(this.undoSplitMenuItem_Click);
            // 
            // skipSplitMenuItem
            // 
            resources.ApplyResources(this.skipSplitMenuItem, "skipSplitMenuItem");
            this.skipSplitMenuItem.Name = "skipSplitMenuItem";
            this.skipSplitMenuItem.Click += new System.EventHandler(this.skipSplitMenuItem_Click);
            // 
            // pauseMenuItem
            // 
            resources.ApplyResources(this.pauseMenuItem, "pauseMenuItem");
            this.pauseMenuItem.Name = "pauseMenuItem";
            this.pauseMenuItem.Click += new System.EventHandler(this.pauseMenuItem_Click);
            // 
            // hotkeysMenuItem
            // 
            this.hotkeysMenuItem.Name = "hotkeysMenuItem";
            resources.ApplyResources(this.hotkeysMenuItem, "hotkeysMenuItem");
            this.hotkeysMenuItem.Click += new System.EventHandler(this.hotkeysMenuItem_Click);
            // 
            // TimerForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ContextMenuStrip = this.RightClickMenu;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "TimerForm";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TimerForm_FormClosing);
            this.Load += new System.EventHandler(this.TimerForm_Load);
            this.Shown += new System.EventHandler(this.TimerForm_Shown);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.TimerForm_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TimerForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TimerForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TimerForm_MouseUp);
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
        private System.Windows.Forms.ToolStripMenuItem hotkeysMenuItem;
        private System.Windows.Forms.ToolStripMenuItem comparisonMenuItem;

    }
}

