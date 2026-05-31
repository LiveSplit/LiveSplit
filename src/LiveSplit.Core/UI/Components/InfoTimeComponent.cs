using LiveSplit.TimeFormatters;
using System;
using System.Drawing;

namespace LiveSplit.UI.Components;

public class InfoTimeComponent : InfoTextComponent
{
    public TimeSpan? TimeValue
    {
        get;
        set
        {
            field = value;
            InformationValue = Formatter.Format(field);
        }
    }

    public ITimeFormatter Formatter
    {
        get;
        set
        {
            if (value != null && value != field)
            {
                InformationValue = value.Format(TimeValue);
            }

            field = value;
        }
    }

    public override void PrepareDraw(Model.LiveSplitState state, LayoutMode mode)
    {
        ValueLabel.IsMonospaced = true;
        ValueLabel.Font = state.LayoutSettings.TimesFont;
        NameMeasureLabel.Font = state.LayoutSettings.TextFont;
        NameLabel.Font = state.LayoutSettings.TextFont;
        if (mode == LayoutMode.Vertical)
        {
            NameLabel.VerticalAlignment = StringAlignment.Center;
            ValueLabel.VerticalAlignment = StringAlignment.Center;
        }
        else
        {
            NameLabel.VerticalAlignment = StringAlignment.Near;
            ValueLabel.VerticalAlignment = StringAlignment.Far;
        }
    }

    public InfoTimeComponent(string informationName, TimeSpan? timeValue, ITimeFormatter formatter)
        : base(informationName, "")
    {
        Formatter = formatter;
        TimeValue = timeValue;
    }
}
