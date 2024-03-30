using LiveSplit.Model;
using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LiveSplit.Web.Share
{
    public interface IRunUploadPlatform
    {
        string PlatformName { get; }
        string Description { get; }
        ISettings Settings { get; set; }

        bool VerifyLogin(string username, string password);
        bool SubmitRun(
            IRun run,
            string username, string password,
            Func<Image> screenShotFunction = null,
            bool attachSplits = false,
            TimingMethod method = TimingMethod.RealTime,
            string gameId = "", string categoryId = "",
            string version = "", string comment = "",
            string video = "",
            params string[] additionalParams);
    }
}
