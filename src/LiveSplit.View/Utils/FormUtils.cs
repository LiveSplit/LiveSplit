using System;
using System.Drawing;
using System.Windows.Forms;

namespace LiveSplit.Utils;

/// <summary>
/// Class that provides extension methods
/// that act upon Windows Forms related controls.
/// </summary>
internal static class FormUtils
{
    /// <summary>
    /// Executes an <see cref="Action"/>, invoking it if necessary.
    /// </summary>
    /// <param name="control">The control to act upon.</param>
    /// <param name="action">The action to execute.</param>
    public static void InvokeIfRequired(this Control control, Action action)
    {
        if (control.InvokeRequired)
        {
            control.Invoke(action);
        }
        else
        {
            action();
        }
    }

    /// <summary>
    /// Moves a form so that it is fully contained within the working area of the
    /// screen its owner is on. Forms that already fit are left untouched.
    /// </summary>
    /// <param name="form">The form to act upon. Its handle must already be created.</param>
    public static void MoveIntoOwnerScreen(this Form form)
    {
        Rectangle workingArea = (form.Owner != null ? Screen.FromControl(form.Owner) : Screen.FromControl(form)).WorkingArea;
        int x = Math.Max(workingArea.X, Math.Min(form.Left, workingArea.Right - form.Width));
        int y = Math.Max(workingArea.Y, Math.Min(form.Top, workingArea.Bottom - form.Height));
        form.Location = new Point(x, y);
    }
}
