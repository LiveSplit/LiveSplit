using System;
using System.Collections.Generic;

using LiveSplit.Options;

namespace LiveSplit.UI;

public class GraphicsCache
{
    protected Dictionary<string, object> FlagDirectory { get; set; }
    public bool HasChanged { get; protected set; }

    public object this[string flag]
    {
        get => FlagDirectory[flag];
        set
        {
            if (!FlagDirectory.ContainsKey(flag))
            {
                try
                {
                    FlagDirectory.Add(flag, value);
                    HasChanged = true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    HasChanged = true;
                }
            }
            else if (!Equals(FlagDirectory[flag], value))
            {
                FlagDirectory[flag] = value;
                HasChanged = true;
            }
        }
    }

    public GraphicsCache()
    {
        FlagDirectory = [];
    }

    public void Restart()
    {
        HasChanged = false;
    }
}
