namespace LiveSplit.View
{
    partial class ShareRunDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShareRunDialog));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cbxCategory = new System.Windows.Forms.ComboBox();
            this.cbxGame = new System.Windows.Forms.ComboBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtVersion = new System.Windows.Forms.TextBox();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.txtVideoURL = new System.Windows.Forms.TextBox();
            this.cbxPlatform = new System.Windows.Forms.ComboBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblDescription = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnInsertGame = new System.Windows.Forms.Button();
            this.btnInsertCategory = new System.Windows.Forms.Button();
            this.btnInsertTitle = new System.Windows.Forms.Button();
            this.btnInsertPB = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnInsertSplitName = new System.Windows.Forms.Button();
            this.btnInsertDeltaTime = new System.Windows.Forms.Button();
            this.btnInsertSplitTime = new System.Windows.Forms.Button();
            this.btnInsertStreamLink = new System.Windows.Forms.Button();
            this.chkAttachSplits = new System.Windows.Forms.CheckBox();
            this.btnShare = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.cbxCategory, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.cbxGame, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.txtPassword, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.txtVersion, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.txtUser, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.txtNotes, 1, 8);
            this.tableLayoutPanel1.Controls.Add(this.txtVideoURL, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.cbxPlatform, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnClose, 5, 12);
            this.tableLayoutPanel1.Controls.Add(this.lblDescription, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label9, 0, 10);
            this.tableLayoutPanel1.Controls.Add(this.btnInsertGame, 1, 10);
            this.tableLayoutPanel1.Controls.Add(this.btnInsertCategory, 2, 10);
            this.tableLayoutPanel1.Controls.Add(this.btnInsertTitle, 3, 10);
            this.tableLayoutPanel1.Controls.Add(this.btnInsertPB, 4, 10);
            this.tableLayoutPanel1.Controls.Add(this.btnPreview, 5, 10);
            this.tableLayoutPanel1.Controls.Add(this.btnInsertSplitName, 1, 11);
            this.tableLayoutPanel1.Controls.Add(this.btnInsertDeltaTime, 2, 11);
            this.tableLayoutPanel1.Controls.Add(this.btnInsertSplitTime, 3, 11);
            this.tableLayoutPanel1.Controls.Add(this.btnInsertStreamLink, 4, 11);
            this.tableLayoutPanel1.Controls.Add(this.chkAttachSplits, 0, 12);
            this.tableLayoutPanel1.Controls.Add(this.btnShare, 2, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // cbxCategory
            // 
            resources.ApplyResources(this.cbxCategory, "cbxCategory");
            this.tableLayoutPanel1.SetColumnSpan(this.cbxCategory, 5);
            this.cbxCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxCategory.FormattingEnabled = true;
            this.cbxCategory.Items.AddRange(new object[] {
            resources.GetString("cbxCategory.Items")});
            this.cbxCategory.Name = "cbxCategory";
            // 
            // cbxGame
            // 
            resources.ApplyResources(this.cbxGame, "cbxGame");
            this.tableLayoutPanel1.SetColumnSpan(this.cbxGame, 5);
            this.cbxGame.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxGame.FormattingEnabled = true;
            this.cbxGame.Items.AddRange(new object[] {
            resources.GetString("cbxGame.Items")});
            this.cbxGame.Name = "cbxGame";
            this.cbxGame.SelectionChangeCommitted += new System.EventHandler(this.cbxGame_SelectionChangeCommitted);
            // 
            // txtPassword
            // 
            resources.ApplyResources(this.txtPassword, "txtPassword");
            this.tableLayoutPanel1.SetColumnSpan(this.txtPassword, 5);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // txtVersion
            // 
            resources.ApplyResources(this.txtVersion, "txtVersion");
            this.tableLayoutPanel1.SetColumnSpan(this.txtVersion, 5);
            this.txtVersion.Name = "txtVersion";
            // 
            // txtUser
            // 
            resources.ApplyResources(this.txtUser, "txtUser");
            this.tableLayoutPanel1.SetColumnSpan(this.txtUser, 5);
            this.txtUser.Name = "txtUser";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
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
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // txtNotes
            // 
            this.txtNotes.AcceptsReturn = true;
            resources.ApplyResources(this.txtNotes, "txtNotes");
            this.tableLayoutPanel1.SetColumnSpan(this.txtNotes, 5);
            this.txtNotes.Name = "txtNotes";
            this.tableLayoutPanel1.SetRowSpan(this.txtNotes, 2);
            // 
            // txtVideoURL
            // 
            resources.ApplyResources(this.txtVideoURL, "txtVideoURL");
            this.txtVideoURL.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtVideoURL.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
            this.tableLayoutPanel1.SetColumnSpan(this.txtVideoURL, 5);
            this.txtVideoURL.Name = "txtVideoURL";
            // 
            // cbxPlatform
            // 
            resources.ApplyResources(this.cbxPlatform, "cbxPlatform");
            this.tableLayoutPanel1.SetColumnSpan(this.cbxPlatform, 5);
            this.cbxPlatform.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPlatform.FormattingEnabled = true;
            this.cbxPlatform.Name = "cbxPlatform";
            this.cbxPlatform.SelectionChangeCommitted += new System.EventHandler(this.cbxPlatform_SelectionChangeCommitted);
            // 
            // btnClose
            // 
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblDescription
            // 
            resources.ApplyResources(this.lblDescription, "lblDescription");
            this.tableLayoutPanel1.SetColumnSpan(this.lblDescription, 5);
            this.lblDescription.Name = "lblDescription";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // btnInsertGame
            // 
            resources.ApplyResources(this.btnInsertGame, "btnInsertGame");
            this.btnInsertGame.Name = "btnInsertGame";
            this.btnInsertGame.UseVisualStyleBackColor = true;
            this.btnInsertGame.Click += new System.EventHandler(this.btnInsertGame_Click);
            // 
            // btnInsertCategory
            // 
            resources.ApplyResources(this.btnInsertCategory, "btnInsertCategory");
            this.btnInsertCategory.Name = "btnInsertCategory";
            this.btnInsertCategory.UseVisualStyleBackColor = true;
            this.btnInsertCategory.Click += new System.EventHandler(this.btnInsertCategory_Click);
            // 
            // btnInsertTitle
            // 
            resources.ApplyResources(this.btnInsertTitle, "btnInsertTitle");
            this.btnInsertTitle.Name = "btnInsertTitle";
            this.btnInsertTitle.UseVisualStyleBackColor = true;
            this.btnInsertTitle.Click += new System.EventHandler(this.btnInsertTitle_Click);
            // 
            // btnInsertPB
            // 
            resources.ApplyResources(this.btnInsertPB, "btnInsertPB");
            this.btnInsertPB.Name = "btnInsertPB";
            this.btnInsertPB.UseVisualStyleBackColor = true;
            this.btnInsertPB.Click += new System.EventHandler(this.btnInsertPB_Click);
            // 
            // btnPreview
            // 
            resources.ApplyResources(this.btnPreview, "btnPreview");
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnInsertSplitName
            // 
            resources.ApplyResources(this.btnInsertSplitName, "btnInsertSplitName");
            this.btnInsertSplitName.Name = "btnInsertSplitName";
            this.btnInsertSplitName.UseVisualStyleBackColor = true;
            this.btnInsertSplitName.Click += new System.EventHandler(this.btnInsertSplitName_Click);
            // 
            // btnInsertDeltaTime
            // 
            resources.ApplyResources(this.btnInsertDeltaTime, "btnInsertDeltaTime");
            this.btnInsertDeltaTime.Name = "btnInsertDeltaTime";
            this.btnInsertDeltaTime.UseVisualStyleBackColor = true;
            this.btnInsertDeltaTime.Click += new System.EventHandler(this.btnInsertDeltaTime_Click);
            // 
            // btnInsertSplitTime
            // 
            resources.ApplyResources(this.btnInsertSplitTime, "btnInsertSplitTime");
            this.btnInsertSplitTime.Name = "btnInsertSplitTime";
            this.btnInsertSplitTime.UseVisualStyleBackColor = true;
            this.btnInsertSplitTime.Click += new System.EventHandler(this.btnInsertSplitTime_Click);
            // 
            // btnInsertStreamLink
            // 
            resources.ApplyResources(this.btnInsertStreamLink, "btnInsertStreamLink");
            this.btnInsertStreamLink.Name = "btnInsertStreamLink";
            this.btnInsertStreamLink.UseVisualStyleBackColor = true;
            this.btnInsertStreamLink.Click += new System.EventHandler(this.btnInsertStreamLink_Click);
            // 
            // chkAttachSplits
            // 
            resources.ApplyResources(this.chkAttachSplits, "chkAttachSplits");
            this.tableLayoutPanel1.SetColumnSpan(this.chkAttachSplits, 2);
            this.chkAttachSplits.Name = "chkAttachSplits";
            this.chkAttachSplits.UseVisualStyleBackColor = true;
            // 
            // btnShare
            // 
            resources.ApplyResources(this.btnShare, "btnShare");
            this.tableLayoutPanel1.SetColumnSpan(this.btnShare, 3);
            this.btnShare.Name = "btnShare";
            this.btnShare.UseVisualStyleBackColor = true;
            this.btnShare.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // ShareRunDialog
            // 
            this.AcceptButton = this.btnShare;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ShareRunDialog";
            this.Load += new System.EventHandler(this.SubmitDialog_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtVersion;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.TextBox txtVideoURL;
        private System.Windows.Forms.ComboBox cbxPlatform;
        private System.Windows.Forms.ComboBox cbxCategory;
        private System.Windows.Forms.ComboBox cbxGame;
        private System.Windows.Forms.Button btnShare;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnInsertGame;
        private System.Windows.Forms.Button btnInsertCategory;
        private System.Windows.Forms.Button btnInsertTitle;
        private System.Windows.Forms.Button btnInsertPB;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnInsertDeltaTime;
        private System.Windows.Forms.Button btnInsertSplitTime;
        private System.Windows.Forms.Button btnInsertSplitName;
        private System.Windows.Forms.Button btnInsertStreamLink;
        private System.Windows.Forms.CheckBox chkAttachSplits;
    }
}