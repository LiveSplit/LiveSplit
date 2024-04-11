using LiveSplit.TimeFormatters;
using System;
using System.Drawing;

namespace LiveSplit.UI.Components
{
    public class InfoTimeComponent : InfoTextComponent
    {
        private TimeSpan? timeValue;
        public TimeSpan? TimeValue 
        {
            get 
            { 
                return timeValue; 
            } 
            set
            {
                timeValue = value;
                InformationValue = Formatter.Format(timeValue);
            }
        }

        private ITimeFormatter formatter;
        public ITimeFormatter Formatter
        {
            get
            {
                return formatter;
            }
            set
            {
                if (value != null && value != formatter)
                    InformationValue = value.Format(timeValue);
                formatter = value;
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
}
