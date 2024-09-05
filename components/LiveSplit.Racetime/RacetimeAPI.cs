using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using LiveSplit.Model;
using LiveSplit.Racetime.Controller;
using LiveSplit.Racetime.Model;
using LiveSplit.Racetime.View;
using LiveSplit.UI.Components;
using LiveSplit.Web;

namespace LiveSplit.Racetime;

public class RacetimeAPI : RaceProviderAPI
{
    protected static readonly Uri BaseUri = new($"{Properties.Resources.PROTOCOL_REST}://{Properties.Resources.DOMAIN}/");
    protected static string racesEndpoint => Properties.Resources.ENDPOINT_RACES;
    private static RacetimeAPI _instance;
    public static RacetimeAPI Instance
    {
        get
        {
            _instance ??= new RacetimeAPI();

            return _instance;
        }
    }

    public RacetimeAPI()
    {
        Authenticator = new RacetimeAuthenticator(new RTAuthentificationSettings());
        JoinRace = Join;
        CreateRace = Create;
    }

    public void Join(ITimerModel model, string id)
    {
        var channel = new RacetimeChannel(model.CurrentState, model, (RacetimeSettings)Settings);
        _ = new ChannelForm(channel, id, model.CurrentState.LayoutSettings.AlwaysOnTop);
    }

    public void Warn()
    {

    }

    public void Create(ITimerModel model)
    {
        Process.Start(GetUri(Properties.Resources.CREATE_RACE_ADDRESS).AbsoluteUri);
    }

    public IEnumerable<Race> Races { get; set; }

    internal RacetimeAuthenticator Authenticator { get; set; }

    public override string ProviderName => "racetime.gg";

    public override string Username => Authenticator.Identity?.Name;

    protected Uri GetUri(string subUri)
    {
        return new Uri(BaseUri, subUri);
    }

    public override void RefreshRacesListAsync()
    {
        Task.Factory.StartNew(RefreshRacesList);
    }

    protected void RefreshRacesList()
    {
        try
        {
            Races = GetRacesFromServer().ToArray();
            RacesRefreshedCallback?.Invoke(this);
        }
        catch { }
    }

    protected IEnumerable<Race> GetRacesFromServer()
    {
        var request = WebRequest.Create(new Uri(BaseUri.AbsoluteUri + racesEndpoint));
        request.Headers.Add("Authorization", "Bearer " + Authenticator.AccessToken);

        using WebResponse response = request.GetResponse();
        dynamic data = JSON.FromResponse(response);

        dynamic races = data.races;
        foreach (dynamic r in races)
        {
            Race raceObj;
            r.entrants = new List<dynamic>();
            raceObj = RTModelBase.Create<Race>(r);
            yield return raceObj;
        }

        yield break;
    }

    public override IEnumerable<IRaceInfo> GetRaces()
    {
        return Races;
    }
}
