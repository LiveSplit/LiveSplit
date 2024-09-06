using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

using LiveSplit.Model;

namespace LiveSplit.UI.Components;

public class SplitsComponent : IComponent
{
    public ComponentRendererComponent InternalComponent { get; protected set; }

    public float PaddingTop => InternalComponent.PaddingTop;
    public float PaddingLeft => InternalComponent.PaddingLeft;
    public float PaddingBottom => InternalComponent.PaddingBottom;
    public float PaddingRight => InternalComponent.PaddingRight;

    protected IList<IComponent> Components { get; set; }
    protected IList<SplitComponent> SplitComponents { get; set; }

    protected SplitsSettings Settings { get; set; }

    private Dictionary<Image, Image> ShadowImages { get; set; }

    private int visualSplitCount;
    private IRun previousRun;

    protected bool PreviousShowLabels { get; set; }

    private readonly SectionList sectionList;

    protected int ScrollOffset { get; set; }
    protected int LastSplitSeparatorIndex { get; set; }
    private int lastSplitOfSection { get; set; }

    protected LiveSplitState CurrentState { get; set; }
    protected LiveSplitState OldState { get; set; }
    protected LayoutMode OldLayoutMode { get; set; }
    protected Color OldShadowsColor { get; set; }

    protected IEnumerable<ColumnData> ColumnsList => Settings.ColumnsList.Select(x => x.Data);

    public string ComponentName
      => "Subsplits";

    public float VerticalHeight => InternalComponent.VerticalHeight;

    public float MinimumWidth => InternalComponent.MinimumWidth;

    public float HorizontalWidth => InternalComponent.HorizontalWidth;

    public float MinimumHeight => InternalComponent.MinimumHeight;

    public IDictionary<string, Action> ContextMenuControls => null;

    public SplitsComponent(LiveSplitState state)
    {
        CurrentState = state;
        Settings = new SplitsSettings(state);
        InternalComponent = new ComponentRendererComponent();
        ShadowImages = [];
        visualSplitCount = Settings.VisualSplitCount;
        Settings.SplitLayoutChanged += Settings_SplitLayoutChanged;
        ScrollOffset = 0;
        RebuildVisualSplits();
        sectionList = new SectionList();
        previousRun = state.Run;
        sectionList.UpdateSplits(state.Run);
    }

    private void state_RunManuallyModified(object sender, EventArgs e)
    {
        sectionList.UpdateSplits(((LiveSplitState)sender).Run);
    }

    private void state_ComparisonRenamed(object sender, EventArgs e)
    {
        var args = (RenameEventArgs)e;
        foreach (ColumnData column in ColumnsList)
        {
            if (column.Comparison == args.OldName)
            {
                column.Comparison = args.NewName;
                ((LiveSplitState)sender).Layout.HasChanged = true;
            }
        }
    }

    private void Settings_SplitLayoutChanged(object sender, EventArgs e)
    {
        RebuildVisualSplits();
    }

    private void RebuildVisualSplits()
    {
        Components = [];
        SplitComponents = [];
        InternalComponent.VisibleComponents = Components;

        if (Settings.ShowColumnLabels && CurrentState.Layout?.Mode == LayoutMode.Vertical)
        {
            Components.Add(new LabelsComponent(Settings, ColumnsList));
            Components.Add(new SeparatorComponent());
        }

        for (int i = 0; i < visualSplitCount; ++i)
        {
            if (i == visualSplitCount - 1 && i > 0)
            {
                LastSplitSeparatorIndex = Components.Count;
                if (Settings.AlwaysShowLastSplit && Settings.SeparatorLastSplit)
                {
                    Components.Add(new SeparatorComponent());
                }
                else if (Settings.ShowThinSeparators)
                {
                    Components.Add(new ThinSeparatorComponent());
                }
            }

            var splitComponent = new SplitComponent(Settings, ColumnsList);
            Components.Add(splitComponent);
            SplitComponents.Add(splitComponent);

            if (Settings.ShowThinSeparators && i < visualSplitCount - 2)
            {
                Components.Add(new ThinSeparatorComponent());
            }
        }
    }

