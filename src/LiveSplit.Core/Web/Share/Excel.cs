using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using LiveSplit.Model;
using LiveSplit.Model.RunSavers;
using LiveSplit.Options;

namespace LiveSplit.Web.Share;

public class Excel : IRunUploadPlatform
{
    protected static Excel _Instance = new();

    public static Excel Instance => _Instance;

    public string PlatformName => "Excel";

    public string Description
=> @"Export your splits as an Excel Sheet to analyze your splits. 
This includes your whole history of all the runs you ever did.";

    public ISettings Settings { get; set; }

    protected Excel() { }

    #region Not supported

    bool IRunUploadPlatform.VerifyLogin()
    {
        return true;
    }

    #endregion

    public bool SubmitRun(IRun run, Func<Image> screenShotFunction = null, bool attachSplits = false, TimingMethod method = TimingMethod.RealTime, string comment = "", params string[] additionalParams)
    {
        using var dialog = new SaveFileDialog();
        dialog.Filter = "Excel Sheet (*.xlsx)|*.xlsx";
        DialogResult result = dialog.ShowDialog();
        if (result == DialogResult.OK)
        {
            string path = dialog.FileName;

            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }

            using FileStream stream = File.Open(path, FileMode.Create, FileAccess.Write);
            var runSaver = new ExcelRunSaver();
            runSaver.Save(run, stream);
            return true;
        }

        return false;
    }
}
