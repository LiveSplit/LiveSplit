using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveSplit.Register
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (!EventLog.SourceExists("LiveSplit"))
                    EventLog.CreateEventSource("LiveSplit", "Application");
            }
            catch { }

            try
            {
                

                if (args.Length > 0 && args[0] == "obsplugin")
                    InstallOBSPlugin();
                else
                    FiletypeRegistryHelper.RegisterFileFormats();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        static void InstallOBSPlugin()
        {
            String text = "";
            try
            {
                var obsPath = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles"), "OBS", "plugins");
                if (Environment.Is64BitOperatingSystem)
                {
                    InstallOBSPluginToPath(obsPath, true);
                    text += "The 64-bit version of the OBS Plugin has installed successfully.\r\n";
                }
                else
                {
                    InstallOBSPluginToPath(obsPath, false);
                    text += "The 32-bit version of the OBS Plugin has installed successfully.\r\n";
                }
            }
            catch (Exception ex)
            {
                if (Environment.Is64BitOperatingSystem)
                    text += "The 64-bit version of the OBS Plugin has failed to install.\r\n";
                else
                    text += "The 32-bit version of the OBS Plugin has failed to install.\r\n";
                Log.Error(ex);
            }
            try
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    var obsPath32 = Path.Combine(Environment.GetEnvironmentVariable("PROGRAMFILES(X86)"), "OBS", "plugins");
                    InstallOBSPluginToPath(obsPath32, false);
                    text += "The 32-bit version of the OBS Plugin has installed successfully.\r\n";
                }
            }
            catch (Exception ex)
            {
                text += "The 32-bit version of the OBS Plugin has failed to install.\r\n";
                Log.Error(ex);
            }
            MessageBox.Show(text, "OBS Plugin Setup", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        static void InstallOBSPluginToPath(String obsPath, bool is64Bit)
        {
            if (is64Bit)
                File.Copy("CLRHostPlugin64.dll", Path.Combine(obsPath, "CLRHostPlugin.dll"), true);
            else
                File.Copy("CLRHostPlugin32.dll", Path.Combine(obsPath, "CLRHostPlugin.dll"), true);
            var clrHostPath = Path.Combine(obsPath, "CLRHostPlugin");
            if (!Directory.Exists(clrHostPath))
                Directory.CreateDirectory(clrHostPath);
            if (is64Bit)
                File.Copy("CLRHost.Interop64.dll", Path.Combine(clrHostPath, "CLRHost.Interop.dll"), true);
            else
                File.Copy("CLRHost.Interop32.dll", Path.Combine(clrHostPath, "CLRHost.Interop.dll"), true);
            InstallOtherCrap(clrHostPath);
        }

        static void InstallOtherCrap(String clrHostPath)
        {
            File.Copy("LiveSplit.Core.dll", Path.Combine(clrHostPath, "LiveSplit.Core.dll"), true);
            File.Copy("LiveSplit.Plugin.dll", Path.Combine(clrHostPath, "LiveSplit.Plugin.dll"), true);
            File.Copy("LiveSplit.View.dll", Path.Combine(clrHostPath, "LiveSplit.View.dll"), true);
            File.Copy("UpdateManager.dll", Path.Combine(clrHostPath, "UpdateManager.dll"), true);
            File.Copy("WinFormsColor.dll", Path.Combine(clrHostPath, "WinFormsColor.dll"), true);
            File.Copy("LinqToTwitter.dll", Path.Combine(clrHostPath, "LinqToTwitter.dll"), true);
            File.Copy("Microsoft.WindowsAPICodePack.dll", Path.Combine(clrHostPath, "Microsoft.WindowsAPICodePack.dll"), true);
            File.Copy("Microsoft.WindowsAPICodePack.Shell.dll", Path.Combine(clrHostPath, "Microsoft.WindowsAPICodePack.Shell.dll"), true);
            File.Copy("LiveSplit.Register.exe", Path.Combine(clrHostPath, "LiveSplit.Register.exe"), true);
            File.Copy("SharpDX.dll", Path.Combine(clrHostPath, "SharpDX.dll"), true);
            File.Copy("SharpDX.DirectInput.dll", Path.Combine(clrHostPath, "SharpDX.DirectInput.dll"), true);
            var settingsPath = Path.Combine(clrHostPath, "livesplit.cfg");
            if (File.Exists(settingsPath))
                File.Delete(settingsPath);
            File.Create(settingsPath).Close();
            File.WriteAllText(settingsPath, Path.GetDirectoryName(Application.ExecutablePath));
        }
    }
}
