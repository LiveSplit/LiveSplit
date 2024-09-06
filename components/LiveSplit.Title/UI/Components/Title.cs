using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

using LiveSplit.Model;

namespace LiveSplit.UI.Components;

public class Title : IComponent
{
    public TitleSettings Settings { get; set; }
    public float VerticalHeight { get; set; }
    public GraphicsCache Cache { get; set; }
    protected int FrameCount { get; set; }
    protected Image OldImage { get; set; }
    protected int FinishedRunsInHistory { get; set; }

    public float MinimumWidth => GameNameLabel.X + AttemptCountLabel.ActualWidth + 5;

    public float HorizontalWidth
    {
        get
        {
            if (!Settings.ShowCount)
            {
                return Math.Max(GameNameLabel.ActualWidth, CategoryNameLabel.ActualWidth) + GameNameLabel.X + 5;
            }

            // If the category + attempt is longer than the name, just return category + attempts
            if (CategoryNameLabel.ActualWidth + AttemptCountLabel.ActualWidth > GameNameLabel.ActualWidth)
            {
                return CategoryNameLabel.ActualWidth + AttemptCountLabel.ActualWidth + CategoryNameLabel.X + 5;
            }

            // The game name is longer than the category+attempts, so center the category, then add the attempts and compare with the game name.
            float centeredCategoryWidth = (GameNameLabel.ActualWidth / 2) + (CategoryNameLabel.ActualWidth / 2) + AttemptCountLabel.ActualWidth;
            if (centeredCategoryWidth > GameNameLabel.ActualWidth)
            {
                return centeredCategoryWidth + CategoryNameLabel.X + 5;
            }
            else
            {
                return GameNameLabel.ActualWidth + GameNameLabel.X + 5;
            }
        }
    }

    public IDictionary<string, Action> ContextMenuControls => null;

    public float PaddingTop => 0f;
    public float PaddingLeft => 7f;
    public float PaddingBottom => 0f;
    public float PaddingRight => 7f;

    protected SimpleLabel GameNameLabel = new();
    protected SimpleLabel CategoryNameLabel = new();
    protected SimpleLabel AttemptCountLabel = new();

    protected Font TitleFont { get; set; }

    public float MinimumHeight { get; set; }

    public Title()
    {
        VerticalHeight = 10;
        Settings = new TitleSettings();
        Cache = new GraphicsCache();
        GameNameLabel = new SimpleLabel();
        CategoryNameLabel = new SimpleLabel();
        AttemptCountLabel = new SimpleLabel();
    }

    private void DrawGeneral(Graphics g, LiveSplitState state, float width, float height, LayoutMode mode)
    {
        DrawBackground(g, width, height);

        if (Settings.OverrideTitleFont)
        {
            TitleFont = Settings.TitleFont;
        }
        else
        {
            TitleFont = state.LayoutSettings.TextFont;
        }

        MinimumHeight = g.MeasureString("A", TitleFont).Height * 1.7f;
        VerticalHeight = g.MeasureString("A", TitleFont).Height * 1.7f;
        bool showGameIcon = state.Run.GameIcon != null && Settings.DisplayGameIcon;
        if (showGameIcon)
        {
            DrawGameIcon(g, state, height);
        }

        DrawAttemptCount(g, state, width, height);

        CalculatePadding(height, mode, showGameIcon, out float startPadding, out float titleEndPadding, out float categoryEndPadding);

        DrawGameName(g, state, width, height, showGameIcon, startPadding, titleEndPadding);
        DrawCategoryName(g, state, width, height, showGameIcon, startPadding, categoryEndPadding);
    }

    private void DrawBackground(Graphics g, float width, float height)
    {
        if (Settings.BackgroundColor.A > 0
            || (Settings.BackgroundGradient != GradientType.Plain
            && Settings.BackgroundColor2.A > 0))
        {
            var gradientBrush = new LinearGradientBrush(
                        new PointF(0, 0),
                        Settings.BackgroundGradient == GradientType.Horizontal
                        ? new PointF(width, 0)
                        : new PointF(0, height),
                        Settings.BackgroundColor,
                        Settings.BackgroundGradient == GradientType.Plain
                        ? Settings.BackgroundColor
                        : Settings.BackgroundColor2);
            g.FillRectangle(gradientBrush, 0, 0, width, height);
        }
    }

