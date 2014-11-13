using LiveSplit.Model;
using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace LiveSplit.Web.Share
{
    public class Screenshot : IRunUploadPlatform
    {
        public ISettings Settings { get; set; }
        protected static Screenshot _Instance = new Screenshot();

        public static Screenshot Instance { get { return _Instance; } }

        protected Screenshot() { }

        public string PlatformName
        {
            get { return "Screenshot"; }
        }

        public string Description
        {
            get 
            {
                return "Sharing will save a screenshot of LiveSplit.";
            }
        }

        public IEnumerable<ASUP.IdPair> GetGameList()
        {
            yield break;
        }

        public IEnumerable<string> GetGameNames()
        {
            yield break;
        }

        public string GetGameIdByName(string gameName)
        {
            return String.Empty;
        }

        public IEnumerable<ASUP.IdPair> GetGameCategories(string gameId)
        {
            yield break;
        }

        public string GetCategoryIdByName(string gameId, string categoryName)
        {
            return String.Empty;
        }

        public bool VerifyLogin(string username, string password)
        {
            return true;
        }

        public bool SubmitRun(IRun run, string username, string password, Func<System.Drawing.Image> screenShotFunction = null, bool attachSplits = false, TimingMethod method = TimingMethod.RealTime, string gameId = "", string categoryId = "", string version = "", string comment = "", string video = "", params string[] additionalParams)
        {
            try
            {
                var image = screenShotFunction();
                var dialog = new SaveFileDialog();
                dialog.Filter = "PNG (*.png)|*.png|JPEG (*.jpeg)|*.jpeg|GIF (*.gif)|*.gif|Bitmap (*.bmp)|*.bmp|TIFF (*.tiff)|*.tiff|WMF (*.wmf)|*.wmf";
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    image.Save(dialog.FileName);
                    return true;
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
