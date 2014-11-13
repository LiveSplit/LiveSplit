using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LiveSplit.View
{
    public partial class AuthenticationDialog : Form
    {
        public String Username { get; set; }
        public String Password { get; set; }
        public bool RememberPassword { get; set; }

        public AuthenticationDialog()
        {
            InitializeComponent();
            this.Load += AuthenticationDialog_Load;
        }

        void AuthenticationDialog_Load(object sender, EventArgs e)
        {
            tbxUsername.Text = Username;
            tbxPassword.Text = Password;
            chkRememberPassword.Checked = RememberPassword;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Username = tbxUsername.Text;
            Password = tbxPassword.Text;
            RememberPassword = chkRememberPassword.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
