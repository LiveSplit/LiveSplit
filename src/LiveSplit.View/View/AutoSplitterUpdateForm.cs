using LiveSplit.Localization;
using LiveSplit.Model;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveSplit.View;

public sealed class AutoSplitterUpdateForm : Form
{
    private readonly Label statusLabel;
    private readonly ProgressBar progressBar;
    private Func<IProgress<AutoSplitterUpdateProgress>, Task> operation;
    private Exception operationError;

    public AutoSplitterUpdateForm()
    {
        Text = UiLocalizer.Translate("Updating Auto Splitter");
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ControlBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(560, 160);
        Padding = new Padding(16);
        AutoScaleMode = AutoScaleMode.Dpi;

        statusLabel = new Label()
        {
            AutoEllipsis = false,
            AutoSize = false,
            Dock = DockStyle.Fill,
            Text = UiLocalizer.Translate("Preparing Auto Splitter..."),
            TextAlign = ContentAlignment.MiddleLeft,
            UseMnemonic = false
        };

        progressBar = new ProgressBar()
        {
            Dock = DockStyle.Fill,
            Maximum = 100,
            Minimum = 0,
            Style = ProgressBarStyle.Continuous
        };

        var layout = new TableLayoutPanel()
        {
            ColumnCount = 1,
            Dock = DockStyle.Fill,
            RowCount = 2
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        layout.Controls.Add(statusLabel, 0, 0);
        layout.Controls.Add(progressBar, 0, 1);
        Controls.Add(layout);
    }

    public static bool Run(IWin32Window owner, string title, Func<IProgress<AutoSplitterUpdateProgress>, Task> operation)
    {
        using var form = new AutoSplitterUpdateForm()
        {
            Text = UiLocalizer.Translate(title),
            operation = operation
        };
        form.StartPosition = owner == null ? FormStartPosition.CenterScreen : FormStartPosition.CenterParent;
        form.Shown += form.AutoSplitterUpdateForm_Shown;
        form.ShowDialog(owner);

        if (form.operationError != null)
        {
            throw new InvalidOperationException(UiLocalizer.Translate("The Auto Splitter initialization failed."), form.operationError);
        }

        return true;
    }

    private async void AutoSplitterUpdateForm_Shown(object sender, EventArgs e)
    {
        try
        {
            await operation(new Progress<AutoSplitterUpdateProgress>(UpdateProgress));
        }
        catch (Exception ex)
        {
            operationError = ex;
        }
        finally
        {
            Close();
        }
    }
    public void UpdateProgress(AutoSplitterUpdateProgress progress)
    {
        if (IsDisposed)
        {
            return;
        }

        if (InvokeRequired)
        {
            BeginInvoke(new Action<AutoSplitterUpdateProgress>(UpdateProgress), progress);
            return;
        }

        statusLabel.Text = progress.Status;
        progressBar.Value = (int)Math.Round(progress.Percentage);
    }
}