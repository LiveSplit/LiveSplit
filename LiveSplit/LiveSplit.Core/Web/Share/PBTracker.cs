using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.Web.Share
{
    public class PBTracker : ASUPRunUploadPlatform
    {
        protected static PBTracker _Instance = new PBTracker();

        public static PBTracker Instance { get { return _Instance; } }

        protected PBTracker()
            : base(
            "PBTracker", "http://www.pbtracker.net/", "asup",
            "PBTracker is a useful platform for keeping track of your Personal Bests. "
          + "You can also view other runners' personal bests from any game or category.")
        { }

        public Image GetGameBoxArt(String gameId)
        {
            var request = WebRequest.Create(GetUri(String.Format("static/boxart/{0}.jpg", gameId)));
            var response = request.GetResponse();
            var stream = response.GetResponseStream();
            return Image.FromStream(stream);
        }

        public dynamic GetGame(String gameId)
        {
            return JSON.FromUri(GetUri(String.Format("game/{0}.json", gameId)));
        }
    }
}
