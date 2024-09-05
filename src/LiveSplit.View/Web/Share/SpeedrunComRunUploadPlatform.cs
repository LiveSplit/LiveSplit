using System;
using System.Windows.Forms;

using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.View;

namespace LiveSplit.Web.Share;

public class SpeedrunComRunUploadPlatform : IRunUploadPlatform
{
    public ISettings Settings { get; set; }
    protected static readonly SpeedrunComRunUploadPlatform instance = new();

    public static SpeedrunComRunUploadPlatform Instance => instance;

    protected SpeedrunComRunUploadPlatform() { }

    public string PlatformName => "Speedrun.com";

    public string Description
        => "Speedrun.com is a site intended to provide centralized leaderboards for speedrunning.";

    public bool VerifyLogin()
    {
        return true;
    }

    public bool SubmitRun(IRun run, Func<System.Drawing.Image> screenShotFunction = null, bool attachSplits = false, TimingMethod method = TimingMethod.RealTime, string comment = "", params string[] additionalParams)
    {
        bool isValid = SpeedrunCom.ValidateRun(run.Metadata.LiveSplitRun, out string reason);

        if (!isValid)
        {
            MessageBox.Show(reason, "Submitting Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        using var submitDialog = new SpeedrunComSubmitDialog(run.Metadata);
        DialogResult result = submitDialog.ShowDialog();
        return result == DialogResult.OK;
    }
}
