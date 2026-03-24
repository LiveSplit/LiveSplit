using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

using LiveSplit.Localization;
using LiveSplit.View;

namespace LiveSplit;

internal static class Program
{
    private const string SettingsFileName = "settings.cfg";
    private const string UiLanguageNodeName = "UILanguage";

    /// <summary>
    /// Der Haupteinstiegspunkt für die Anwendung.
    /// </summary>
    [STAThread]
    private static void Main(string[] args)
    {
        try
        {
            SetStartupCulture();
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
            string message = string.Format(
                UiLocalizer.TranslateKey(LocalizationKeys.CrashReason, "LiveSplit has crashed due to the following reason:\n\n{0}"),
                e.Message);
            MessageBox.Show(message, UiLocalizer.TranslateKey(LocalizationKeys.Error, "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
#endif
        finally
        {
        }
    }

    private static void SetStartupCulture()
    {
        try
        {
            string executableDirectory = Path.GetDirectoryName(Application.ExecutablePath) ?? string.Empty;
            string settingsPath = Path.Combine(executableDirectory, SettingsFileName);
            UiTextCatalog.Initialize(executableDirectory);
            string languageValue = ReadLanguageSetting(settingsPath);
            CultureInfo targetCulture = LanguageResolver.ResolveCulture(languageValue);

            CultureInfo.DefaultThreadCurrentCulture = targetCulture;
            CultureInfo.DefaultThreadCurrentUICulture = targetCulture;
            Thread.CurrentThread.CurrentCulture = targetCulture;
            Thread.CurrentThread.CurrentUICulture = targetCulture;
        }
        catch (Exception e)
        {
            Options.Log.Error(e);
        }
    }

    private static string ReadLanguageSetting(string settingsPath)
    {
        if (!File.Exists(settingsPath))
        {
            return string.Empty;
        }

        var document = new XmlDocument();
        document.Load(settingsPath);
        XmlNode languageNode = document.SelectSingleNode($"/Settings/{UiLanguageNodeName}");
        return languageNode?.InnerText ?? string.Empty;
    }
}
