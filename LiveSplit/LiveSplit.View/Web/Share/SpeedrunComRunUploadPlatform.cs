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
        protected static readonly SpeedrunComRunUploadPlatform instance = new SpeedrunComRunUploadPlatform();

        public static SpeedrunComRunUploadPlatform Instance => instance;

        protected SpeedrunComRunUploadPlatform() { }

        public string PlatformName => "Speedrun.com";

        public string Description =>
            "Speedrun.com is a site intended to provide centralized leaderboards for speedrunning.";

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
