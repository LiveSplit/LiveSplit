using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using LiveSplit.Model;

namespace LiveSplit.Racetime.Model;

public class Race : RTModelBase, IRaceInfo
{
    public static dynamic EntrantToUserConverter(dynamic e)
    {

        return new
        {
            id = e.user.id,
            name = e.user.name,
            full_name = e.user.full_name,
            flair = e.user.flair,
            twitch_name = e.user.twitch_name,
            twitch_channel = e.user.twitch_channel,
            discriminator = e.user.discriminator,
            status = e.status,
            place = e.place,
            place_ordinal = e.place_ordinal,
            finish_time = e.finish_time,
            finished_at = e.finished_at,
            start_delay = e.start_delay,
            stream_live = e.stream_live,
            comment = e.comment,
            stream_override = e.stream_override
        };
    }
    public bool AllowNonEntrantChat => false;
    public bool AllowMidraceChat => Data.allow_midrace_chat;
    public bool AllowComments => Data.allow_comments;
    public string GameSlug => Id[..Id.IndexOf('/')];
    public string Name => Data.name;
    public string Goal
    {
        get
        {
            try
            {
                return Data.goal.name;
            }
            catch
            {
                return null;
            }
        }
    }
    public string Info
    {
        get
        {
            try
            {
                return Data.info;
            }
            catch
            {
                return null;
            }
        }
    }
    public TimeSpan StartDelay
    {
        get
        {
            try
            {
                TimeSpan ts = XmlConvert.ToTimeSpan(Data.start_delay);
                return ts;
            }
            catch
            {
                return TimeSpan.Zero;
            }
        }
    }
    public int NumEntrants => Data.entrants_count;
    public IEnumerable<RacetimeUser> Entrants
    {
        get
        {
            foreach (dynamic e in Data.entrants)
            {
                yield return RTModelBase.Create<RacetimeUser>(EntrantToUserConverter(e));
            }
        }
    }
    public string ChannelName => Id[(Id.IndexOf('/') + 1)..];
    public RaceState State
    {
        get
        {
            return Data.status.value switch
            {
                "open" => RaceState.Open,
                "invitational" => RaceState.OpenInviteOnly,
                "pending" => RaceState.Starting,
                "in_progress" => RaceState.Started,
                "finished" => RaceState.Ended,
                "cancelled" => RaceState.Cancelled,
                _ => RaceState.Unknown,
            };
        }
    }
    public DateTime StartedAt
    {
        get
        {
            try
            {
                if (Data.started_at == null)
                {
                    return DateTime.MaxValue;
                }

                return DateTime.Parse(Data.started_at).ToUniversalTime();
            }
            catch
            {
                return DateTime.MaxValue;
            }
        }
    }

    public DateTime OpenedAt
    {
        get
        {
            try
            {
                if (Data.opened_at == null)
                {
                    return DateTime.MaxValue;
                }

                return DateTime.Parse(Data.opened_at);
            }
            catch
            {
                return DateTime.MaxValue;
            }
        }
    }
    public RacetimeUser OpenedBy => RTModelBase.Create<RacetimeUser>(Data.opened_by);

    public int Finishes => Data.entrants_count_finished;
    public int Forfeits => Data.entrants_count_inactive;

    public string GameId => Data.category.slug;

    public string GameName => Data.category.name;

    public string Id => Data.name;

    public IEnumerable<string> LiveStreams => Entrants.Where(x => x.Status != UserStatus.Forfeit && x.Status != UserStatus.Disqualified && !x.HasFinished).Select(x => x.TwitchName);

    public int Starttime => StartedAt == DateTime.MaxValue ? 0 : (int)(StartedAt - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

    int IRaceInfo.State => (State is RaceState.Open or RaceState.OpenInviteOnly) ? 1 : (State == RaceState.Started ? 3 : 42);

    public bool IsParticipant(string username)
    {
        return true;//Entrants.Any(x => x.Name.ToLower() == username?.ToLower());
    }
}
