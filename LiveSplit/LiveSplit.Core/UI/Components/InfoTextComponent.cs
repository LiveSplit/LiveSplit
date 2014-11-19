using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LiveSplit.UI.Components
{
    public class InfoTextComponent : IComponent
    {
        public String InformationName { get { return NameLabel.Text; } set { NameLabel.Text = value; } }
        public String InformationValue { get { return ValueLabel.Text; } set { ValueLabel.Text = value; } }

        public ICollection<String> AlternateNameText { get { return NameLabel.AlternateText; } set { NameLabel.AlternateText = value; } }
        //public ICollection<String> AlternateValueText { get { return ValueLabel.AlternateText; } set { ValueLabel.AlternateText = value; } }

        public SimpleLabel NameLabel { get; protected set; }
        public SimpleLabel ValueLabel { get; protected set; }

        public String LongestString { get; set; }
        protected SimpleLabel NameMeasureLabel { get; set; }

        public float PaddingTop { get; set; }
        public float PaddingLeft { get { return 7f; } }
        public float PaddingBottom { get; set; }
        public float PaddingRight { get { return 7f; } }

        public bool DisplayTwoRows { get; set; }

        public float VerticalHeight { get; set; }

        public float MinimumWidth
        {
            get { return 20; }
        }

        public float HorizontalWidth
        {
            get { return Math.Max(NameMeasureLabel.ActualWidth, ValueLabel.ActualWidth) + 10; }
        }

        public float MinimumHeight { get; set; }

        public InfoTextComponent(String informationName, String informationValue)
        {
            NameLabel = new SimpleLabel()
            {
                HorizontalAlignment = StringAlignment.Near,
                Text = informationName
            };
            ValueLabel = new SimpleLabel()
            {
                HorizontalAlignment = StringAlignment.Far,
                Text = informationValue
            };
            NameMeasureLabel = new SimpleLabel();
            VerticalHeight = 31;
            LongestString = "";
        }

        public virtual void PrepareDraw(LiveSplitState state, LayoutMode mode)
        {
            NameMeasureLabel.Font = state.LayoutSettings.TextFont;
            ValueLabel.Font = state.LayoutSettings.TextFont;
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

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            if (DisplayTwoRows)
            {
                VerticalHeight = 0.9f * (g.MeasureString("A", ValueLabel.Font).Height + g.MeasureString("A", NameLabel.Font).Height);
                PaddingTop = PaddingBottom = 0;
                DrawTwoRows(g, state, width, VerticalHeight);
            }
            else
            {
                VerticalHeight = 31;
                NameLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
                ValueLabel.ShadowColor = state.LayoutSettings.ShadowsColor;

                var textHeight = 0.75f * Math.Max(g.MeasureString("A", ValueLabel.Font).Height, g.MeasureString("A", NameLabel.Font).Height);
                PaddingTop = Math.Max(0, (VerticalHeight - textHeight) / 2f);
                PaddingBottom = PaddingTop;

                NameMeasureLabel.Text = LongestString;
                NameMeasureLabel.SetActualWidth(g);
                ValueLabel.SetActualWidth(g);

                NameLabel.Width = width - ValueLabel.ActualWidth - 10;
                NameLabel.Height = VerticalHeight;
                NameLabel.X = 5;
                NameLabel.Y = 0;

                ValueLabel.Width = ValueLabel.IsMonospaced ? width - 12 : width - 10;
                ValueLabel.Height = VerticalHeight;
                ValueLabel.Y = 0;
                ValueLabel.X = 5;

                PrepareDraw(state, LayoutMode.Vertical);

                NameLabel.Draw(g);
                ValueLabel.Draw(g);
            }
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            DrawTwoRows(g, state, HorizontalWidth, height);
        }

        protected void DrawTwoRows(Graphics g, LiveSplitState state, float width, float height)
        {
            NameLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
            ValueLabel.ShadowColor = state.LayoutSettings.ShadowsColor;

            if (InformationName != null && LongestString != null && InformationName.Length > LongestString.Length)
            {
                LongestString = InformationName;
                NameMeasureLabel.Text = LongestString;
            }
            NameMeasureLabel.Text = LongestString;
            NameMeasureLabel.Font = state.LayoutSettings.TextFont;
            NameMeasureLabel.SetActualWidth(g);

            MinimumHeight = 0.85f * (g.MeasureString("A", ValueLabel.Font).Height + g.MeasureString("A", NameLabel.Font).Height);
            NameLabel.Width = width - 10;
            NameLabel.Height = height;
            NameLabel.X = 5;
            NameLabel.Y = 0;

            ValueLabel.Width = ValueLabel.IsMonospaced ? width - 12 : width - 10;
            ValueLabel.Height = height;
            ValueLabel.Y = 0;
            ValueLabel.X = 5;

            PrepareDraw(state, LayoutMode.Horizontal);

            NameLabel.Draw(g);
            ValueLabel.Draw(g);
        }

        public string ComponentName
        {
            get { throw new NotSupportedException(); }
        }


        public Control GetSettingsControl(LayoutMode mode)
        {
            throw new NotImplementedException();
        }

        public void SetSettings(System.Xml.XmlNode settings)
        {
            throw new NotImplementedException();
        }

        public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            throw new NotImplementedException();
        }

        public string UpdateName
        {
            get { throw new NotSupportedException(); }
        }

        public string XMLURL
        {
            get { throw new NotSupportedException(); }
        }

        public string UpdateURL
        {
            get { throw new NotSupportedException(); }
        }

        public Version Version
        {
            get { throw new NotSupportedException(); }
        }

        public IDictionary<string, Action> ContextMenuControls
        {
            get { return null; }
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
        }

        public void Dispose()
        {
        }
    }
}