    private void Prepare(LiveSplitState state)
    {
        if (state != OldState)
        {
            state.OnScrollDown += state_OnScrollDown;
            state.OnScrollUp += state_OnScrollUp;
            state.OnStart += state_OnStart;
            state.OnReset += state_OnReset;
            state.OnSplit += state_OnSplit;
            state.OnSkipSplit += state_OnSkipSplit;
            state.OnUndoSplit += state_OnUndoSplit;
            state.ComparisonRenamed += state_ComparisonRenamed;
            state.RunManuallyModified += state_RunManuallyModified;
            OldState = state;
        }

        if (Settings.VisualSplitCount != visualSplitCount
        || Settings.ShowColumnLabels != PreviousShowLabels
        || (Settings.ShowColumnLabels && state.Layout.Mode != OldLayoutMode))
        {
            PreviousShowLabels = Settings.ShowColumnLabels;
            OldLayoutMode = state.Layout.Mode;
            visualSplitCount = Settings.VisualSplitCount;
            RebuildVisualSplits();
        }

        if (OldShadowsColor != state.LayoutSettings.ShadowsColor)
        {
            ShadowImages.Clear();
        }

        foreach (ISegment split in state.Run)
        {
            if (split.Icon != null && (!ShadowImages.ContainsKey(split.Icon) || OldShadowsColor != state.LayoutSettings.ShadowsColor))
            {
                ShadowImages.Add(split.Icon, IconShadow.Generate(split.Icon, state.LayoutSettings.ShadowsColor));
            }
        }

        bool iconsNotBlank = state.Run.Count(x => x.Icon != null) > 0;

        foreach (SplitComponent split in SplitComponents)
        {
            bool hideIconSectionSplit = !Settings.ShowIconSectionSplit && split.Split != null && state.Run.IndexOf(split.Split) == lastSplitOfSection;
            bool shouldIndent = split.Split == null || split.Split.Icon != null || Settings.IndentBlankIcons;

            if (split.Header)
            {
                split.DisplayIcon = Settings.ShowSectionIcon && shouldIndent && iconsNotBlank;
            }
            else
            {
                split.DisplayIcon = Settings.DisplayIcons && !hideIconSectionSplit && iconsNotBlank && shouldIndent;
            }

            if (split.Split != null && split.Split.Icon != null)
            {
                split.ShadowImage = ShadowImages[split.Split.Icon];
            }
            else
            {
                split.ShadowImage = null;
            }
        }

        OldShadowsColor = state.LayoutSettings.ShadowsColor;

        foreach (IComponent component in Components)
        {
            if (component is SeparatorComponent separator)
            {
                int index = Components.IndexOf(separator);
                if (state.CurrentPhase is TimerPhase.Running or TimerPhase.Paused)
                {
                    if (((SplitComponent)Components[index + 1]).Split == state.CurrentSplit)
                    {
                        separator.LockToBottom = true;
                    }
                    else if (Components[index - 1] is SplitComponent splits && splits.Split == state.CurrentSplit)
                    {
                        separator.LockToBottom = false;
                    }
                }
            }
            else if (component is ThinSeparatorComponent thinSeparator)
            {
                int index = Components.IndexOf(thinSeparator);
                if (state.CurrentPhase is TimerPhase.Running or TimerPhase.Paused)
                {
                    if (((SplitComponent)Components[index + 1]).Split == state.CurrentSplit)
                    {
                        thinSeparator.LockToBottom = true;
                    }
                    else if (Components[index - 1] is SplitComponent splits && splits.Split == state.CurrentSplit)
                    {
                        thinSeparator.LockToBottom = false;
                    }
                }
            }
        }
    }

    private void state_OnUndoSplit(object sender, EventArgs e)
    {
        ScrollOffset = 0;
    }

    private void state_OnSkipSplit(object sender, EventArgs e)
    {
        ScrollOffset = 0;
    }

    private void state_OnSplit(object sender, EventArgs e)
    {
        ScrollOffset = 0;
    }

    private void state_OnReset(object sender, TimerPhase e)
    {
        ScrollOffset = 0;
    }

    private sealed class SectionList
    {
        public sealed class Section
        {
            public readonly int startIndex;
            public readonly int endIndex;

