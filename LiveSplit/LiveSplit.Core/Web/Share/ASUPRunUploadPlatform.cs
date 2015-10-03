using LiveSplit.Model;
using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace LiveSplit.Web.Share
{
    public class ASUPRunUploadPlatform : IRunUploadPlatform
    {
        public string PlatformName { get; protected set; }
        public ISettings Settings { get; set; }

        protected IEnumerable<ASUP.IdPair> gameList;
        protected IList<string> gameNames;

        public Uri BaseUri { get; protected set; }
        protected ASUP ASUP { get; set; }

        public string Description { get; set; }

        public ASUPRunUploadPlatform(
            string platformName, string baseUri,
            string asupUri, string description)
        {
            PlatformName = platformName;
            BaseUri = new Uri(baseUri);
            ASUP = new ASUP(GetUri(asupUri));
            Description = description;
        }

        protected Uri GetUri(string subUri)
        {
            return new Uri(BaseUri, subUri);
        }

        public IEnumerable<ASUP.IdPair> GetGameList()
        {
            return gameList ?? (gameList = ASUP.GetGameList());
        }

        public IEnumerable<string> GetGameNames()
        {
            if (gameNames == null)
            {
                var x = GetGameList();
                gameNames = x.Select(a => HttpUtility.HtmlDecode(a.Value)).OrderBy(a => a).ToList();
            }

            return gameNames;
        }

        public string GetGameIdByName(string gameName)
        {
            var lowerCaseGameName = gameName.ToLowerInvariant();
            return GetGameList().First(a => a.Value.ToLowerInvariant() == lowerCaseGameName).Id;
        }

        public IEnumerable<ASUP.IdPair> GetGameCategories(string gameId)
        {
            return ASUP.GetGameCategories(gameId);
        }

        public string GetCategoryIdByName(string gameId, string categoryName)
        {
            var lowerCaseCategoryName = categoryName.ToLowerInvariant();
            return GetGameCategories(gameId).First(a => a.Value.ToLowerInvariant() == lowerCaseCategoryName).Id;
        }

        public bool VerifyLogin(string username, string password)
        {
            return ASUP.VerifyLogin(username, password);
        }

        public virtual bool SubmitRun(
            IRun run,
            string username, string password, 
            Func<Image> screenShotFunction = null,
            bool attachSplits = false,
            TimingMethod method = TimingMethod.RealTime,
            string gameId = "", string categoryId = "",
            string version = "", string comment = "",
            string video = "",
            params string[] additionalParams)
        {
            try
            {
                if (attachSplits)
                    comment += " " + SplitsIO.Instance.Share(run, screenShotFunction);

                if (gameId == string.Empty)
                    gameId = GetGameIdByName(run.GameName);

                if (categoryId == string.Empty)
                    categoryId = GetCategoryIdByName(gameId, run.CategoryName);

                var json = ASUP.SubmitRun(run, username, password, gameId, categoryId, version, comment, video, additionalParams);

                return json.result == "success";
            }
            catch (Exception e)
            {
                Log.Error(e);

                return false;
            }
        }
    }
}
