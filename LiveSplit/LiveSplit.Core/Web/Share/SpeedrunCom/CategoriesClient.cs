using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    public class CategoriesClient
    {
        private SpeedrunComClient baseClient;

        public CategoriesClient(SpeedrunComClient baseClient)
        {
            this.baseClient = baseClient;
        }

        public static Uri GetCategoriesUri(string subUri)
        {
            return SpeedrunComClient.GetAPIUri(string.Format("categories{0}", subUri));
        }

        public Category GetCategory(int categoryId, bool embedGame = false, bool embedVariables = false)
        {
            var embedList = new List<string>();

            if (embedGame)
                embedList.Add("game");
            if (embedVariables)
                embedList.Add("variables");

            var embeds = "";
            if (embedList.Any())
                embeds = "?embed=" + embedList.Aggregate((a, b) => a + "," + b);

            var uri = GetCategoriesUri(string.Format("/{0}{1}", categoryId, embeds));
            var result = JSON.FromUri(uri);

            return Category.Parse(baseClient, result.data);
        }

        public ReadOnlyCollection<Variable> GetVariables(int categoryId)
        {
            var uri = GetCategoriesUri(string.Format("/{0}/variables", categoryId));
            return SpeedrunComClient.DoDataCollectionRequest<Variable>(uri,
                x => Variable.Parse(baseClient, x));
        }
    }
}
