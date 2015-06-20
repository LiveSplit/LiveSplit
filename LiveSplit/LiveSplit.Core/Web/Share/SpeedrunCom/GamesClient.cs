using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class GamesClient
    {
        private SpeedrunComClient baseClient;

        public GamesClient(SpeedrunComClient baseClient)
        {
            this.baseClient = baseClient;
        }

        public static Uri GetGamesUri(string subUri)
        {
            return SpeedrunComClient.GetAPIUri(string.Format("games{0}", subUri));
        }

        public IEnumerable<Game> GetGames(
            string name = null, int? yearOfRelease = null, 
            int? platformId = null, int? regionId = null, 
            int? moderatorId = null, int? elementsPerPage = null)
        {
            var filterList = new List<string>();

            if (name != null)
                filterList.Add(string.Format("name={0}", HttpUtility.UrlPathEncode(name)));

            if (yearOfRelease.HasValue)
                filterList.Add(string.Format("released={0}", yearOfRelease.Value));

            if (platformId.HasValue)
                filterList.Add(string.Format("platform={0}", platformId.Value));

            if (regionId.HasValue)
                filterList.Add(string.Format("region={0}", regionId.Value));

            if (moderatorId.HasValue)
                filterList.Add(string.Format("moderator={0}", moderatorId.Value));

            if (elementsPerPage.HasValue)
                filterList.Add(string.Format("max={0}", elementsPerPage.Value));

            var filters = "";
            if (filterList.Any())
                filters = "?" + filterList.Aggregate((a, b) => a + "&" + b);

            var uri = GetGamesUri(filters);
            var elements = SpeedrunComClient.DoPaginatedRequest(uri);

            return elements.Select(x => Game.Parse(baseClient, x) as Game);
        }

        public IEnumerable<GameHeader> GetGameHeaders(int elementsPerPage = 1000)
        {
            var uri = GetGamesUri(string.Format("?_bulk=yes&max={0}", elementsPerPage));
            var elements = SpeedrunComClient.DoPaginatedRequest(uri);

            return elements.Select(x => GameHeader.Parse(baseClient, x) as GameHeader);
        }

        public Game GetGame(int gameId,
            bool embedLevels = false, bool embedCategories = false,
            bool embedModerators = false, bool embedPlatforms = false,
            bool embedRegions = false, bool embedVariables = false)
        {
            var embedList = new List<string>();

            if (embedLevels)
                embedList.Add("levels");
            if (embedCategories)
                embedList.Add("categories");
            if (embedModerators)
                embedList.Add("moderators");
            if (embedPlatforms)
                embedList.Add("platforms");
            if (embedRegions)
                embedList.Add("regions");
            if (embedVariables)
                embedList.Add("variables");

            var embeds = "";
            if (embedList.Any())
                embeds = "?embed=" + embedList.Aggregate((a, b) => a + "," + b);

            var uri = GetGamesUri(string.Format("/{0}{1}", gameId, embeds));
            var result = JSON.FromUri(uri);

            return Game.Parse(baseClient, result.data);
        }

        public ReadOnlyCollection<Category> GetCategories(int gameId, bool miscellaneous = true)
        {
            var uri = GetGamesUri(string.Format("/{0}/categories{1}", gameId, miscellaneous ? "" : "?miscellaneous=no"));
            return SpeedrunComClient.DoDataCollectionRequest<Category>(uri,
                x => Category.Parse(baseClient, x));
        }

        public ReadOnlyCollection<Level> GetLevels(int gameId)
        {
            var uri = GetGamesUri(string.Format("/{0}/levels", gameId));
            return SpeedrunComClient.DoDataCollectionRequest<Level>(uri,
                 x => Level.Parse(baseClient, x));
        }

        public ReadOnlyCollection<Variable> GetVariables(int gameId)
        {
            var uri = GetGamesUri(string.Format("/{0}/variables", gameId));
            return SpeedrunComClient.DoDataCollectionRequest<Variable>(uri,
                x => Variable.Parse(baseClient, x));
        }

        public ReadOnlyCollection<Game> GetChildren(int gameId)
        {
            var uri = GetGamesUri(string.Format("/{0}/children", gameId));
            return SpeedrunComClient.DoDataCollectionRequest<Game>(uri, 
                x => Game.Parse(baseClient, x));
        }
    }
}
