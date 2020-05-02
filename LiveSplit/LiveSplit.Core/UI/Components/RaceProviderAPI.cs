using LiveSplit.Model;
using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using UpdateManager;

namespace LiveSplit.UI.Components
{
    public interface IRaceProviderFactory : IUpdateable
    {
        RaceProviderAPI Create(ITimerModel model, RaceProviderSettings settings);
        RaceProviderSettings CreateSettings();
    }

    public abstract class RaceProviderAPI
    {
        public bool IsActivated { get; set; } = true;
        public abstract IEnumerable<IRaceInfo> GetRaces();
        public RacesRefreshedCallback RacesRefreshedCallback;
        public JoinRaceDelegate JoinRace;
        public CreateRaceDelegate CreateRace;
        public abstract void RefreshRacesListAsync();
        public abstract Image GetGameImage(string id);
        public abstract string ProviderName { get; }
        public abstract string Username { get; }
        public RaceProviderSettings Settings { get; set; }
    }

    public delegate void RacesRefreshedCallback(RaceProviderAPI api);
    public delegate void JoinRaceDelegate(ITimerModel model, string raceid);
    public delegate void CreateRaceDelegate(ITimerModel model);
}