            public Section(int topIndex, int bottomIndex)
            {
                startIndex = topIndex;
                endIndex = bottomIndex;
            }

            public bool splitInRange(int splitIndex)
            {
                return splitIndex >= startIndex && splitIndex <= endIndex;
            }

            public int getSubsplitCount()
            {
                return endIndex - startIndex;
            }
        }

        public List<Section> Sections;

        public void UpdateSplits(IRun splits)
        {
            Sections = [];
            for (int splitIndex = splits.Count() - 1; splitIndex >= 0; splitIndex--)
            {
                int sectionIndex = splitIndex;
                while ((splitIndex > 0) && splits[splitIndex - 1].Name.StartsWith("-"))
                {
                    splitIndex--;
                }

                Sections.Insert(0, new Section(splitIndex, sectionIndex));
            }
        }

        public int getSection(int splitIndex)
        {
            foreach (Section section in Sections)
            {
                if (section.splitInRange(splitIndex))
                {
                    return Sections.IndexOf(section);
                }
            }

            return -1;
        }

        public bool isMajorSplit(int splitIndex)
        {
            int sectionIndex = getSection(splitIndex);

            if (sectionIndex == -1)
            {
                return true;
            }

            return splitIndex == Sections[sectionIndex].endIndex;
        }

        public int getMajorSplit(int splitIndex)
        {
            int sectionIndex = getSection(splitIndex);

            if (sectionIndex == -1)
            {
                return splitIndex;
            }

            return Sections[sectionIndex].endIndex;
        }
    }

    private void state_OnStart(object sender, EventArgs e)
    {
        ScrollOffset = 0;
    }

    private void state_OnScrollUp(object sender, EventArgs e)
    {
        ScrollOffset--;
    }

    private void state_OnScrollDown(object sender, EventArgs e)
    {
        ScrollOffset++;
    }

    private void DrawBackground(Graphics g, float width, float height)
    {
        if (Settings.BackgroundGradient != ExtendedGradientType.Alternating
            && (Settings.BackgroundColor.A > 0
            || (Settings.BackgroundGradient != ExtendedGradientType.Plain
            && Settings.BackgroundColor2.A > 0)))
        {
            var gradientBrush = new LinearGradientBrush(
                        new PointF(0, 0),
                        Settings.BackgroundGradient == ExtendedGradientType.Horizontal
                        ? new PointF(width, 0)
                        : new PointF(0, height),
                        Settings.BackgroundColor,
                        Settings.BackgroundGradient == ExtendedGradientType.Plain
                        ? Settings.BackgroundColor
                        : Settings.BackgroundColor2);
            g.FillRectangle(gradientBrush, 0, 0, width, height);
        }
    }

    public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
    {
        Prepare(state);
        DrawBackground(g, width, VerticalHeight);
        InternalComponent.DrawVertical(g, state, width, clipRegion);
    }

    public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
    {
        Prepare(state);
        DrawBackground(g, HorizontalWidth, height);
        InternalComponent.DrawHorizontal(g, state, height, clipRegion);
    }

    public Control GetSettingsControl(LayoutMode mode)
    {
        Settings.Mode = mode;
        return Settings;
    }

    public void SetSettings(System.Xml.XmlNode settings)
    {
        Settings.SetSettings(settings);
        RebuildVisualSplits();
    }

    public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
    {
        return Settings.GetSettings(document);
    }

