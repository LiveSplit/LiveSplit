using System;

namespace LiveSplit.Racetime.Model;

public abstract class RTModelBase
{
    public DateTime Received { get; set; }

    public static T Create<T>(dynamic dataroot) where T : RTModelBase, new()
    {
        if (dataroot == null)
        {
            return null;
        }

        var item = new T
        {
            Received = DateTime.Now,
            Data = dataroot
        };

        return item;
    }

    public dynamic Data { get; set; }

    public RTModelBase()
    {

    }
}
