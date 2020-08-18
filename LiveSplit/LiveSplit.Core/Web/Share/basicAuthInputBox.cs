using System;
using System.Drawing;
using System.Windows.Forms;

namespace LiveSplit.UI
{
    public static class BasicAuthInputBox
    {
        public static DialogResult Show(string title, string promptText, string promptText2, ref string value, ref string value2)
        {
            return Show(null, title, promptText, promptText2, ref value, ref value2);
        }

        public static DialogResult Show(IWin32Window owner, string title, string promptText, string promptText2, ref string value, ref string value2)
        {
            using (Form form = new Form())
            {
                Label unameLabel = new Label();
                Label pwLabel = new Label();
                TextBox unameBox = new TextBox();
                TextBox pwBox = new TextBox();
                Button buttonOk = new Button();
                Button buttonCancel = new Button();

                form.Text = title;
                unameLabel.Text = promptText;
                pwLabel.Text = promptText2;
                unameBox.Text = value;
                pwBox.Text = value2;

                buttonOk.Text = "OK";
                buttonCancel.Text = "Cancel";
                buttonOk.DialogResult = DialogResult.OK;
                buttonCancel.DialogResult = DialogResult.Cancel;

                unameLabel.SetBounds(9, 20, 372, 13);
                pwLabel.SetBounds(9, 66, 372, 13);
                unameBox.SetBounds(12, 36, 372, 20);
                pwBox.SetBounds(12, 82, 372, 20);
                buttonOk.SetBounds(228, 118, 75, 23);
                buttonCancel.SetBounds(309, 118, 75, 23);

                unameLabel.AutoSize = true;
                pwLabel.AutoSize = true;
                unameBox.Anchor = unameBox.Anchor | AnchorStyles.Right;
                pwBox.Anchor = unameBox.Anchor | AnchorStyles.Right;
                buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

                pwBox.PasswordChar = '*';

                form.ClientSize = new Size(396, 153);
                form.Controls.AddRange(new Control[] { unameLabel, pwLabel, unameBox, pwBox, buttonOk, buttonCancel });
                form.ClientSize = new Size(Math.Max(300, unameLabel.Right + 10), form.ClientSize.Height);
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MinimizeBox = false;
                form.MaximizeBox = false;
                form.AcceptButton = buttonOk;
                form.CancelButton = buttonCancel;

                DialogResult dialogResult = owner != null ? form.ShowDialog(owner) : form.ShowDialog();
                value = unameBox.Text;
                value2 = pwBox.Text;
                return dialogResult;
            }
        }
    }
}
