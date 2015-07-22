namespace LiveSplit.View
{
    partial class LayoutEditorDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayoutEditorDialog));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnLayoutSettings = new System.Windows.Forms.Button();
            this.lbxComponents = new System.Windows.Forms.ListBox();
            this.rdoHorizontal = new System.Windows.Forms.RadioButton();
            this.rdoVertical = new System.Windows.Forms.RadioButton();
            this.btnSetSize = new System.Windows.Forms.Button();
            this.menuAddComponents = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.btnAdd, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnRemove, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnMoveUp, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnMoveDown, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 6, 6);
            this.tableLayoutPanel1.Controls.Add(this.btnOK, 5, 6);
            this.tableLayoutPanel1.Controls.Add(this.btnLayoutSettings, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.lbxComponents, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.rdoHorizontal, 3, 6);
            this.tableLayoutPanel1.Controls.Add(this.rdoVertical, 4, 6);
            this.tableLayoutPanel1.Controls.Add(this.btnSetSize, 2, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // btnAdd
            // 
            resources.ApplyResources(this.btnAdd, "btnAdd");
            this.btnAdd.BackgroundImage = global::LiveSplit.Properties.Resources.Add;
            this.btnAdd.ForeColor = System.Drawing.SystemColors.Control;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            resources.ApplyResources(this.btnRemove, "btnRemove");
            this.btnRemove.BackgroundImage = global::LiveSplit.Properties.Resources.Remove;
            this.btnRemove.ForeColor = System.Drawing.SystemColors.Control;
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnMoveUp
            // 
            resources.ApplyResources(this.btnMoveUp, "btnMoveUp");
            this.btnMoveUp.ForeColor = System.Drawing.SystemColors.Control;
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnMoveDown
            // 
            resources.ApplyResources(this.btnMoveDown, "btnMoveDown");
            this.btnMoveDown.ForeColor = System.Drawing.SystemColors.Control;
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
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
            // btnLayoutSettings
            // 
            resources.ApplyResources(this.btnLayoutSettings, "btnLayoutSettings");
            this.btnLayoutSettings.Name = "btnLayoutSettings";
            this.btnLayoutSettings.UseVisualStyleBackColor = true;
            this.btnLayoutSettings.Click += new System.EventHandler(this.btnLayoutSettings_Click);
            // 
            // lbxComponents
            // 
            resources.ApplyResources(this.lbxComponents, "lbxComponents");
            this.tableLayoutPanel1.SetColumnSpan(this.lbxComponents, 6);
            this.lbxComponents.FormattingEnabled = true;
            this.lbxComponents.Name = "lbxComponents";
            this.tableLayoutPanel1.SetRowSpan(this.lbxComponents, 6);
            this.lbxComponents.DoubleClick += new System.EventHandler(this.lbxComponents_DoubleClick);
            this.lbxComponents.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbxComponents_MouseDoubleClick);
            // 
            // rdoHorizontal
            // 
            resources.ApplyResources(this.rdoHorizontal, "rdoHorizontal");
            this.rdoHorizontal.Name = "rdoHorizontal";
            this.rdoHorizontal.TabStop = true;
            this.rdoHorizontal.UseVisualStyleBackColor = true;
            // 
            // rdoVertical
            // 
            resources.ApplyResources(this.rdoVertical, "rdoVertical");
            this.rdoVertical.Name = "rdoVertical";
            this.rdoVertical.UseVisualStyleBackColor = true;
            // 
            // btnSetSize
            // 
            resources.ApplyResources(this.btnSetSize, "btnSetSize");
            this.btnSetSize.Name = "btnSetSize";
            this.btnSetSize.UseVisualStyleBackColor = true;
            this.btnSetSize.Click += new System.EventHandler(this.btnSetSize_Click);
            // 
            // menuAddComponents
            // 
            resources.ApplyResources(this.menuAddComponents, "menuAddComponents");
            this.menuAddComponents.Name = "menuAddComponents";
            // 
            // LayoutEditorDialog
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "LayoutEditorDialog";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.ListBox lbxComponents;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ContextMenuStrip menuAddComponents;
        private System.Windows.Forms.Button btnLayoutSettings;
        private System.Windows.Forms.RadioButton rdoHorizontal;
        private System.Windows.Forms.RadioButton rdoVertical;
        private System.Windows.Forms.Button btnSetSize;
    }
}