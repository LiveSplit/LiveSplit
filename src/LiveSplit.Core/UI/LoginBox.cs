using System;
using System.Drawing;
using System.Windows.Forms;

namespace LiveSplit.UI;

public static class LoginBox
{

    public static DialogResult Show(string title, string usernamePrompt, string passwordPrompt, ref string username, ref string password, ref bool remember)
    {
        return Show(null, title, usernamePrompt, passwordPrompt, ref username, ref password, ref remember);
    }

    public static DialogResult Show(IWin32Window owner, string title, string usernamePrompt, string passwordPrompt, ref string username, ref string password, ref bool remember)
    {
        using var form = new Form();
        var usernameLabel = new Label();
        var passwordLabel = new Label();
        var usernameBox = new TextBox();
        var passwordBox = new TextBox();
        var rememberCheckBox = new CheckBox();
        var buttonOk = new Button();
        var buttonCancel = new Button();

        form.Text = title;
        usernameLabel.Text = usernamePrompt;
        passwordLabel.Text = passwordPrompt;
        usernameBox.Text = username;
        passwordBox.Text = password;
        passwordBox.UseSystemPasswordChar = true;
        rememberCheckBox.Text = "Remember Password:";
        rememberCheckBox.Checked = true;
        rememberCheckBox.CheckAlign = ContentAlignment.MiddleRight;
        rememberCheckBox.TextAlign = ContentAlignment.MiddleLeft;

        buttonOk.Text = "OK";
        buttonCancel.Text = "Cancel";
        buttonOk.DialogResult = DialogResult.OK;
        buttonCancel.DialogResult = DialogResult.Cancel;

        usernameLabel.SetBounds(9, 20, 422, 13);
        passwordLabel.SetBounds(9, 66, 422, 13);
        rememberCheckBox.SetBounds(9, 110, 422, 13);
        usernameBox.SetBounds(12, 36, 422, 20);
        passwordBox.SetBounds(12, 82, 422, 20);
        buttonOk.SetBounds(278, 140, 75, 23);
        buttonCancel.SetBounds(359, 140, 75, 23);

        usernameLabel.AutoSize = true;
        passwordLabel.AutoSize = true;
        rememberCheckBox.AutoSize = true;
        usernameBox.Anchor |= AnchorStyles.Right;
        passwordBox.Anchor = usernameBox.Anchor | AnchorStyles.Right;
        rememberCheckBox.Anchor = usernameBox.Anchor | AnchorStyles.Right;
        buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

        form.ClientSize = new Size(446, 174);
        form.Controls.AddRange([usernameLabel, passwordLabel, usernameBox, passwordBox, rememberCheckBox, buttonOk, buttonCancel]);
        form.ClientSize = new Size(Math.Max(350, usernameLabel.Right + 10), form.ClientSize.Height);
        form.FormBorderStyle = FormBorderStyle.FixedDialog;
        form.StartPosition = FormStartPosition.CenterScreen;
        form.MinimizeBox = false;
        form.MaximizeBox = false;
        form.AcceptButton = buttonOk;
        form.CancelButton = buttonCancel;

        DialogResult dialogResult = owner != null ? form.ShowDialog(owner) : form.ShowDialog();
        username = usernameBox.Text;
        password = passwordBox.Text;
        remember = rememberCheckBox.Checked;
        return dialogResult;
    }
}
