﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LiveSplit.View;

public partial class EditHistoryDialog : Form
{
    public IList<string> History { get; set; }

    public EditHistoryDialog(IEnumerable<string> history)
    {
        InitializeComponent();
        History = history.Reverse().ToList();
        historyListBox.Items.AddRange(History.Where(x => !string.IsNullOrEmpty(x)).ToArray());
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
        History = History.Reverse().ToList();
        DialogResult = DialogResult.OK;
        Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void btnRemove_Click(object sender, EventArgs e)
    {
        foreach (object item in historyListBox.SelectedItems)
        {
            History.Remove((string)item);
        }

        historyListBox.Items.Clear();
        historyListBox.Items.AddRange(History.Where(x => !string.IsNullOrEmpty(x)).ToArray());
    }
}
