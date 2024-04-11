using System;
using System.Windows.Forms;

namespace LiveSplit.View
{
    public partial class AuthenticationDialog : Form
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberPassword { get; set; }

        public AuthenticationDialog()
        {
            InitializeComponent();
            Load += AuthenticationDialog_Load;
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

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
