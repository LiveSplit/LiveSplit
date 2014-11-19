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
        public String PlatformName { get; protected set; }
        public ISettings Settings { get; set; }

        protected IEnumerable<ASUP.IdPair> gameList;
        protected IList<String> gameNames;

        public Uri BaseUri { get; protected set; }
        protected ASUP ASUP { get; set; }

        public String Description { get; set; }

        public ASUPRunUploadPlatform(
            String platformName, String baseUri, 
            String asupUri, String description)
        {
            PlatformName = platformName;
            BaseUri = new Uri(baseUri);
            ASUP = new ASUP(GetUri(asupUri));
            Description = description;
        }

        protected Uri GetUri(String subUri)
        {
            return new Uri(BaseUri, subUri);
        }

        public IEnumerable<ASUP.IdPair> GetGameList()
        {
            if (gameList == null)
                gameList = ASUP.GetGameList();

            return gameList;
        }

        public IEnumerable<String> GetGameNames()
        {
            if (gameNames == null)
            {
                var x = GetGameList();
                gameNames = x.Select(a => HttpUtility.HtmlDecode(a.Value)).OrderBy(a => a).ToList();
            }

            return gameNames;
        }

        public String GetGameIdByName(String gameName)
        {
            var lowerCaseGameName = gameName.ToLowerInvariant();
            return GetGameList().First(a => a.Value.ToLowerInvariant() == lowerCaseGameName).Id;
        }

        public IEnumerable<ASUP.IdPair> GetGameCategories(String gameId)
        {
            return ASUP.GetGameCategories(gameId);
        }

        public String GetCategoryIdByName(String gameId, String categoryName)
        {
            var lowerCaseCategoryName = categoryName.ToLowerInvariant();
            return GetGameCategories(gameId).First(a => a.Value.ToLowerInvariant() == lowerCaseCategoryName).Id;
        }

        public bool VerifyLogin(String username, String password)
        {
            return ASUP.VerifyLogin(username, password);
        }

        public virtual bool SubmitRun(
            IRun run,
            String username, String password, 
            Func<Image> screenShotFunction = null,
            bool attachSplits = false,
            TimingMethod method = TimingMethod.RealTime,
            String gameId = "", String categoryId = "",
            String version = "", String comment = "",
            String video = "",
            params String[] additionalParams)
        {
            try
            {
                if (attachSplits)
                    comment += " " + SplitsIO.Instance.Share(run, screenShotFunction);
                if (gameId == String.Empty)
                    gameId = GetGameIdByName(run.GameName);
                if (categoryId == String.Empty)
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
