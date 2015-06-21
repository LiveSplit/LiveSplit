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
            string platformId = null, string regionId = null, 
            string moderatorId = null, int? elementsPerPage = null,
            GameEmbeds embeds = default(GameEmbeds))
        {
            var parameters = new List<string>() { embeds.ToString() };

            if (name != null)
                parameters.Add(string.Format("name={0}", HttpUtility.UrlPathEncode(name)));

            if (yearOfRelease.HasValue)
                parameters.Add(string.Format("released={0}", yearOfRelease.Value));

            if (!string.IsNullOrEmpty(platformId))
                parameters.Add(string.Format("platform={0}", HttpUtility.UrlPathEncode(platformId)));

            if (!string.IsNullOrEmpty(regionId))
                parameters.Add(string.Format("region={0}", HttpUtility.UrlPathEncode(regionId)));

            if (!string.IsNullOrEmpty(moderatorId))
                parameters.Add(string.Format("moderator={0}", HttpUtility.UrlPathEncode(moderatorId)));

            if (elementsPerPage.HasValue)
                parameters.Add(string.Format("max={0}", elementsPerPage.Value));

            var uri = GetGamesUri(parameters.ToParameters());
            return SpeedrunComClient.DoPaginatedRequest(uri, 
                x => Game.Parse(baseClient, x) as Game);
        }

        public IEnumerable<GameHeader> GetGameHeaders(int elementsPerPage = 1000)
        {
            var uri = GetGamesUri(string.Format("?_bulk=yes&max={0}", elementsPerPage));
            return SpeedrunComClient.DoPaginatedRequest(uri,
                x => GameHeader.Parse(baseClient, x) as GameHeader);
        }

        public Game GetGame(string gameId, GameEmbeds embeds = default(GameEmbeds))
        {
            var parameters = new List<string>() { embeds.ToString() };

            var uri = GetGamesUri(string.Format("/{0}{1}", 
                HttpUtility.UrlPathEncode(gameId), 
                parameters.ToParameters()));

            var result = JSON.FromUri(uri);

            return Game.Parse(baseClient, result.data);
        }

        public ReadOnlyCollection<Category> GetCategories(
            string gameId, bool miscellaneous = true,
            CategoryEmbeds embeds = default(CategoryEmbeds))
        {
            var parameters = new List<string>() { embeds.ToString() };

            if (!miscellaneous)
                parameters.Add("miscellaneous=no");

            var uri = GetGamesUri(string.Format("/{0}/categories{1}", 
                HttpUtility.UrlPathEncode(gameId), 
                parameters.ToParameters()));

            return SpeedrunComClient.DoDataCollectionRequest(uri,
                x => Category.Parse(baseClient, x) as Category);
        }

        public ReadOnlyCollection<Level> GetLevels(string gameId,
            LevelEmbeds embeds = default(LevelEmbeds))
        {
            var parameters = new List<string>() { embeds.ToString() };

            var uri = GetGamesUri(string.Format("/{0}/levels{1}", 
                HttpUtility.UrlPathEncode(gameId),
                parameters.ToParameters()));

            return SpeedrunComClient.DoDataCollectionRequest(uri,
                 x => Level.Parse(baseClient, x) as Level);
        }

        public ReadOnlyCollection<Variable> GetVariables(string gameId)
        {
            var uri = GetGamesUri(string.Format("/{0}/variables", HttpUtility.UrlPathEncode(gameId)));
            return SpeedrunComClient.DoDataCollectionRequest(uri,
                x => Variable.Parse(baseClient, x) as Variable);
        }

        public ReadOnlyCollection<Game> GetChildren(string gameId,
            GameEmbeds embeds = default(GameEmbeds))
        {
            var parameters = new List<string>() { embeds.ToString() };

            var uri = GetGamesUri(string.Format("/{0}/children{1}", 
                HttpUtility.UrlPathEncode(gameId),
                parameters.ToParameters()));

            return SpeedrunComClient.DoDataCollectionRequest(uri, 
                x => Game.Parse(baseClient, x) as Game);
        }
    }
}
