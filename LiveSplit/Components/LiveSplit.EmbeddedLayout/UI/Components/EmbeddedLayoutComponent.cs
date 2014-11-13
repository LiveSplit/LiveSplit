using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.Options;
using LiveSplit.TimeFormatters;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveSplit.UI.Components
{
    public class EmbeddedLayoutComponent : IComponent
    {
        protected ComponentRendererComponent InternalComponent { get; set; }
        public DeltaSettings Settings { get; set; }

        public GraphicsCache Cache { get; set; }

        public float PaddingTop { get { return InternalComponent.PaddingTop; } }
        public float PaddingLeft { get { return InternalComponent.PaddingLeft; } }
        public float PaddingBottom { get { return InternalComponent.PaddingBottom; } }
        public float PaddingRight { get { return InternalComponent.PaddingRight; } }

        public IDictionary<string, Action> ContextMenuControls
        {
            get { return InternalComponent.ContextMenuControls; }
        }

        public EmbeddedLayoutComponent(LiveSplitState state)
        {
            Settings = new DeltaSettings()
            {
                CurrentState = state
            };
            InternalComponent = new ComponentRendererComponent();
            InternalComponent.VisibleComponents = new List<IComponent>() { new RunPrediction(state), new GraphCompositeComponent(state) };
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            var scaleFactor = (float)width / Math.Max(InternalComponent.OverallWidth, 1f);
            var oldMatrix = g.Transform;

            try
            {
                g.ScaleTransform(scaleFactor, scaleFactor);
                InternalComponent.DrawHorizontal(g, state, VerticalHeight / scaleFactor, clipRegion);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            g.Transform = oldMatrix;
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            var scaleFactor = (float)height / Math.Max(InternalComponent.OverallHeight, 1f);
            var oldMatrix = g.Transform;

            try
            {
                g.ScaleTransform(scaleFactor, scaleFactor);
                InternalComponent.DrawVertical(g, state, HorizontalWidth / scaleFactor, clipRegion);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            g.Transform = oldMatrix;
        }

        public float VerticalHeight
        {
            get { return /*from the settings*/100; }
        }

        public float MinimumWidth
        {
            get { return 20; }
        }

        public float HorizontalWidth
        {
            get { return /*from the settings*/100; }
        }

        public float MinimumHeight
        {
            get { return 20; }
        }

        public string ComponentName
        {
            get { return "Embedded Layout"; }
        }

        public Control GetSettingsControl(LayoutMode mode)
        {
            /*Settings.Mode = mode;
            return Settings;*/
            return null;
        }

        public void SetSettings(System.Xml.XmlNode settings)
        {
            //Settings.SetSettings(settings);
        }

        public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            return document.CreateElement("pr0n");
            //return Settings.GetSettings(document);
        }


        public void RenameComparison(string oldName, string newName)
        {
            InternalComponent.RenameComparison(oldName, newName);
        }


        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (invalidator != null)
                InternalComponent.Update(invalidator, state, width, height, mode == LayoutMode.Vertical ? LayoutMode.Horizontal : LayoutMode.Vertical);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
