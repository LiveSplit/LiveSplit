using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.View;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LiveSplit.Web.Share
{
    public class SpeedrunComRunUploadPlatform : IRunUploadPlatform
    {
        public ISettings Settings { get; set; }
        protected static readonly SpeedrunComRunUploadPlatform _Instance = new SpeedrunComRunUploadPlatform();

        public static SpeedrunComRunUploadPlatform Instance { get { return _Instance; } }

        protected SpeedrunComRunUploadPlatform() { }

        public string PlatformName
        {
            get { return "Speedrun.com"; }
        }

        public string Description
        {
            get 
            {
                return "Speedrun.com is a site intended to provide centralized leaderboards for speedrunning.";
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
            return string.Empty;
        }

        public IEnumerable<ASUP.IdPair> GetGameCategories(string gameId)
        {
            yield break;
        }

        public string GetCategoryIdByName(string gameId, string categoryName)
        {
            return string.Empty;
        }

        public bool VerifyLogin(string username, string password)
        {
            return true;
        }

        public bool SubmitRun(IRun run, string username, string password, Func<System.Drawing.Image> screenShotFunction = null, bool attachSplits = false, TimingMethod method = TimingMethod.RealTime, string gameId = "", string categoryId = "", string version = "", string comment = "", string video = "", params string[] additionalParams)
        {
            string reason;
            var isValid = SpeedrunCom.ValidateRun(run.Metadata.LiveSplitRun, out reason);

            if (!isValid)
            {
                MessageBox.Show(reason, "Submitting Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            using (var submitDialog = new SpeedrunComSubmitDialog(run.Metadata))
            {
                var result = submitDialog.ShowDialog();
                return result == DialogResult.OK;
            }
        }
    }
}
