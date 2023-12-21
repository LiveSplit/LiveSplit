using System;
using System.Windows.Forms;
using LiveSplit.Options;

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
            RefreshText();
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

        private void RefreshText ()
        {
            btnCancel.Text = Languages.Instance.GetText("btnCancel", "Cancel");
            btnOK.Text = Languages.Instance.GetText("btnOK", "OK");
            label2.Text = Languages.Instance.GetText("Password", "Password:");
            label1.Text = Languages.Instance.GetText("Username", "Username:");
            chkRememberPassword.Text = Languages.Instance.GetText("chkRememberPassword", "Remember Password");
            Text = Languages.Instance.GetText("AuthenticationDialog", "Authentication");
        }
    }
}
