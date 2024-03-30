using LiveSplit.Model;
using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LiveSplit.Web.Share
{
    public class Screenshot : IRunUploadPlatform
    {
        public ISettings Settings { get; set; }
        protected static readonly Screenshot _Instance = new Screenshot();

        public static Screenshot Instance => _Instance;

        protected Screenshot() { }

        public string PlatformName => "Screenshot";

        public string Description => "Sharing will save a screenshot of LiveSplit.";

        public bool VerifyLogin()
        {
            return true;
        }

        public bool SubmitRun(IRun run, Func<System.Drawing.Image> screenShotFunction = null, bool attachSplits = false, TimingMethod method = TimingMethod.RealTime, string comment = "", params string[] additionalParams)
        {
            try
            {
                var image = screenShotFunction();

                using (var dialog = new SaveFileDialog())
                {
                    dialog.Filter = "PNG (*.png)|*.png|JPEG (*.jpeg)|*.jpeg|GIF (*.gif)|*.gif|Bitmap (*.bmp)|*.bmp|TIFF (*.tiff)|*.tiff|WMF (*.wmf)|*.wmf";
                    var result = dialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        image.Save(dialog.FileName);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return false;
        }
    }
}
