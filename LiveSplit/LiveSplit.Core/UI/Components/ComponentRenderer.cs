using LiveSplit.Model;
using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LiveSplit.UI.Components
{
    public class ComponentRenderer
    {
        public IEnumerable<IComponent> VisibleComponents { get; set; }

        public float OverallHeight = 10f;
        public float OverallWidth = 10f;

        public float MinimumWidth
        {
            get
            {
                return VisibleComponents.Count() == 0 ? 0 : VisibleComponents.Max(x => x.MinimumWidth);
            }
        }

        public float MinimumHeight
        {
            get
            {
                return VisibleComponents.Count() == 0 ? 0 : VisibleComponents.Max(x => x.MinimumHeight);
            }
        }

        protected bool errorInComponent = false;

        private void DrawVerticalComponent(int index, Graphics g, LiveSplitState state, float width, float height, Region clipRegion)
        {
            var component = VisibleComponents.ElementAt(index);
            var paddingAbove = index > 0 ? VisibleComponents.ElementAt(index - 1).PaddingBottom : 0f;
            var paddingBeneath = index < VisibleComponents.Count() - 1 ? VisibleComponents.ElementAt(index + 1).PaddingTop : 0f;
            var topPadding = Math.Min(paddingAbove, component.PaddingTop) / 2f;
            var bottomPadding = Math.Min(paddingBeneath, component.PaddingBottom) / 2f;
            g.IntersectClip(new RectangleF(0, topPadding, width, component.VerticalHeight - topPadding - bottomPadding));

            var scale = g.Transform.Elements.First();
            var offset = component.VerticalHeight * scale < 3 ? 1 : 0;

            if (clipRegion.IsVisible(new RectangleF(
                g.Transform.OffsetX,
                -offset + g.Transform.OffsetY - topPadding * scale,
                width,
                offset + scale * (component.VerticalHeight + bottomPadding))))
                component.DrawVertical(g, state, width, clipRegion);
            g.TranslateTransform(0.0f, component.VerticalHeight - bottomPadding * 2f);
        }

        private void DrawHorizontalComponent(int index, Graphics g, LiveSplitState state, float width, float height, Region clipRegion)
        {
            var component = VisibleComponents.ElementAt(index);
            var paddingToLeft = index > 0 ? VisibleComponents.ElementAt(index - 1).PaddingRight : 0f;
            var paddingToRight = index < VisibleComponents.Count() - 1 ? VisibleComponents.ElementAt(index + 1).PaddingLeft : 0f;
            var leftPadding = Math.Min(paddingToLeft, component.PaddingLeft) / 2f;
            var rightPadding = Math.Min(paddingToRight, component.PaddingRight) / 2f;
            g.IntersectClip(new RectangleF(leftPadding, 0, component.HorizontalWidth - leftPadding - rightPadding, height));

            var scale = g.Transform.Elements.First();
            var offset = component.HorizontalWidth * scale < 3 ? 1 : 0;

            if (clipRegion.IsVisible(new RectangleF(
                -offset + g.Transform.OffsetX - leftPadding * scale,
                g.Transform.OffsetY,
                offset + scale * (component.HorizontalWidth + rightPadding),
                height)))
                component.DrawHorizontal(g, state, height, clipRegion);
            g.TranslateTransform(component.HorizontalWidth - rightPadding * 2f, 0.0f);
        }

        protected float GetHeightVertical(int index)
        {
            var component = VisibleComponents.ElementAt(index);
            var paddingBeneath = index < VisibleComponents.Count() - 1 ? VisibleComponents.ElementAt(index + 1).PaddingTop : 0f;
            var bottomPadding = Math.Min(paddingBeneath, component.PaddingBottom) / 2f;
            return component.VerticalHeight - bottomPadding * 2f;
        }

        protected float GetWidthHorizontal(int index)
        {
            var component = VisibleComponents.ElementAt(index);
            var paddingToRight = index < VisibleComponents.Count() - 1 ? VisibleComponents.ElementAt(index + 1).PaddingLeft : 0f;
            var rightPadding = Math.Min(paddingToRight, component.PaddingRight) / 2f;
            return component.HorizontalWidth - rightPadding * 2f;
        }

        public void CalculateOverallHeight(LayoutMode mode)
        {
            var totalSize = 0f;
            var index = 0;
            foreach (var component in VisibleComponents)
            {
                if (mode == LayoutMode.Vertical)
                    totalSize += GetHeightVertical(index);
                else
                    totalSize += GetWidthHorizontal(index);
                index++;
            }

            if (mode == LayoutMode.Vertical)
            {
                OverallHeight = totalSize;
                OverallWidth = VisibleComponents.Aggregate(0.0f, (x, y) => x + y.HorizontalWidth);
            }
            else
            {
                OverallWidth = totalSize;
                OverallHeight = VisibleComponents.Aggregate(0.0f, (x, y) => x + y.VerticalHeight);
            }
        }

        public void Render(Graphics g, LiveSplitState state, float width, float height, LayoutMode mode, Region clipRegion)
        {
            if (!errorInComponent)
            {
                try
                {
                    var clip = g.Clip;
                    var transform = g.Transform;
                    var crashedComponents = new List<IComponent>();
                    var index = 0;
                    var totalSize = 0f;
                    foreach (var component in VisibleComponents)
                    {
                        try
                        {
                            g.Clip = clip;
                            if (mode == LayoutMode.Vertical)
                                DrawVerticalComponent(index, g, state, width, height, clipRegion);
                            else
                                DrawHorizontalComponent(index, g, state, width, height, clipRegion);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                            crashedComponents.Add(component);
                            errorInComponent = true;
                        }
                        index++;
                    }

                    if (crashedComponents.Count > 0)
                    {
                        var remainingComponents = VisibleComponents.ToList();
                        crashedComponents.ForEach(x => remainingComponents.Remove(x));
                        //crashedComponents.ForEach(x => MessageBox.Show(String.Format("The component {0} crashed.", x.ComponentName), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error));
                        VisibleComponents = remainingComponents;
                    }
                    g.Transform = transform;
                    g.Clip = clip;
                }
                finally
                {
                    errorInComponent = false;
                }
            }
            CalculateOverallHeight(mode);
        }

        protected void InvalidateVerticalComponent(int index, LiveSplitState state, IInvalidator invalidator, float width, float height, float scaleFactor)
        {
            var component = VisibleComponents.ElementAt(index);
            var paddingAbove = index > 0 ? VisibleComponents.ElementAt(index - 1).PaddingBottom : 0f;
            var paddingBeneath = index < VisibleComponents.Count() - 1 ? VisibleComponents.ElementAt(index + 1).PaddingTop : 0f;
            var topPadding = Math.Min(paddingAbove, component.PaddingTop) / 2f;
            var bottomPadding = Math.Min(paddingBeneath, component.PaddingBottom) / 2f;
            var totalHeight = scaleFactor * (component.VerticalHeight - topPadding - bottomPadding);
            component.Update(invalidator, state, width, totalHeight, LayoutMode.Vertical);
            invalidator.Transform.Translate(0.0f, totalHeight);
        }

        protected void InvalidateHorizontalComponent(int index, LiveSplitState state, IInvalidator invalidator, float width, float height, float scaleFactor)
        {
            var component = VisibleComponents.ElementAt(index);
            var paddingToLeft = index > 0 ? VisibleComponents.ElementAt(index - 1).PaddingRight : 0f;
            var paddingToRight = index < VisibleComponents.Count() - 1 ? VisibleComponents.ElementAt(index + 1).PaddingLeft : 0f;
            var leftPadding = Math.Min(paddingToLeft, component.PaddingLeft) / 2f;
            var rightPadding = Math.Min(paddingToRight, component.PaddingRight) / 2f;
            var totalWidth = scaleFactor * (component.HorizontalWidth - leftPadding - rightPadding);
            component.Update(invalidator, state, totalWidth, height, LayoutMode.Horizontal);
            invalidator.Transform.Translate(totalWidth, 0.0f);
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            var oldTransform = invalidator.Transform.Clone();
            var scaleFactor = mode == LayoutMode.Vertical
                    ? (float)height / OverallHeight
                    : (float)width / OverallWidth;

            for (var ind = 0; ind < VisibleComponents.Count(); ind++)
            {
                var component = VisibleComponents.ElementAt(ind);
                if (mode == LayoutMode.Vertical)
                    InvalidateVerticalComponent(ind, state, invalidator, width, height, scaleFactor);
                else
                    InvalidateHorizontalComponent(ind, state, invalidator, width, height, scaleFactor);
            }
            invalidator.Transform = oldTransform;
        }
    }
}
