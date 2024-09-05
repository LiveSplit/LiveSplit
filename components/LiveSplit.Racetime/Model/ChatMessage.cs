using System;
using System.Text.RegularExpressions;

namespace LiveSplit.Racetime.Model;

public abstract class ChatMessage : RTModelBase
{
    public abstract MessageType Type { get; }

    public virtual string Message
    {
        get
        {
            try
            {
                return Data.message;
            }
            catch
            {
                return "";
            }
        }
    }
    public virtual RacetimeUser User
    {
        get
        {
            try
            {
                return RTModelBase.Create<RacetimeUser>(Data.user);
            }
            catch
            {
                return null;
            }
        }
    }
    public DateTime Posted
    {
        get
        {
            try
            {
                if (Data.posted_at == null)
                {
                    return Received;
                }

                return DateTime.Parse(Data.posted_at);
            }
            catch
            {
                return DateTime.MaxValue;
            }
        }
    }
    public virtual bool Highlight
    {
        get
        {
            try
            {
                return Data.highlight;
            }
            catch
            {
                return false;
            }
        }
    }
    public bool IsSystem
    {
        get
        {
            try
            {
                return Data.is_system;
            }
            catch
            {
                return false;
            }
        }
    }
}

public class LiveSplitMessage : ChatMessage
{
    public override MessageType Type => MessageType.LiveSplit;

    public override RacetimeUser User => RacetimeUser.LiveSplit;

    public static LiveSplitMessage Create(string msg, bool important)
    {
        var dataroot = new
        {
            message = msg,
            user = RacetimeUser.LiveSplit,
            posted_at = DateTime.Now.ToString(),
            highlight = important,
            is_system = true
        };
        return Create<LiveSplitMessage>(dataroot);
    }
}
public class SystemMessage : ChatMessage
{
    public override MessageType Type => MessageType.System;

    public override string Message
    {
        get
        {
            try
            {
                return Data.message_plain;
            }
            catch
            {
                return Data.message;
            }
        }
    }

    public override RacetimeUser User => RacetimeUser.System;

    public bool IsFinishingMessage => Regex.IsMatch(Message, "(finish|forfeit|comment|done)", RegexOptions.IgnoreCase);
}
public class BotMessage : ChatMessage
{
    public override MessageType Type => MessageType.Bot;

    public override string Message
    {
        get
        {
            try
            {
                return Data.message_plain;
            }
            catch
            {
                return Data.message;
            }
        }
    }

    public string BotName
    {
        get
        {
            try
            {
                return Data.bot;
            }
            catch
            {
                return null;
            }
        }
    }

    public override RacetimeUser User => RacetimeUser.Bot;
}

public class UserMessage : ChatMessage
{
    public override MessageType Type => MessageType.User;

    public override string Message
    {
        get
        {
            try
            {
                return Data.message_plain;
            }
            catch
            {
                return Data.message;
            }
        }
    }
}
public class ErrorMessage : ChatMessage
{
    public override MessageType Type => MessageType.Error;

    public override bool Highlight => true;

    public override RacetimeUser User => RacetimeUser.System;

    public override string Message
    {
        get
        {
            try
            {
                string msg = "";
                foreach (dynamic s in Data.errors)
                {
                    msg += s + " ";
                }

                return msg;
            }
            catch
            {
                return "Error in the error message";
            }
        }
    }
}
public class SplitMessage : ChatMessage
{
    public override MessageType Type => MessageType.SplitUpdate;
    public override string Message
    {
        get
        {
            try
            {
                return Data.message_plain;
            }
            catch
            {
                return Data.message;
            }
        }
    }

    public SplitUpdate SplitUpdate => RTModelBase.Create<SplitUpdate>(Data);
}
public class RaceMessage : ChatMessage
{
    public override MessageType Type => MessageType.Race;

    public override string Message
    {
        get
        {
            try
            {
                return Data.message_plain;
            }
            catch
            {
                return Data.message;
            }
        }
    }

    public Race Race => RTModelBase.Create<Race>(Data);
}
