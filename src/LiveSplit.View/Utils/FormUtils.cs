using System;
using System.Windows.Forms;

namespace LiveSplit.Utils
{
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
                control.Invoke(action);
            else
                action();
        }
    }
}
