using System;
using System.IO;
using System.Windows.Forms;

using LiveSplit.View;

namespace LiveSplit;

internal static class Program
{
    /// <summary>
    /// Der Haupteinstiegspunkt für die Anwendung.
    /// </summary>
    [STAThread]
    private static void Main(string[] args)
    {
        try
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Environment.CurrentDirectory = Path.GetDirectoryName(Application.ExecutablePath);

#if !DEBUG
            Options.FiletypeRegistryHelper.RegisterFileFormatsIfNotAlreadyRegistered();
#endif

            string splitsPath = null;
            string layoutPath = null;

            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i] == "-s")
                {
                    splitsPath = args[++i];
                }
                else if (args[i] == "-l")
                {
                    layoutPath = args[++i];
                }
            }

            Application.Run(new TimerForm(splitsPath: splitsPath, layoutPath: layoutPath));
        }
#if !DEBUG
        catch (Exception e)
        {
            Options.Log.Error(e);
            MessageBox.Show(string.Format("LiveSplit has crashed due to the following reason:\n\n{0}", e.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
#endif
        finally
        {
        }
    }
}
