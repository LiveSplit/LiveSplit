using LiveSplit.Options;
using System;
using System.Diagnostics;
using System.IO;
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
                FiletypeRegistryHelper.RegisterFileFormats();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

				// Add LiveSplit.exe to Registry if not already there, so that Emulated Browser can load Twitch properly (look up Emulated Browsers & Compatibility View if interested)
            try
            {
                if (!InternetExplorerBrowserEmulation.IsBrowserEmulationSet("LiveSplit.exe"))
                    InternetExplorerBrowserEmulation.SetBrowserEmulationVersion("LiveSplit.exe");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}
