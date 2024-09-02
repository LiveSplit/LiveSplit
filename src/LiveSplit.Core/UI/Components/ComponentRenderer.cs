using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using LiveSplit.Model;
using LiveSplit.Options;

namespace LiveSplit.UI.Components;

public class ComponentRenderer
{
    public IEnumerable<IComponent> VisibleComponents { get; set; }

    public float OverallSize = 10f;

    public float MinimumWidth
        => !VisibleComponents.Any() ? 0 : VisibleComponents.Max(x => x.MinimumWidth);

    public float MinimumHeight
        => !VisibleComponents.Any() ? 0 : VisibleComponents.Max(x => x.MinimumHeight);

    protected bool errorInComponent;

    private void DrawVerticalComponent(int index, Graphics g, LiveSplitState state, float width, float height, Region clipRegion)
    {
        IComponent component = VisibleComponents.ElementAt(index);
        float topPadding = Math.Min(GetPaddingAbove(index), component.PaddingTop) / 2f;
        float bottomPadding = Math.Min(GetPaddingBelow(index), component.PaddingBottom) / 2f;
        g.IntersectClip(new RectangleF(0, topPadding, width, component.VerticalHeight - topPadding - bottomPadding));

        float scale = g.Transform.Elements.First();
        int separatorOffset = component.VerticalHeight * scale < 3 ? 1 : 0;

        if (clipRegion.IsVisible(new RectangleF(
            g.Transform.OffsetX,
            -separatorOffset + g.Transform.OffsetY - (topPadding * scale),
            width,
            (separatorOffset * 2f) + (scale * (component.VerticalHeight + bottomPadding)))))
        {
            component.DrawVertical(g, state, width, clipRegion);
        }

        g.TranslateTransform(0.0f, component.VerticalHeight - (bottomPadding * 2f));
    }

    private void DrawHorizontalComponent(int index, Graphics g, LiveSplitState state, float width, float height, Region clipRegion)
    {
        IComponent component = VisibleComponents.ElementAt(index);
        float leftPadding = Math.Min(GetPaddingToLeft(index), component.PaddingLeft) / 2f;
        float rightPadding = Math.Min(GetPaddingToRight(index), component.PaddingRight) / 2f;
        g.IntersectClip(new RectangleF(leftPadding, 0, component.HorizontalWidth - leftPadding - rightPadding, height));

        float scale = g.Transform.Elements.First();
        int separatorOffset = component.VerticalHeight * scale < 3 ? 1 : 0;

        if (clipRegion.IsVisible(new RectangleF(
            -separatorOffset + g.Transform.OffsetX - (leftPadding * scale),
            g.Transform.OffsetY,
            (separatorOffset * 2f) + (scale * (component.HorizontalWidth + rightPadding)),
            height)))
        {
            component.DrawHorizontal(g, state, height, clipRegion);
        }

        g.TranslateTransform(component.HorizontalWidth - (rightPadding * 2f), 0.0f);
    }

    private float GetPaddingAbove(int index)
    {
        while (index > 0)
        {
            index--;
            IComponent component = VisibleComponents.ElementAt(index);
            if (component.VerticalHeight != 0)
            {
                return component.PaddingBottom;
            }
        }

        return 0f;
    }

    private float GetPaddingBelow(int index)
    {
        while (index < VisibleComponents.Count() - 1)
        {
            index++;
            IComponent component = VisibleComponents.ElementAt(index);
            if (component.VerticalHeight != 0)
            {
                return component.PaddingTop;
            }
        }

        return 0f;
    }

    private float GetPaddingToLeft(int index)
    {
        while (index > 0)
        {
            index--;
            IComponent component = VisibleComponents.ElementAt(index);
            if (component.HorizontalWidth != 0)
            {
                return component.PaddingLeft;
            }
        }

        return 0f;
    }