    public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
    {
        if (state.Run != previousRun)
        {
            sectionList.UpdateSplits(state.Run);
            previousRun = state.Run;
        }

        int runningSectionIndex = Math.Min(Math.Max(state.CurrentSplitIndex, 0), state.Run.Count - 1);
        ScrollOffset = Math.Min(Math.Max(ScrollOffset, -runningSectionIndex), state.Run.Count - runningSectionIndex - 1);
        int currentSplit = ScrollOffset + runningSectionIndex;
        int currentSection = sectionList.getSection(currentSplit);
        runningSectionIndex = sectionList.getSection(runningSectionIndex);
        if (sectionList.Sections[currentSection].getSubsplitCount() > 0)
        {
            lastSplitOfSection = sectionList.Sections[currentSection].endIndex;
        }
        else
        {
            lastSplitOfSection = -1;
        }

        if (Settings.HideSubsplits)
        {
            if (ScrollOffset != 0)
            {
                currentSplit = sectionList.getMajorSplit(currentSplit);
                SplitsSettings.HilightSplit = state.Run[currentSplit];
            }
            else
            {
                SplitsSettings.HilightSplit = null;
            }

            SplitsSettings.SectionSplit = state.Run[sectionList.Sections[runningSectionIndex].endIndex];
        }
        else
        {
            if (ScrollOffset != 0)
            {
                SplitsSettings.HilightSplit = state.Run[currentSplit];
            }
            else
            {
                SplitsSettings.HilightSplit = null;
            }

            if (currentSection == runningSectionIndex)
            {
                SplitsSettings.SectionSplit = null;
            }
            else
            {
                SplitsSettings.SectionSplit = state.Run[sectionList.Sections[runningSectionIndex].endIndex];
            }
        }

        bool addLast = Settings.AlwaysShowLastSplit || currentSplit == state.Run.Count() - 1;
        bool addHeader = Settings.ShowHeader && (sectionList.Sections[currentSection].getSubsplitCount() > 0);

        int freeSplits = visualSplitCount - (addLast ? 1 : 0) - (addHeader ? 1 : 0);
        int topSplit = currentSplit - 1;
        int bottomSplit = currentSplit + 1;
        int majorSplitsToAdd = (!Settings.ShowSubsplits && !Settings.HideSubsplits) ? Math.Min(currentSection, Settings.MinimumMajorSplits) : 0;

        List<int> visibleSplits = [];
        if ((currentSplit < state.Run.Count() - 1) && (freeSplits > 0) && (!Settings.HideSubsplits || sectionList.isMajorSplit(currentSplit)))
        {
            visibleSplits.Add(currentSplit);
            freeSplits--;
        }

        int previewCount = 0;
        while ((previewCount < Settings.SplitPreviewCount) && (bottomSplit < state.Run.Count() - (addLast ? 1 : 0)) && (freeSplits > majorSplitsToAdd))
        {
            if (ShouldIncludeSplit(currentSection, bottomSplit))
            {
                if (bottomSplit == state.Run.Count - 1)
                {
                    addLast = true;
                }
                else
                {
                    visibleSplits.Add(bottomSplit);
                    previewCount++;
                }

                freeSplits--;
            }

            bottomSplit++;
        }

        while ((topSplit >= 0) && (freeSplits > 0))
        {
            bool isMajor = sectionList.isMajorSplit(topSplit);
            if (ShouldIncludeSplit(currentSection, topSplit) && (freeSplits > majorSplitsToAdd || sectionList.isMajorSplit(topSplit)))
            {
                visibleSplits.Insert(0, topSplit);
                freeSplits--;
                if (isMajor)
                {
                    majorSplitsToAdd--;
                }
            }

            topSplit--;
        }

        while ((bottomSplit < state.Run.Count() - (addLast ? 1 : 0)) && (freeSplits > 0))
        {
            if (ShouldIncludeSplit(currentSection, bottomSplit))
            {
                if (bottomSplit == state.Run.Count - 1)
                {
                    addLast = true;
                }
                else
                {
                    visibleSplits.Add(bottomSplit);
                }

                freeSplits--;
            }

            bottomSplit++;
        }

        foreach (IComponent component in Components)
        {
            if (component is SeparatorComponent separator)
            {
                int index = Components.IndexOf(separator);

                if (Settings.AlwaysShowLastSplit && Settings.SeparatorLastSplit && index == LastSplitSeparatorIndex)
                {
                    int lastIndex = state.Run.Count() - 1;

                    if (freeSplits > 0 || (visibleSplits.Any() && (visibleSplits.Last() == lastIndex - 1)))
                    {
                        if (Settings.ShowThinSeparators)
                        {
                            separator.DisplayedSize = 1f;
                        }
                        else
                        {
                            separator.DisplayedSize = 0f;
                        }

                        separator.UseSeparatorColor = false;
                    }
                    else
                    {
                        int prevSection = sectionList.getSection(lastIndex) - 1;
                        if (visibleSplits.Any() && (prevSection <= 0 || visibleSplits.Last() == sectionList.Sections[prevSection].endIndex))
                        {
                            if (Settings.ShowThinSeparators)
                            {
                                separator.DisplayedSize = 1f;
                            }
                            else
                            {
                                separator.DisplayedSize = 0f;
                            }

                            separator.UseSeparatorColor = false;
                        }
                        else
                        {
                            separator.DisplayedSize = 2f;
                            separator.UseSeparatorColor = true;
                        }
                    }
                }
            }
        }

        if (!Settings.LockLastSplit && addLast)
        {
            visibleSplits.Add(state.Run.Count() - 1);
        }

        for (; freeSplits > 0; freeSplits--)
        {
            visibleSplits.Add(int.MinValue);
        }

        if (Settings.LockLastSplit && addLast)
        {
            visibleSplits.Add(state.Run.Count() - 1);
        }

        if (addHeader)
        {
            int insertIndex = 0;
            if (currentSection > 0)
            {
                insertIndex = visibleSplits.IndexOf(sectionList.Sections[currentSection - 1].endIndex) + 1;
            }

            visibleSplits.Insert(insertIndex, -(currentSection + 1));
        }

        int i = 0;
        foreach (int split in visibleSplits)
        {
            if (i < SplitComponents.Count)
            {
                SplitComponents[i].ForceIndent = !Settings.ShowSubsplits && !Settings.HideSubsplits && Settings.IndentSectionSplit && split == lastSplitOfSection;

                if (split == int.MinValue)
                {
                    SplitComponents[i].Header = false;
                    SplitComponents[i].CollapsedSplit = false;
                    SplitComponents[i].Split = null;
                    SplitComponents[i].oddSplit = true;
                }
                else if (split < 0)
                {
                    SplitComponents[i].Header = true;
                    SplitComponents[i].CollapsedSplit = false;
                    SplitComponents[i].TopSplit = sectionList.Sections[-split - 1].startIndex;
                    SplitComponents[i].Split = state.Run[sectionList.Sections[-split - 1].endIndex];
                    SplitComponents[i].oddSplit = ((-split - 1 + (Settings.ShowColumnLabels ? 1 : 0)) % 2) == 0;
                }
                else
                {
                    SplitComponents[i].Header = false;
                    SplitComponents[i].Split = state.Run[split];
                    SplitComponents[i].oddSplit = ((sectionList.getSection(split) + (Settings.ShowColumnLabels ? 1 : 0)) % 2) == 0;

                    if ((Settings.HideSubsplits || sectionList.getSection(split) != currentSection)
                        && sectionList.Sections[sectionList.getSection(split)].getSubsplitCount() > 0
                        && !Settings.ShowSubsplits)
                    {
                        SplitComponents[i].CollapsedSplit = true;
                        SplitComponents[i].TopSplit = sectionList.Sections[sectionList.getSection(split)].startIndex;
                    }
                    else
                    {
                        SplitComponents[i].CollapsedSplit = false;
                    }
                }
            }

            i++;
        }

        if (invalidator != null)
        {
            InternalComponent.Update(invalidator, state, width, height, mode);
        }
    }

    private bool ShouldIncludeSplit(int currentSection, int split)
    {
        return (sectionList.isMajorSplit(split)
                   && (!Settings.CurrentSectionOnly || sectionList.getSection(split) == currentSection)) ||
               (!sectionList.isMajorSplit(split)
                   && (Settings.ShowSubsplits || (!Settings.HideSubsplits && sectionList.getSection(split) == currentSection)));
    }

    public void Dispose()
    {
        CurrentState.OnScrollDown -= state_OnScrollDown;
        CurrentState.OnScrollUp -= state_OnScrollUp;
        CurrentState.OnStart -= state_OnStart;
        CurrentState.OnReset -= state_OnReset;
        CurrentState.OnSplit -= state_OnSplit;
        CurrentState.OnSkipSplit -= state_OnSkipSplit;
        CurrentState.OnUndoSplit -= state_OnUndoSplit;
        CurrentState.ComparisonRenamed -= state_ComparisonRenamed;
        CurrentState.RunManuallyModified -= state_RunManuallyModified;
    }

    public int GetSettingsHashCode()
    {
        return Settings.GetSettingsHashCode();
    }
}
