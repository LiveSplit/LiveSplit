using System;
using System.Collections.Generic;
using System.Drawing;
using LiveSplit.Model;
using LiveSplit.Options;
using System.Windows.Forms;
using LiveSplit.Model.RunSavers;
using System.IO;

namespace LiveSplit.Web.Share
{
    public class Excel : IRunUploadPlatform
    {
        protected static Excel _Instance = new Excel();

        public static Excel Instance => _Instance;

        public string PlatformName => "Excel";

        public string Description =>
@"Export your splits as an Excel Sheet to analyze your splits. 
This includes your whole history of all the runs you ever did.";

        public ISettings Settings { get; set; }

        protected Excel() { }

        #region Not supported

        bool IRunUploadPlatform.VerifyLogin(string username, string password)
        {
            return true;
        }

        #endregion

        public bool SubmitRun(IRun run, string username, string password, Func<Image> screenShotFunction = null, bool attachSplits = false, TimingMethod method = TimingMethod.RealTime, string gameId = "", string categoryId = "", string version = "", string comment = "", string video = "", params string[] additionalParams)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Excel Sheet (*.xlsx)|*.xlsx";
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    var path = dialog.FileName;

                    if (!File.Exists(path))
                        File.Create(path).Close();
                    using (var stream = File.Open(path, FileMode.Create, FileAccess.Write))
                    {
                        var runSaver = new ExcelRunSaver();
                        runSaver.Save(run, stream);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
