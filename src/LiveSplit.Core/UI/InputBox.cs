﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace LiveSplit.UI;

public static class InputBox
{
    public static DialogResult Show(string title, string promptText, ref string value)
    {
        return Show(null, title, promptText, ref value);
    }

    public static DialogResult Show(IWin32Window owner, string title, string promptText, ref string value)
    {
        using var form = new Form();
        var label = new Label();
        var textBox = new TextBox();
        var buttonOk = new Button();
        var buttonCancel = new Button();

        form.Text = title;
        label.Text = promptText;
        textBox.Text = value;

        buttonOk.Text = "OK";
        buttonCancel.Text = "Cancel";
        buttonOk.DialogResult = DialogResult.OK;
        buttonCancel.DialogResult = DialogResult.Cancel;

        label.SetBounds(9, 20, 372, 13);
        textBox.SetBounds(12, 36, 372, 20);
        buttonOk.SetBounds(228, 72, 75, 23);
        buttonCancel.SetBounds(309, 72, 75, 23);

        label.AutoSize = true;
        textBox.Anchor |= AnchorStyles.Right;
        buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

        form.ClientSize = new Size(396, 107);
        form.Controls.AddRange([label, textBox, buttonOk, buttonCancel]);
        form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
        form.FormBorderStyle = FormBorderStyle.FixedDialog;
        form.StartPosition = FormStartPosition.CenterScreen;
        form.MinimizeBox = false;
        form.MaximizeBox = false;
        form.AcceptButton = buttonOk;
        form.CancelButton = buttonCancel;

        DialogResult dialogResult = owner != null ? form.ShowDialog(owner) : form.ShowDialog();
        value = textBox.Text;
        return dialogResult;
    }

    public static DialogResult Show(string title, string promptText, string promptText2, ref string value, ref string value2)
    {
        return Show(null, title, promptText, promptText2, ref value, ref value2);
    }

    public static DialogResult Show(IWin32Window owner, string title, string promptText, string promptText2, ref string value, ref string value2)
    {
        using var form = new Form();
        var label = new Label();
        var label2 = new Label();
        var textBox = new TextBox();
        var textBox2 = new TextBox();
        var buttonOk = new Button();
        var buttonCancel = new Button();

        form.Text = title;
        label.Text = promptText;
        label2.Text = promptText2;
        textBox.Text = value;
        textBox2.Text = value2;

        buttonOk.Text = "OK";
        buttonCancel.Text = "Cancel";
        buttonOk.DialogResult = DialogResult.OK;
        buttonCancel.DialogResult = DialogResult.Cancel;

        label.SetBounds(9, 20, 372, 13);
        label2.SetBounds(9, 66, 372, 13);
        textBox.SetBounds(12, 36, 372, 20);
        textBox2.SetBounds(12, 82, 372, 20);
        buttonOk.SetBounds(228, 118, 75, 23);
        buttonCancel.SetBounds(309, 118, 75, 23);

        label.AutoSize = true;
        label2.AutoSize = true;
        textBox.Anchor |= AnchorStyles.Right;
        textBox2.Anchor = textBox.Anchor | AnchorStyles.Right;
        buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

        form.ClientSize = new Size(396, 153);
        form.Controls.AddRange([label, label2, textBox, textBox2, buttonOk, buttonCancel]);
        form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
        form.FormBorderStyle = FormBorderStyle.FixedDialog;
        form.StartPosition = FormStartPosition.CenterScreen;
        form.MinimizeBox = false;
        form.MaximizeBox = false;
        form.AcceptButton = buttonOk;
        form.CancelButton = buttonCancel;

        DialogResult dialogResult = owner != null ? form.ShowDialog(owner) : form.ShowDialog();
        value = textBox.Text;
        value2 = textBox2.Text;
        return dialogResult;
    }
}
