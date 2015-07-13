using System.Drawing;
using System.Net;

namespace LiveSplit.Web.Share
{
    public class PBTracker : ASUPRunUploadPlatform
    {
        protected static readonly PBTracker _Instance = new PBTracker();

        public static PBTracker Instance { get { return _Instance; } }

        protected PBTracker()
            : base(
            "PBTracker", "http://www.pbtracker.net/", "asup",
            "PBTracker is a useful platform for keeping track of your Personal Bests. "
          + "You can also view other runners' personal bests from any game or category.")
        { }

        public Image GetGameBoxArt(string gameId)
        {
            var request = WebRequest.Create(GetUri(string.Format("static/boxart/{0}.jpg", gameId)));

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                return Image.FromStream(stream);
            }
        }

        public dynamic GetGame(string gameId)
        {
            return JSON.FromUri(GetUri(string.Format("game/{0}.json", gameId)));
        }
    }
}
