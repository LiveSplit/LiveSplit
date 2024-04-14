using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UpdateManager;

namespace LiveSplit.Updates
{
    public static class UpdateHelper
    {
        public static readonly Version Version = Version.Parse($"{ Git.LastTag }.{ Git.CommitsSinceLastTag }");
        public static string UserAgent => $"LiveSplit/{ Version }";

        public static readonly List<Type> AlreadyChecked = new List<Type>();

        public static void Update(Form form, Action closeAction, params IUpdateable[] updateables)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var actualUpdateables = updateables.Where(x => !AlreadyChecked.Contains(x.GetType()));
                    if (Updater.CheckForAnyUpdate(actualUpdateables))
                    {
                        string dialogText = actualUpdateables.Where(x => x.CheckForUpdate()).Select(x =>
                                x.UpdateName + " (" + x.GetNewVersion() + ")\r\n" +
                                x.GetChangeLog().Select(y => " - " + y + "\r\n")
                                        .Aggregate("", (y, z) => y + z) + "\r\n")
                                        .Aggregate((x, y) => x + y) + "Do you want to update?";

                        Action promptForUpdates = () =>
                        {
                            DialogResult result = (new ScrollableMessageBox()).Show(dialogText, "New updates are available", MessageBoxButtons.YesNo);
                            if (result == DialogResult.Yes)
                            {
                                try
                                {
                                    Updater.UpdateAll(actualUpdateables, "http://livesplit.org/update/UpdateManagerV2.exe", "http://livesplit.org/update/UpdateManagerV2.exe.config");
                                    closeAction();
                                }
                                catch (Exception e)
                                {
                                    Log.Error(e);
                                }
                            }
                        };

                        if (form.InvokeRequired)
                            form.Invoke(promptForUpdates);
                        else
                            promptForUpdates();
                    }
                    AlreadyChecked.AddRange(actualUpdateables.Select(x => x.GetType()));
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            });
        }
    }
}