    private float GetPaddingToRight(int index)
    {
        while (index < VisibleComponents.Count() - 1)
        {
            index++;
            IComponent component = VisibleComponents.ElementAt(index);
            if (component.HorizontalWidth != 0)
            {
                return component.PaddingRight;
            }
        }

        return 0f;
    }

    protected float GetHeightVertical(int index)
    {
        IComponent component = VisibleComponents.ElementAt(index);
        float bottomPadding = Math.Min(GetPaddingBelow(index), component.PaddingBottom) / 2f;
        return component.VerticalHeight - (bottomPadding * 2f);
    }

    protected float GetWidthHorizontal(int index)
    {
        IComponent component = VisibleComponents.ElementAt(index);
        float rightPadding = Math.Min(GetPaddingToRight(index), component.PaddingRight) / 2f;
        return component.HorizontalWidth - (rightPadding * 2f);
    }

    public void CalculateOverallSize(LayoutMode mode)
    {
        float totalSize = 0f;
        int index = 0;
        foreach (IComponent component in VisibleComponents)
        {
            if (mode == LayoutMode.Vertical)
            {
                totalSize += GetHeightVertical(index);
            }
            else
            {
                totalSize += GetWidthHorizontal(index);
            }

            index++;
        }

        OverallSize = Math.Max(totalSize, 1f);
    }

    public void Render(Graphics g, LiveSplitState state, float width, float height, LayoutMode mode, Region clipRegion)
    {
        if (!errorInComponent)
        {
            try
            {
                Region clip = g.Clip;
                System.Drawing.Drawing2D.Matrix transform = g.Transform;
                var crashedComponents = new List<IComponent>();
                int index = 0;
                foreach (IComponent component in VisibleComponents)
                {
                    try
                    {
                        g.Clip = clip;
                        if (mode == LayoutMode.Vertical)
                        {
                            DrawVerticalComponent(index, g, state, width, height, clipRegion);
                        }
                        else
                        {
                            DrawHorizontalComponent(index, g, state, width, height, clipRegion);
                        }
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
        IComponent component = VisibleComponents.ElementAt(index);
        float topPadding = Math.Min(GetPaddingAbove(index), component.PaddingTop) / 2f;
        float bottomPadding = Math.Min(GetPaddingBelow(index), component.PaddingBottom) / 2f;
        float totalHeight = scaleFactor * (component.VerticalHeight - topPadding - bottomPadding);
        component.Update(invalidator, state, width, totalHeight, LayoutMode.Vertical);
        invalidator.Transform.Translate(0.0f, totalHeight);
    }

    protected void InvalidateHorizontalComponent(int index, LiveSplitState state, IInvalidator invalidator, float width, float height, float scaleFactor)
    {
        IComponent component = VisibleComponents.ElementAt(index);
        float leftPadding = Math.Min(GetPaddingToLeft(index), component.PaddingLeft) / 2f;
        float rightPadding = Math.Min(GetPaddingToRight(index), component.PaddingRight) / 2f;
        float totalWidth = scaleFactor * (component.HorizontalWidth - leftPadding - rightPadding);
        component.Update(invalidator, state, totalWidth, height, LayoutMode.Horizontal);
        invalidator.Transform.Translate(totalWidth, 0.0f);
    }

    public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
    {
        System.Drawing.Drawing2D.Matrix oldTransform = invalidator.Transform.Clone();
        float scaleFactor = mode == LayoutMode.Vertical
                ? height / OverallSize
                : width / OverallSize;

        for (int ind = 0; ind < VisibleComponents.Count(); ind++)
        {
            IComponent component = VisibleComponents.ElementAt(ind);
            if (mode == LayoutMode.Vertical)
            {
                InvalidateVerticalComponent(ind, state, invalidator, width, height, scaleFactor);
            }
            else
            {
                InvalidateHorizontalComponent(ind, state, invalidator, width, height, scaleFactor);
            }
        }

        invalidator.Transform = oldTransform;
    }
}