    private void CalculatePadding(float height, LayoutMode mode, bool showGameIcon, out float startPadding, out float titleEndPadding, out float categoryEndPadding)
    {
        startPadding = 5;
        titleEndPadding = 5;
        categoryEndPadding = 5;
        if (showGameIcon)
        {
            startPadding += height + 3;
        }

        if (mode == LayoutMode.Vertical && Settings.ShowCount)
        {
            if (string.IsNullOrEmpty(CategoryNameLabel.Text))
            {
                titleEndPadding += AttemptCountLabel.ActualWidth;
            }
            else
            {
                categoryEndPadding += AttemptCountLabel.ActualWidth;
            }
        }
    }

    private void DrawCategoryName(Graphics g, LiveSplitState state, float width, float height, bool showGameIcon, float startPadding, float categoryEndPadding)
    {
        if (Settings.TextAlignment == AlignmentType.Center || (Settings.TextAlignment == AlignmentType.Auto && !showGameIcon))
        {
            CategoryNameLabel.CalculateAlternateText(g, width - startPadding - categoryEndPadding);
            float stringWidth = CategoryNameLabel.ActualWidth;
            PositionAndWidth positionAndWidth = calculateCenteredPositionAndWidth(width, stringWidth, startPadding, categoryEndPadding);
            CategoryNameLabel.X = positionAndWidth.position;
            CategoryNameLabel.Width = positionAndWidth.width;
        }
        else
        {
            CategoryNameLabel.X = startPadding;
            CategoryNameLabel.Width = width - startPadding - categoryEndPadding;
        }

        CategoryNameLabel.Y = 0;
        CategoryNameLabel.HorizontalAlignment = StringAlignment.Near;
        CategoryNameLabel.VerticalAlignment = string.IsNullOrEmpty(GameNameLabel.Text) ? StringAlignment.Center : StringAlignment.Far;
        CategoryNameLabel.Font = TitleFont;
        CategoryNameLabel.Brush = new SolidBrush(Settings.OverrideTitleColor ? Settings.TitleColor : state.LayoutSettings.TextColor);
        CategoryNameLabel.HasShadow = state.LayoutSettings.DropShadows;
        CategoryNameLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
        CategoryNameLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
        CategoryNameLabel.Height = height;
        CategoryNameLabel.Draw(g);
    }

    private void DrawAttemptCount(Graphics g, LiveSplitState state, float width, float height)
    {
        if (Settings.ShowCount)
        {
            AttemptCountLabel.HorizontalAlignment = StringAlignment.Far;
            AttemptCountLabel.VerticalAlignment = StringAlignment.Far;
            AttemptCountLabel.X = 0;
            AttemptCountLabel.Y = height - 40;
            AttemptCountLabel.Width = width - 5;
            AttemptCountLabel.Height = 40;
            AttemptCountLabel.Font = TitleFont;
            AttemptCountLabel.Brush = new SolidBrush(Settings.OverrideTitleColor ? Settings.TitleColor : state.LayoutSettings.TextColor);
            AttemptCountLabel.HasShadow = state.LayoutSettings.DropShadows;
            AttemptCountLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
            AttemptCountLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
            AttemptCountLabel.Draw(g);
        }
    }

    private void DrawGameName(Graphics g, LiveSplitState state, float width, float height, bool showGameIcon, float startPadding, float titleEndPadding)
    {
        if (Settings.TextAlignment == AlignmentType.Center || (Settings.TextAlignment == AlignmentType.Auto && !showGameIcon))
        {
            GameNameLabel.CalculateAlternateText(g, width - startPadding - titleEndPadding);
            float stringWidth = GameNameLabel.ActualWidth;
            PositionAndWidth positionAndWidth = calculateCenteredPositionAndWidth(width, stringWidth, startPadding, titleEndPadding);
            GameNameLabel.X = positionAndWidth.position;
            GameNameLabel.Width = positionAndWidth.width;
        }
        else
        {
            GameNameLabel.X = startPadding;
            GameNameLabel.Width = width - startPadding - titleEndPadding;
        }

        GameNameLabel.HorizontalAlignment = StringAlignment.Near;
        GameNameLabel.VerticalAlignment = string.IsNullOrEmpty(CategoryNameLabel.Text) ? StringAlignment.Center : StringAlignment.Near;
        GameNameLabel.Y = 0;
        GameNameLabel.Height = height;
        GameNameLabel.Font = TitleFont;
        GameNameLabel.Brush = new SolidBrush(Settings.OverrideTitleColor ? Settings.TitleColor : state.LayoutSettings.TextColor);
        GameNameLabel.HasShadow = state.LayoutSettings.DropShadows;
        GameNameLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
        GameNameLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
        GameNameLabel.Draw(g);
    }

