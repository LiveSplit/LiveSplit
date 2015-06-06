using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LiveSplit.UI.Components
{
    public class ThinSeparatorComponent : IComponent
    {
        public float PaddingTop { get { return 0f; } }
        public float PaddingLeft { get { return 0f; } }
        public float PaddingBottom { get { return 0f; } }
        public float PaddingRight { get { return 0f; } }

        public bool LockToBottom { get; set; }

        public GraphicsCache Cache { get; set; }

        protected LineComponent Line { get; set; }

        public float VerticalHeight
        {
            get { return 1f; }
        }

        public float MinimumWidth
        {
            get { return 0; }
        }

        public float HorizontalWidth
        {
            get { return 1f; }
        }

        public float MinimumHeight
        {
            get { return 0; }
        }

        public ThinSeparatorComponent()
        {
            Line = new LineComponent(1, Color.White);
            Cache = new GraphicsCache();
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            var oldClip = g.Clip;
            var oldMatrix = g.Transform;
            var oldMode = g.SmoothingMode;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            g.Clip = new Region();
            Line.LineColor = state.LayoutSettings.ThinSeparatorsColor;
            var scale = g.Transform.Elements.First();
            var newHeight = Math.Max((int)(1f * scale + 0.5f), 1) / scale;
            Line.VerticalHeight = newHeight;
            if (LockToBottom)
                g.TranslateTransform(0, 1f - newHeight);
            Line.DrawVertical(g, state, width, clipRegion);
            g.Clip = oldClip;
            g.Transform = oldMatrix;
            g.SmoothingMode = oldMode;
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            var oldClip = g.Clip;
            var oldMatrix = g.Transform;
            var oldMode = g.SmoothingMode;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            g.Clip = new Region();
            Line.LineColor = state.LayoutSettings.ThinSeparatorsColor;
            var scale = g.Transform.Elements.First();
            var newWidth = Math.Max((int)(1f * scale + 0.5f), 1) / scale;
            if (LockToBottom)
                g.TranslateTransform(1f - newWidth, 0);
            Line.HorizontalWidth = newWidth;
            Line.DrawHorizontal(g, state, height, clipRegion);
            g.Clip = oldClip;
            g.Transform = oldMatrix;
            g.SmoothingMode = oldMode;
        }

        public string ComponentName
        {
            get { return "Thin Separator"; }
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
            Cache.Restart();
            Cache["LockToBottom"] = LockToBottom;

            if (invalidator != null && Cache.HasChanged)
                invalidator.Invalidate(0, 0, width, height);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
