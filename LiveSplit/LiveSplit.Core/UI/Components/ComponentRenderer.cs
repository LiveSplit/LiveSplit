using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.UI.Components.SplitAt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace LiveSplit.UI.Components
{
    public class ComponentRenderer
    {

        public IEnumerable<IComponent> VisibleComponents { get; set; }

        /// <summary>
        /// If enabled, looks for SplitAtComponent for splitting the drawing into several lines.
        /// </summary>
        /// <remarks>
        /// A SplitAtComponent added to VisibleComponents will start a new line at it's position,
        /// with the given spacing (relative to overall size) applied between the old and new line.
        /// The word "line" in this context refers either to rows (horizontal layout) or columns
        /// (vertical layout) and "size" would be either the height or the width.
        /// </remarks>
        public bool SplitAtEnabled { get; set; }

        public float OverallSize = 10f;

        public float MinimumWidth
            => !VisibleComponents.Any() ? 0 : VisibleComponents.Max(x => x.MinimumWidth);

        public float MinimumHeight 
            => !VisibleComponents.Any() ? 0 : VisibleComponents.Max(x => x.MinimumHeight);

        protected bool errorInComponent;

        private void DrawVerticalComponent(int index, Graphics g, LiveSplitState state, float width, float height, Region clipRegion)
        {
            var component = VisibleComponents.ElementAt(index);
            var topPadding = Math.Min(GetPaddingAbove(index), component.PaddingTop) / 2f;
            var bottomPadding = Math.Min(GetPaddingBelow(index), component.PaddingBottom) / 2f;
            g.IntersectClip(new RectangleF(0, topPadding, width, component.VerticalHeight - topPadding - bottomPadding));

            var scale = g.Transform.Elements.First();
            var separatorOffset = component.VerticalHeight * scale < 3 ? 1 : 0;

            if (clipRegion.IsVisible(new RectangleF(
                g.Transform.OffsetX,
                -separatorOffset + g.Transform.OffsetY - topPadding * scale,
                width,
                separatorOffset * 2f + scale * (component.VerticalHeight + bottomPadding))))
                component.DrawVertical(g, state, width, clipRegion);
            g.TranslateTransform(0.0f, component.VerticalHeight - bottomPadding * 2f);
        }

        private void DrawHorizontalComponent(int index, Graphics g, LiveSplitState state, float width, float height, Region clipRegion)
        {
            var component = VisibleComponents.ElementAt(index);
            var leftPadding = Math.Min(GetPaddingToLeft(index), component.PaddingLeft) / 2f;
            var rightPadding = Math.Min(GetPaddingToRight(index), component.PaddingRight) / 2f;
            g.IntersectClip(new RectangleF(leftPadding, 0, component.HorizontalWidth - leftPadding - rightPadding, height));

            var scale = g.Transform.Elements.First();
            var separatorOffset = component.VerticalHeight * scale < 3 ? 1 : 0;

            if (clipRegion.IsVisible(new RectangleF(
                -separatorOffset + g.Transform.OffsetX - leftPadding * scale,
                g.Transform.OffsetY,
                separatorOffset * 2f + scale * (component.HorizontalWidth + rightPadding),
                height)))
                component.DrawHorizontal(g, state, height, clipRegion);
            g.TranslateTransform(component.HorizontalWidth - rightPadding * 2f, 0.0f);
        }

        private float GetPaddingAbove(int index)
        {
            while (index > 0)
            {
                index--;
                var component = VisibleComponents.ElementAt(index);
                if (component.VerticalHeight != 0)
                    return component.PaddingBottom;
            }
            return 0f;
        }

        private float GetPaddingBelow(int index)
        {
            while (index < VisibleComponents.Count() - 1)
            {
                index++;
                var component = VisibleComponents.ElementAt(index);
                if (component.VerticalHeight != 0)
                    return component.PaddingTop;
            }
            return 0f;
        }

        private float GetPaddingToLeft(int index)
        {
            while (index > 0)
            {
                index--;
                var component = VisibleComponents.ElementAt(index);
                if (component.HorizontalWidth != 0)
                    return component.PaddingLeft;
            }
            return 0f;
        }

        private float GetPaddingToRight(int index)
        {
            while (index < VisibleComponents.Count() - 1)
            {
                index++;
                var component = VisibleComponents.ElementAt(index);
                if (component.HorizontalWidth != 0)
                    return component.PaddingRight;
            }
            return 0f;
        }

        protected float GetHeightVertical(int index)
        {
            var component = VisibleComponents.ElementAt(index);
            var bottomPadding = Math.Min(GetPaddingBelow(index), component.PaddingBottom) / 2f;
            return component.VerticalHeight - bottomPadding * 2f;
        }

        protected float GetWidthHorizontal(int index)
        {
            var component = VisibleComponents.ElementAt(index);
            var rightPadding = Math.Min(GetPaddingToRight(index), component.PaddingRight) / 2f;
            return component.HorizontalWidth - rightPadding * 2f;
        }

        public void CalculateOverallSize(LayoutMode mode)
        {
            // totalSize2 remembers the longest line (or stays at 1 if no separate lines exist)
            var totalSize = 0f;
            var totalSize2 = 1f;
            var index = 0;
            foreach (var component in VisibleComponents)
            {
                if (mode == LayoutMode.Vertical)
                    totalSize += GetHeightVertical(index);
                else
                    totalSize += GetWidthHorizontal(index);
                index++;
                if (ShouldSplitAt(component))
                {
                    totalSize2 = Math.Max(totalSize2, totalSize);
                    totalSize = 0;
                }
            }

            OverallSize = Math.Max(totalSize, totalSize2);
        }


        public void Render(Graphics g, LiveSplitState state, float width, float height, LayoutMode mode, Region clipRegion)
        {
            if (!errorInComponent)
            {
                try
                {
                    var clip = g.Clip;
                    var transform = g.Transform;
                    var orig = g.Transform.Elements;
                    var crashedComponents = new List<IComponent>();
                    var index = 0;

                    float[] lineSizes;
                    if (mode == LayoutMode.Vertical)
                        lineSizes = GetLineSizes(width);
                    else
                        lineSizes = GetLineSizes(height);

                    int lineIndex = 0;
                    foreach (var component in VisibleComponents)
                    {
                        try
                        {
                            g.Clip = clip;
                            if (ShouldSplitAt(component))
                            {
                                lineIndex++;
                                //Console.WriteLine("BOffset: " + g.Transform.OffsetX + " / " + g.Transform.OffsetY+" Orig: "+transform.OffsetX+" / "+transform.OffsetY);
                                g.Transform = ChangeTransformMove(g.Transform, orig, mode, lineSizes[lineIndex - 1], width, height, component);
                                //Console.WriteLine("AOffset: "+ g.Transform.OffsetX+" / "+g.Transform.OffsetY+" "+ sizes[lineIndex - 1]);
                            }

                            if (mode == LayoutMode.Vertical)
                                DrawVerticalComponent(index, g, state, lineSizes[lineIndex], height, clipRegion);
                            else
                                DrawHorizontalComponent(index, g, state, width, lineSizes[lineIndex], clipRegion);
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
                        crashedComponents.ForEach(x =>
                        {
                            remainingComponents.Remove(x);
                            state.Layout.LayoutComponents = state.Layout.LayoutComponents.Where(y => y.Component != x).ToList();
                        });
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
        }

        protected void InvalidateVerticalComponent(int index, LiveSplitState state, IInvalidator invalidator, float width, float height, float scaleFactor)
        {
            var component = VisibleComponents.ElementAt(index);
            var topPadding = Math.Min(GetPaddingAbove(index), component.PaddingTop) / 2f;
            var bottomPadding = Math.Min(GetPaddingBelow(index), component.PaddingBottom) / 2f;
            var totalHeight = scaleFactor * (component.VerticalHeight - topPadding - bottomPadding);
            component.Update(invalidator, state, width, totalHeight, LayoutMode.Vertical);
            invalidator.Transform.Translate(0.0f, totalHeight);
        }

        protected void InvalidateHorizontalComponent(int index, LiveSplitState state, IInvalidator invalidator, float width, float height, float scaleFactor)
        {
            var component = VisibleComponents.ElementAt(index);
            var leftPadding = Math.Min(GetPaddingToLeft(index), component.PaddingLeft) / 2f;
            var rightPadding = Math.Min(GetPaddingToRight(index), component.PaddingRight) / 2f;
            var totalWidth = scaleFactor * (component.HorizontalWidth - leftPadding - rightPadding);
            component.Update(invalidator, state, totalWidth, height, LayoutMode.Horizontal);
            invalidator.Transform.Translate(totalWidth, 0.0f);
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            float[] lineSizes;
            if (mode == LayoutMode.Vertical)
                lineSizes = GetLineSizes(width);
            else
                lineSizes = GetLineSizes(height);

            var oldTransform = invalidator.Transform.Clone();
            var oldTransformElements = oldTransform.Elements;
            var scaleFactor = mode == LayoutMode.Vertical
                    ? height / OverallSize
                    : width / OverallSize;

            int lineIndex = 0;
            for (var ind = 0; ind < VisibleComponents.Count(); ind++)
            {
                var component = VisibleComponents.ElementAt(ind);
                if (ShouldSplitAt(component))
                {
                    lineIndex++;
                    invalidator.Transform = ChangeTransformMove(invalidator.Transform, oldTransformElements, mode, lineSizes[lineIndex-1], width, height, component);
                }

                if (mode == LayoutMode.Vertical)
                    InvalidateVerticalComponent(ind, state, invalidator, lineSizes[lineIndex], height, scaleFactor);
                else
                    InvalidateHorizontalComponent(ind, state, invalidator, width, lineSizes[lineIndex], scaleFactor);
            }
            invalidator.Transform = oldTransform;
        }

        /// <summary>
        /// Check whether to split at the current component.
        /// </summary>
        /// <param name="comp">The component that will be drawn</param>
        /// <returns>Whether the following components should be drawn in the next line</returns>
        /// <remarks>Since a SplitAtComponent should take up no space, it probably doesn't matter if the split actually is done before or after the component.</remarks>
        private bool ShouldSplitAt(IComponent comp)
        {
            return SplitAtEnabled && comp is SplitAtComponent;
        }

        /// <summary>
        /// Get the size for each line, based on the given overall <paramref name="size"/>
        /// and the weight setting for each line.
        /// </summary>
        /// <param name="size">The overall size of the component</param>
        /// <returns>The sizes of each line</returns>
        private float[] GetLineSizes(float size)
        {
            if (!SplitAtEnabled)
            {
                return new float[] { size };
            }
            int lines = 1;
            float spacing = 0;
            // Accumulate all size weights, starting with 100 for the first line
            int totalWeight = 1000;
            foreach (IComponent comp in VisibleComponents)
            {
                var splitAtComp = comp as SplitAtComponent;
                if (splitAtComp != null)
                {
                    lines++;
                    spacing += GetSpacing(size, splitAtComp.Settings.Spacing);
                    totalWeight += splitAtComp.Settings.Weight;
                }
            }
            if (lines == 1)
            {
                return new float[] { size };
            }
            else
            {
                // Calculate the size of each line, relative to other lines
                float normalizedSize = (size - spacing) / (totalWeight / 1000f);
                float[] result = new float[lines];
                // First line is always just be 100%
                result[0] = normalizedSize;
                int index = 1;
                foreach (IComponent comp in VisibleComponents)
                {
                    var splitAtComp = comp as SplitAtComponent;
                    if (splitAtComp != null)
                    {
                        result[index] = normalizedSize * (splitAtComp.Settings.Weight / 1000f);
                        index++;
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Calculate the spacing between lines relative to the overall <paramref name="size"/> of the component.
        /// </summary>
        /// <param name="size">The overall size of the component, used to calcuate the actual spacing</param>
        /// <param name="spacing">The percentage (0-1000, in 1/10th steps) of the overall size of the component</param>
        /// <returns>The spacing</returns>
        private float GetSpacing(float size, int spacing)
        {
            // Relative to size because size may be scaled, so this
            // way the spacing is automatically correctly scaled as well
            // (maybe it could calculate from an absolute spacing using
            // the scaleFactor, but this seems to work)
            return spacing / 1000f * size;
        }

        /// <summary>
        /// Returns a new Matrix that is a copy of the given <paramref name="transform"/>, except
        /// that the x or y offset (depending on the layout) is reset to original (usually near 0)
        /// and some offset is added (to move into the next line).
        /// </summary>
        /// <param name="transform">The current transform</param>
        /// <param name="orig">The original transform elements</param>
        /// <param name="mode">The layout mode (vertical or horizontal)</param>
        /// <param name="move">How much to offset (previous line size)</param>
        /// <param name="width">The overall width</param>
        /// <param name="height">The overall height</param>
        /// <param name="comp">The component</param>
        /// <returns>A modified matrix</returns>
        /// <remarks>
        /// Directly setting the x/y offsets of a Matrix doesn't seem possible, and using the
        /// translate function doesn't really work well for resetting to a specific value if
        /// some other transformation (in this case usually scaling) is set (although it could
        /// probably be solved).
        /// 
        /// The move/width/height is already received scaled accordingly, so the Translate
        /// method is used.
        /// 
        /// The original x/y is used instead of setting to 0 since the original may not actually
        /// be 0 (and all lines should start the same).
        /// </remarks>
        private Matrix ChangeTransformMove(Matrix transform, float[] orig, LayoutMode mode, float move, float width, float height, IComponent comp)
        {
            float[] curr = transform.Elements;

            if (mode == LayoutMode.Vertical)
            {
                transform = new Matrix(curr[0], curr[1], curr[2], curr[3], curr[4], orig[5]);
                transform.Translate(move + GetSpacing(width, ((SplitAtComponent)comp).Settings.Spacing), 0);
            }
            else
            {
                transform = new Matrix(curr[0], curr[1], curr[2], curr[3], orig[4], curr[5]);
                transform.Translate(0, move + GetSpacing(height, ((SplitAtComponent)comp).Settings.Spacing));
            }
            return transform;
        }

    }

}