    private void DrawGameIcon(Graphics g, LiveSplitState state, float height)
    {
        Image icon = state.Run.GameIcon;

        if (OldImage != icon)
        {
            ImageAnimator.Animate(icon, (s, o) => { });
            OldImage = icon;
        }

        float aspectRatio = (float)icon.Width / icon.Height;
        float drawWidth = height - 4;
        float drawHeight = height - 4;
        if (icon.Width > icon.Height)
        {
            float ratio = icon.Height / (float)icon.Width;
            drawHeight *= ratio;
        }
        else
        {
            float ratio = icon.Width / (float)icon.Height;
            drawWidth *= ratio;
        }

        ImageAnimator.UpdateFrames(icon);

        g.DrawImage(
            icon,
            7 + ((height - 4 - drawWidth) / 2),
            2 + ((height - 4 - drawHeight) / 2),
            drawWidth,
            drawHeight);
    }

    /*
     * Returns coordinate and width of the string element so that the text is centered in the total width
     * while not overlapping into the start or ending padding.
     */
    private PositionAndWidth calculateCenteredPositionAndWidth(float totalWidth, float stringWidth, float startPadding, float endPadding)
    {
        float position, width;
        if (startPadding + stringWidth + endPadding > totalWidth)
        {
            // We cant fit no matter what we do, so start the string after the start padding 
            position = startPadding;
        }
        else
        {
            // Try to center, but push the string left or right if it overlaps the padding
            position = (totalWidth - stringWidth) / 2;
            position = Math.Max(position, startPadding);
            if (position + stringWidth > totalWidth - endPadding)
            {
                position = totalWidth - endPadding - stringWidth;
            }
        }

        width = totalWidth - endPadding - position;
        return new PositionAndWidth(position, width);
    }

    private class PositionAndWidth
    {
        public float position { get; set; }
        public float width { get; set; }
        public PositionAndWidth(float position, float width)
        {
            this.position = position;
            this.width = width;
        }
    }

    public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
    {
        DrawGeneral(g, state, HorizontalWidth, height, LayoutMode.Horizontal);
    }

    public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
    {
        DrawGeneral(g, state, width, VerticalHeight, LayoutMode.Vertical);
    }

    public string ComponentName => "Title";

    public Control GetSettingsControl(LayoutMode mode)
    {
        return Settings;
    }

    public XmlNode GetSettings(XmlDocument document)
    {
        return Settings.GetSettings(document);
    }

    public void SetSettings(XmlNode settings)
    {
        Settings.SetSettings(settings);
    }

    private IEnumerable<string> getCategoryNameAbbreviations(string categoryName)
    {
        int indexStart = categoryName.IndexOf('(');
        int indexEnd = categoryName.IndexOf(')', indexStart + 1);
        string afterParentheses = "";
        if (indexStart >= 0 && indexEnd >= 0)
        {
            string inside = categoryName.Substring(indexStart + 1, indexEnd - indexStart - 1);
            afterParentheses = categoryName[(indexEnd + 1)..].Trim();
            categoryName = categoryName[..indexStart].Trim();
            string[] splits = inside.Split(',');

            for (int i = splits.Length - 1; i > 0; --i)
            {
                yield return $"{categoryName} ({string.Join(",", splits.Take(i))}) {afterParentheses}".Trim();
            }
        }

        yield return $"{categoryName} {afterParentheses}".Trim();
    }

