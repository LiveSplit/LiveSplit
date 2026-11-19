using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UpdateManager;

namespace LiveSplit.Updates;

public static class UpdateHelper
{
    public static readonly Version Version = GetVersionFromGit();
    public static string UserAgent => GetUserAgent();

    public static readonly List<Type> AlreadyChecked = [];

    private static readonly object AlreadyCheckedLock = new();

    public static void Update(Form form, Action closeAction, params IUpdateable[] updateables)
    {
        _ = Task.Run(() =>
        {
            try
            {
                if (form.IsDisposed)
                {
                    return;
                }

                IUpdateable[] actualUpdateables;
                lock (AlreadyCheckedLock)
                {
                    actualUpdateables = [.. updateables.Where(x => !AlreadyChecked.Contains(x.GetType()))];
                }

                if (actualUpdateables.Length == 0)
                {
                    return;
                }

                if (Updater.CheckForAnyUpdate(actualUpdateables))
                {
                    IUpdateable[] updates = [.. actualUpdateables.Where(x => x.CheckForUpdate())];
                    string dialogText = string.Concat(
                        updates.Select(x =>
                            x.UpdateName + " (" + x.GetNewVersion() + ")\r\n" +
                            string.Concat(x.GetChangeLog().Select(y => " - " + y + "\r\n")) + "\r\n")) +
                        "Do you want to update?";

                    Action promptForUpdates = () =>
                    {
                        DialogResult result = new ScrollableMessageBox().Show(dialogText, "New updates are available", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            try
                            {
                                Updater.UpdateAll(updates, "http://livesplit.org/update/UpdateManagerV2.exe", "http://livesplit.org/update/UpdateManagerV2.exe.config");
                                closeAction();
                            }
                            catch (Exception e)
                            {
                                Log.Error(e);
                            }
                        }
                    };

                    if (form.InvokeRequired)
                    {
                        form.Invoke(promptForUpdates);
                    }
                    else
                    {
                        promptForUpdates();
                    }
                }

                lock (AlreadyCheckedLock)
                {
                    AlreadyChecked.AddRange(actualUpdateables.Select(x => x.GetType()));
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        });
    }

    private static Version GetVersionFromGit()
    {
        try
        {
            return Version.Parse($"{Git.LastTag}.{Git.CommitsSinceLastTag}");
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return null;
        }
    }

    private static string GetUserAgent()
    {
        string versionString = (Version != null) ? Version.ToString() : "Unknown";
        return $"LiveSplit/{versionString}";
    }
}
