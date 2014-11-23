using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LiveSplit.UI.Components
{
    public class LineComponent : IComponent
    {
        public float PaddingTop { get { return 0f; } }
        public float PaddingLeft { get { return 0f; } }
        public float PaddingBottom { get { return 0f; } }
        public float PaddingRight { get { return 0f; } }

        public float VerticalHeight { get; set; }
        public float HorizontalWidth { get; set; }
        public Color LineColor { get; set; }

        public LineComponent (int size, Color lineColor)
        {
            VerticalHeight = size;
            HorizontalWidth = size;
            LineColor = lineColor;
        }


        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            g.FillRectangle(new SolidBrush(LineColor), 0.0f, 0.0f, width, VerticalHeight);
        }

        public string ComponentName
        {
            get { throw new NotSupportedException(); }
        }


        public float MinimumWidth { get { return 0; } }


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

        public float MinimumHeight
        {
            get { throw new NotImplementedException(); }
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            g.FillRectangle(new SolidBrush(LineColor), 0.0f, 0.0f, HorizontalWidth, height);
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            invalidator.Invalidate(0, 0, width, height);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
