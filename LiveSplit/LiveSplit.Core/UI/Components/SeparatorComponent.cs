using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveSplit.UI.Components
{
    public class SeparatorComponent : IComponent
    {
        public float PaddingTop { get { return 0f; } }
        public float PaddingLeft { get { return 0f; } }
        public float PaddingBottom { get { return 0f; } }
        public float PaddingRight { get { return 0f; } }

        public float DisplayedSize { get; set; }
        public bool UseSeparatorColor { get; set; }
        public bool LockToBottom { get; set; }

        protected LineComponent Line { get; set; }

        public GraphicsCache Cache { get; set; }

        public float VerticalHeight
        {
            get { return 2f; }
        }

        public float MinimumWidth
        {
            get { return 0; }
        }

        public float HorizontalWidth
        {
            get { return 2f; }
        }

        public float MinimumHeight
        {
            get { return 0; }
        }

        public SeparatorComponent()
        {
            Line = new LineComponent(2, Color.White);
            DisplayedSize = 2f;
            UseSeparatorColor = true;
            LockToBottom = false;
            Cache = new GraphicsCache();
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            if (DisplayedSize > 0)
            {
                var oldClip = g.Clip;
                var oldMatrix = g.Transform;
                var oldMode = g.SmoothingMode;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
                g.Clip = new Region();
                Line.LineColor = UseSeparatorColor ? state.LayoutSettings.SeparatorsColor : state.LayoutSettings.ThinSeparatorsColor;
                var scale = g.Transform.Elements.First();
                var newHeight = Math.Max((int)(DisplayedSize * scale + 0.5f), 1) / scale;
                Line.VerticalHeight = newHeight;
                if (LockToBottom)
                    g.TranslateTransform(0, 2f - newHeight);
                else if (DisplayedSize > 1)
                    g.TranslateTransform(0, (2f - newHeight) / 2f);
                Line.DrawVertical(g, state, width, clipRegion);
                g.Clip = oldClip;
                g.Transform = oldMatrix;
                g.SmoothingMode = oldMode;
            }
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            if (DisplayedSize > 0)
            {
                var oldClip = g.Clip;
                var oldMatrix = g.Transform;
                var oldMode = g.SmoothingMode;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
                g.Clip = new Region();
                Line.LineColor = UseSeparatorColor ? state.LayoutSettings.SeparatorsColor : state.LayoutSettings.ThinSeparatorsColor;
                var scale = g.Transform.Elements.First();
                var newWidth = Math.Max((int)(DisplayedSize * scale + 0.5f), 1) / scale;
                if (LockToBottom)
                    g.TranslateTransform(2f - newWidth, 0);
                else if (DisplayedSize > 1)
                    g.TranslateTransform((2f - newWidth) / 2f, 0);
                Line.HorizontalWidth = newWidth;
                Line.DrawHorizontal(g, state, height, clipRegion);
                g.Clip = oldClip;
                g.Transform = oldMatrix;
                g.SmoothingMode = oldMode;
            }
        }

        public string ComponentName
        {
            get { return "----------------------------------------------------------------------------"; }
        }


        public Control GetSettingsControl(LayoutMode mode)
        {
            return null;
        }

        public void SetSettings(System.Xml.XmlNode settings)
        {
        }


        public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            return document.CreateElement("SeparatorSettings");
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


        public void RenameComparison(string oldName, string newName)
        {
        }


        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            Cache.Restart();
            Cache["DisplayedSize"] = DisplayedSize;
            Cache["UseSeparatorColor"] = UseSeparatorColor;
            Cache["LockToBottom"] = LockToBottom;

            if (invalidator != null && Cache.HasChanged)
            {
                if (mode == LayoutMode.Vertical)
                    invalidator.Invalidate(0, -1, width, height + 2);
                else
                    invalidator.Invalidate(-1, 0, width + 2, height);
            }
        }

        public void Dispose()
        {
        }
    }
}
