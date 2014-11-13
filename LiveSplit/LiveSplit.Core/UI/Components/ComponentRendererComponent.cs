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
    public class ComponentRendererComponent : ComponentRenderer, IComponent
    {
        public float VerticalHeight { get { return OverallHeight; } }
        public float HorizontalWidth { get { return OverallWidth; } }

        public float PaddingTop { get { return base.VisibleComponents.Count() > 0 ? base.VisibleComponents.First().PaddingTop : 0; } }
        public float PaddingLeft { get { return base.VisibleComponents.Count() > 0 ? base.VisibleComponents.First().PaddingLeft : 0; } }
        public float PaddingBottom { get { return base.VisibleComponents.Count() > 0 ? base.VisibleComponents.Last().PaddingBottom : 0; } }
        public float PaddingRight { get { return base.VisibleComponents.Count() > 0 ? base.VisibleComponents.Last().PaddingRight : 0; } }
       
        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            Render(g, state, width, 0, LayoutMode.Vertical, clipRegion);
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            Render(g, state, 0, height, LayoutMode.Horizontal, clipRegion);
        }

        public string ComponentName
        {
            get { throw new NotSupportedException(); }
        }


        public Control GetSettingsControl(LayoutMode mode)
        {
            throw new NotSupportedException();
        }

        public void SetSettings(System.Xml.XmlNode settings)
        {
            throw new NotSupportedException();
        }

        public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            throw new NotSupportedException();
        }

        public IDictionary<string, Action> ContextMenuControls
        {
            get { return base.VisibleComponents.Select(x => x.ContextMenuControls).Where(x => x != null).SelectMany(x => x).ToDictionary(x => x.Key, x => x.Value); }
        }

        public void RenameComparison(string oldName, string newName)
        {
            foreach (var component in VisibleComponents)
                RenameComparison(oldName, newName);
        }

        public void Dispose()
        {
            foreach (var component in VisibleComponents)
                component.Dispose();
        }
    }
}