    public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
    {
        string extendedCategoryName = state.Run.GetExtendedCategoryName(Settings.ShowRegion, Settings.ShowPlatform, Settings.ShowVariables);

        Cache.Restart();
        Cache["SingleLine"] = Settings.SingleLine;
        Cache["GameName"] = state.Run.GameName;
        Cache["CategoryName"] = extendedCategoryName;
        Cache["LayoutMode"] = mode;
        Cache["ShowGameName"] = Settings.ShowGameName;
        Cache["ShowCategoryName"] = Settings.ShowCategoryName;
        if (Cache.HasChanged)
        {
            if (Settings.SingleLine && Settings.ShowGameName && Settings.ShowCategoryName)
            {
                string text = string.Format("{0} - {1}", state.Run.GameName, extendedCategoryName);
                IEnumerable<string> gameAbbreviations = state.Run.GameName.GetAbbreviations();
                string shortestGameName = gameAbbreviations.Last();
                IEnumerable<string> categoryAbbreviations = getCategoryNameAbbreviations(extendedCategoryName);
                IEnumerable<string> combinedAbbreviations1 = gameAbbreviations.Select(x => string.Format("{0} - {1}", x, extendedCategoryName));
                IEnumerable<string> combinedAbbreviations2 = categoryAbbreviations.Select(x => string.Format("{0} - {1}", shortestGameName, x));
                var abbreviations = combinedAbbreviations1.Concat(combinedAbbreviations2).ToList();
                GameNameLabel.Text = text;
                GameNameLabel.AlternateText = mode == LayoutMode.Vertical ? abbreviations : [];
                CategoryNameLabel.Text = "";
            }
            else
            {
                if (Settings.ShowGameName)
                {
                    GameNameLabel.Text = state.Run.GameName;
                    GameNameLabel.AlternateText = mode == LayoutMode.Vertical ? state.Run.GameName.GetAbbreviations().ToList() : [];
                }
                else
                {
                    GameNameLabel.Text = "";
                    GameNameLabel.AlternateText = [];
                }

                if (Settings.ShowCategoryName)
                {
                    CategoryNameLabel.Text = extendedCategoryName;
                    CategoryNameLabel.AlternateText = mode == LayoutMode.Vertical ? getCategoryNameAbbreviations(extendedCategoryName).ToList() : [];
                }
                else
                {
                    CategoryNameLabel.Text = "";
                    CategoryNameLabel.AlternateText = [];
                }
            }
        }

        Cache.Restart();
        Cache["AttemptHistoryCount"] = state.Run.AttemptHistory.Count;
        Cache["Run"] = state.Run;
        if (Cache.HasChanged)
        {
            FinishedRunsInHistory = state.Run.AttemptHistory.Count(x => x.Time.RealTime != null);
        }

        int totalFinishedRunsCount = FinishedRunsInHistory + (state.CurrentPhase == TimerPhase.Ended ? 1 : 0);

        if (Settings.ShowAttemptCount && Settings.ShowFinishedRunsCount)
        {
            AttemptCountLabel.Text = string.Format("{0}/{1}", totalFinishedRunsCount, state.Run.AttemptCount);
        }
        else if (Settings.ShowAttemptCount)
        {
            AttemptCountLabel.Text = state.Run.AttemptCount.ToString();
        }
        else if (Settings.ShowFinishedRunsCount)
        {
            AttemptCountLabel.Text = totalFinishedRunsCount.ToString();
        }

        Cache.Restart();
        Cache["GameIcon"] = state.Run.GameIcon;
        if (Cache.HasChanged)
        {
            if (state.Run.GameIcon == null)
            {
                FrameCount = 0;
            }
            else
            {
                FrameCount = state.Run.GameIcon.GetFrameCount(new FrameDimension(state.Run.GameIcon.FrameDimensionsList[0]));
            }
        }

        Cache["GameNameLabel"] = GameNameLabel.Text;
        Cache["CategoryNameLabel"] = CategoryNameLabel.Text;
        Cache["AttemptCountLabel"] = AttemptCountLabel.Text;
        Cache["TextAlignment"] = Settings.TextAlignment;

        if (invalidator != null && (Cache.HasChanged || FrameCount > 1))
        {
            invalidator.Invalidate(0, 0, width, height);
        }
    }

    public void Dispose()
    {
    }

    public int GetSettingsHashCode()
    {
        return Settings.GetSettingsHashCode();
    }
}
