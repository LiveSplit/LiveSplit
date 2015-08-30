namespace LiveSplit.View
{
    partial class MetadataControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MetadataControl));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cmbPlatform = new System.Windows.Forms.ComboBox();
            this.lblRegion = new System.Windows.Forms.Label();
            this.lblPlatform = new System.Windows.Forms.Label();
            this.cmbRegion = new System.Windows.Forms.ComboBox();
            this.lblRules = new System.Windows.Forms.Label();
            this.btnAssociate = new System.Windows.Forms.Button();
            this.tbxRules = new System.Windows.Forms.RichTextBox();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.cmbPlatform, 4, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblRegion, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblPlatform, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.cmbRegion, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblRules, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnAssociate, 3, 8);
            this.tableLayoutPanel1.Controls.Add(this.tbxRules, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnSubmit, 0, 8);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // cmbPlatform
            // 
            resources.ApplyResources(this.cmbPlatform, "cmbPlatform");
            this.cmbPlatform.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPlatform.FormattingEnabled = true;
            this.cmbPlatform.Name = "cmbPlatform";
            // 
            // lblRegion
            // 
            resources.ApplyResources(this.lblRegion, "lblRegion");
            this.lblRegion.Name = "lblRegion";
            // 
            // lblPlatform
            // 
            resources.ApplyResources(this.lblPlatform, "lblPlatform");
            this.lblPlatform.Name = "lblPlatform";
            // 
            // cmbRegion
            // 
            resources.ApplyResources(this.cmbRegion, "cmbRegion");
            this.cmbRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRegion.FormattingEnabled = true;
            this.cmbRegion.Name = "cmbRegion";
            // 
            // lblRules
            // 
            resources.ApplyResources(this.lblRules, "lblRules");
            this.lblRules.Name = "lblRules";
            // 
            // btnAssociate
            // 
            resources.ApplyResources(this.btnAssociate, "btnAssociate");
            this.tableLayoutPanel1.SetColumnSpan(this.btnAssociate, 2);
            this.btnAssociate.Name = "btnAssociate";
            this.btnAssociate.UseVisualStyleBackColor = true;
            this.btnAssociate.Click += new System.EventHandler(this.btnAssociate_Click);
            // 
            // tbxRules
            // 
            resources.ApplyResources(this.tbxRules, "tbxRules");
            this.tbxRules.BackColor = System.Drawing.SystemColors.Window;
            this.tableLayoutPanel1.SetColumnSpan(this.tbxRules, 5);
            this.tbxRules.Name = "tbxRules";
            this.tbxRules.ReadOnly = true;
            this.tbxRules.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.tbxRules_LinkClicked);
            // 
            // btnSubmit
            // 
            resources.ApplyResources(this.btnSubmit, "btnSubmit");
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // MetadataControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "MetadataControl";
            this.Load += new System.EventHandler(this.MetadataControl_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox cmbPlatform;
        private System.Windows.Forms.Label lblRegion;
        private System.Windows.Forms.Label lblPlatform;
        private System.Windows.Forms.ComboBox cmbRegion;
        private System.Windows.Forms.Label lblRules;
        private System.Windows.Forms.Button btnAssociate;
        private System.Windows.Forms.RichTextBox tbxRules;
        private System.Windows.Forms.Button btnSubmit;
    }
}
