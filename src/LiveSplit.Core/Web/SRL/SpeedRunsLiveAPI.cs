﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

using LiveSplit.Model;
using LiveSplit.UI.Components;
using LiveSplit.Web.Share;

namespace LiveSplit.Web.SRL;

public class SpeedRunsLiveAPI : RaceProviderAPI
{
    protected static readonly SpeedRunsLiveAPI _Instance = new();

    public static SpeedRunsLiveAPI Instance => _Instance;
    public static readonly Uri BaseUri = new("http://api.speedrunslive.com:81/");

    protected IEnumerable<SRLRaceInfo> racesList;
    protected IEnumerable<dynamic> gameList;
    protected IList<string> gameNames;

    protected SpeedRunsLiveAPI() { }

    protected Uri GetUri(string subUri)
    {
        return new Uri(BaseUri, subUri);
    }

    public IEnumerable<dynamic> GetGameList()
    {
        gameList ??= (IEnumerable<dynamic>)JSON.FromUri(GetUri("games")).games;

        return gameList;
    }

    public IEnumerable<string> GetGameNames()
    {
        if (gameNames == null)
        {
            static string map(dynamic x)
            {
                return x.name;
            }

            gameNames = GetGameList().Select(map).ToList();
        }

        return gameNames;
    }

    public IEnumerable<string> GetCategories(string gameID)
    {
        if (string.IsNullOrEmpty(gameID))
        {
            return new string[0];
        }

        return ((IEnumerable<dynamic>)JSON.FromUri(GetUri("goals/" + gameID + "?season=0")).goals).Select(x => (string)x.name);
    }

    public string GetGameIDFromName(string name)
    {
        bool map(dynamic x)
        {
            return x.name == name;
        }

        dynamic gameID = GetGameList().FirstOrDefault(map);
        if (gameID != null)
        {
            return gameID.abbrev;
        }

        return null;
    }

    public IEnumerable<dynamic> GetEntrants(string raceID)
    {
        dynamic race = GetRace(raceID);
        return race.entrants;
    }

    public dynamic GetRace(string raceID)
    {
        IEnumerable<IRaceInfo> races = GetRaces();
        return races.First(x => x.Id == raceID);
    }

    public override IEnumerable<IRaceInfo> GetRaces()
    {
        if (racesList == null)
        {
            RefreshRacesList();
        }

        return racesList;
    }

    private void SpeedRunsLiveAPI_Elapsed(object sender, ElapsedEventArgs e)
    {
        try
        {
            RefreshRacesList();
        }
        catch { }
    }

    public override void RefreshRacesListAsync()
    {
        Task.Factory.StartNew(RefreshRacesList)
            .ContinueWith((raceItem) => { }, TaskContinuationOptions.OnlyOnFaulted);
    }

    public void RefreshRacesList()
    {
        List<SRLRaceInfo> infoList = [];
        foreach (dynamic race in JSON.FromUri(GetUri("races")).races)
        {
            infoList.Add(new SRLRaceInfo(race));
        }

        racesList = infoList;
        RacesRefreshedCallback?.Invoke(this);
    }

    public override string ProviderName => "SRL";

    public override string Username
    {
        get
        {
            ShareSettings.Default.Reload();
            return WebCredentials.SpeedRunsLiveIRCCredentials.Username;
        }
    }
}

