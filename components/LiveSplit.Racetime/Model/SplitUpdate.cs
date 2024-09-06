using System;

using LiveSplit.Model;

namespace LiveSplit.Racetime.Model;

public class SplitUpdate : RTModelBase
{
    public string SplitName => Data.split_name;

    public TimeSpan? SplitTime
    {
        get
        {
            if (Data.split_time == "-")
            {
                return null;
            }

            return TimeSpanParser.Parse(Data.split_time);
        }
    }

    public bool IsUndo => Data.is_undo;

    public bool IsFinish => Data.is_finish;

    public string UserID => Data.user_id;
}
