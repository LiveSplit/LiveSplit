using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web;

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

        public Category GetCategory(string categoryId, CategoryEmbeds embeds = default(CategoryEmbeds))
        {
            var uri = GetCategoriesUri(string.Format("/{0}{1}", HttpUtility.UrlPathEncode(categoryId), embeds.ToString().ToParameters()));
            var result = JSON.FromUri(uri);

            return Category.Parse(baseClient, result.data);
        }

        public ReadOnlyCollection<Variable> GetVariables(string categoryId)
        {
            var uri = GetCategoriesUri(string.Format("/{0}/variables", HttpUtility.UrlPathEncode(categoryId)));
            return SpeedrunComClient.DoDataCollectionRequest<Variable>(uri,
                x => Variable.Parse(baseClient, x));
        }
    }
}
