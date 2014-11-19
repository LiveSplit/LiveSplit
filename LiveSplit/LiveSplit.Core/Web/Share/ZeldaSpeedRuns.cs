using LiveSplit.Model;
using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LiveSplit.Web.Share
{
    public class ZeldaSpeedRuns : IRunUploadPlatform
    {
        public string PlatformName
        {
            get { return "ZeldaSpeedRuns"; }
        }

        public String Description
        {
            get { return ""; }
        }

        public ISettings Settings { get; set; }

        public IEnumerable<ASUP.IdPair> GetGameList()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetGameNames()
        {
            throw new NotImplementedException();
        }

        public string GetGameIdByName(string gameName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ASUP.IdPair> GetGameCategories(string gameId)
        {
            throw new NotImplementedException();
        }

        public string GetCategoryIdByName(string gameId, string categoryName)
        {
            throw new NotImplementedException();
        }

        public bool VerifyLogin(string username, string password)
        {
            throw new NotImplementedException();
        }

        public bool SubmitRun(Model.IRun run, string username, string password, Func<Image> screenShotFunction = null, bool attachSplits = false, TimingMethod method = TimingMethod.RealTime, string gameId = "", string categoryId = "", string version = "", string comment = "", string video = "", params string[] additionalParams)
        {
            throw new NotImplementedException();
        }
    }
}
