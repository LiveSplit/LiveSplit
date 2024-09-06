using System;
using System.Linq;

namespace LiveSplit.Racetime.Model;

public class RacetimeUser : RTModelBase
{
    private int nameChcecksum = -1;
    public int Class
    {
        get
        {
            if (nameChcecksum == -1)
            {
                nameChcecksum = Name.Sum(x => x);
            }

            return nameChcecksum;
        }
    }
    public override bool Equals(object obj)
    {
        return Name.ToLower() == ((RacetimeUser)obj)?.Name?.ToLower();
    }
    public override int GetHashCode()
    {
        return Name.ToLower().GetHashCode();
    }
    public string ID => Data.id;
    public string FullName => Data.full_name;
    public string Name => Data.name ?? "";
    public string TwitchChannel => Data.twitch_channel;
    public string TwitchName => Data.twitch_name;
    public UserRole Role
    {
        get
        {
            UserRole r = UserRole.Regular;

            if (Data.user.flair == null)
            {
                return UserRole.Unknown;
            }

            string[] flairs = Data.user.flair.ToString().Split(' ');
            foreach (string f in flairs)
            {
                switch (f)
                {
                    case "staff": r |= UserRole.Staff; break;
                    case "moderator": r |= UserRole.Moderator; break;
                    case "monitor": r |= UserRole.Monitor; break;
                    case "bot": r |= UserRole.Bot; break;
                    case "system": r |= UserRole.System; break;
                    case "anonymous": r |= UserRole.Anonymous; break;
                }
            }

            return r;
        }
    }
    public UserStatus Status
    {
        get
        {
            UserStatus s = UserStatus.Unknown;
            if (Data.status == null)
            {
                return UserStatus.Unknown;
            }

            s = Data.status.value switch
            {
                "not_ready" => UserStatus.NotReady,
                "ready" => UserStatus.Ready,
                "done" => UserStatus.Finished,
                "in_progress" => UserStatus.Racing,
                "dnf" => UserStatus.Forfeit,
                "dq" => UserStatus.Disqualified,
                _ => UserStatus.Unknown,
            };
            return s;
        }
    }

    public DateTime FinishedAt
    {
        get
        {
            try
            {
                if (DateTime.TryParse(Data.finished_at, out DateTime dt))
                {
                    return dt.ToUniversalTime();
                }

                return DateTime.MaxValue;
            }
            catch
            {
                return DateTime.MaxValue;
            }
        }
    }
    public bool HasFinished => FinishedAt != DateTime.MaxValue;
    public int Place
    {
        get
        {
            try
            {
                if (Data.place != null)
                {
                    return Data.place;
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }
    }
    public string PlaceOrdinal
    {
        get
        {
            try
            {
                return Data.place_ordinal;
            }
            catch
            {
                return null;
            }
        }
    }
    public string Comment
    {
        get
        {
            try
            {
                return Data.comment;
            }
            catch
            {
                return null;
            }
        }
    }
    public bool IsLive
    {
        get
        {
            try
            {
                return Data.stream_live;
            }
            catch
            {
                return false;
            }
        }
    }
    public bool StreamOverride
    {
        get
        {
            try
            {
                return Data.stream_override;
            }
            catch
            {
                return false;
            }
        }
    }

    public static RacetimeUser System = CreateBot("RaceBot", "bot staff moderator monitor");
    public static RacetimeUser Bot = CreateBot("Bot", "bot staff moderator monitor");
    public static RacetimeUser LiveSplit = CreateBot("LiveSplit", "system staff moderator monitor");
    public static RacetimeUser Anonymous = CreateBot("Anonymous", "anonymous");

    public static RacetimeUser CreateBot(string botname, string flairs)
    {
        var dataroot = new
        {
            name = botname,
            id = botname,
            flair = flairs,
        };
        return Create<RacetimeUser>(dataroot);
    }
}
