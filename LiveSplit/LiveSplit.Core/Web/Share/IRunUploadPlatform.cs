using LiveSplit.Model;
using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.Web.Share
{
    public interface IRunUploadPlatform
    {
        String PlatformName { get; }
        String Description { get; }
        ISettings Settings { get; set; }

        IEnumerable<ASUP.IdPair> GetGameList();
        IEnumerable<String> GetGameNames();
        String GetGameIdByName(String gameName);
        IEnumerable<ASUP.IdPair> GetGameCategories(String gameId);
        String GetCategoryIdByName(String gameId, String categoryName);
        bool VerifyLogin(String username, String password);
        bool SubmitRun(
            IRun run,
            String username, String password,
            Func<Image> screenShotFunction = null,
            bool attachSplits = false,
            TimingMethod method = TimingMethod.RealTime,
            String gameId = "", String categoryId = "",
            String version = "", String comment = "",
            String video = "",
            params String[] additionalParams);
    }
}
