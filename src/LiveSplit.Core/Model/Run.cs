﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Xml;

using LiveSplit.Model.Comparisons;

namespace LiveSplit.Model;

/// <summary>
/// Describes a run for a game with all the splits and times.
/// </summary>
[Serializable]
public class Run : IRun, INotifyPropertyChanged
{
    private string gameName;
    private string categoryName;

    /// <summary>
    /// The name of the comparison used to save your Personal Best splits.
    /// </summary>
    public const string PersonalBestComparisonName = "Personal Best";

    /// <summary>
    /// This is the internal list being used to save the segments, which the run is a facade to.
    /// </summary>
    protected IList<ISegment> InternalList { get; set; }

    /// <summary>
    /// Gets or sets the icon of the game the run is for.
    /// </summary>
    public Image GameIcon { get; set; }

    /// <summary>
    /// Gets or sets the name of the game the run is for.
    /// </summary>
    public string GameName
    {
        get => gameName;
        set
        {
            gameName = value;
            Metadata.Refresh();
            TriggerPropertyChanged("GameName");
        }
    }

    /// <summary>
    /// Gets or sets the category of the run.
    /// </summary>
    public string CategoryName
    {
        get => categoryName;
        set
        {
            categoryName = value;
            Metadata.Refresh();
            TriggerPropertyChanged("CategoryName");
        }
    }

    /// <summary>
    /// Gets or sets the time where the timer starts at.
    /// <remarks>This can be both a negative time as well to simulate a countdown.</remarks>
    /// </summary>
    public TimeSpan Offset { get; set; }

    /// <summary>
    /// Gets or sets the amount of times the run has been started.
    /// </summary>
    public int AttemptCount { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public IList<Attempt> AttemptHistory { get; set; }

    public AutoSplitter AutoSplitter { get; set; }
    public XmlElement AutoSplitterSettings { get; set; }

    public IList<IComparisonGenerator> ComparisonGenerators { get; set; }
    public IList<string> CustomComparisons { get; set; }
    public IEnumerable<string> Comparisons => CustomComparisons.Concat(ComparisonGenerators.Select(x => x.Name));

    public RunMetadata Metadata { get; private set; }

    public event PropertyChangedEventHandler PropertyChanged;

    private void TriggerPropertyChanged(string propertyName)
    {
        PropertyChangedEventHandler propertyChanged = PropertyChanged;
        propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected IComparisonGeneratorsFactory Factory { get; set; }

    public bool HasChanged { get; set; }
    public string FilePath { get; set; }
    public string LayoutPath { get; set; }

    public Run(IComparisonGeneratorsFactory factory)
    {
        InternalList = [];
        AttemptHistory = [];
        Factory = factory;
        ComparisonGenerators = Factory.Create(this).ToList();
        CustomComparisons = [PersonalBestComparisonName];
        Metadata = new RunMetadata(this);
    }

    private Run(IEnumerable<ISegment> collection, IComparisonGeneratorsFactory factory, RunMetadata metadata)
    {
        InternalList = [];
        foreach (ISegment x in collection)
        {
            InternalList.Add(x.Clone() as ISegment);
        }

        AttemptHistory = [];
        Factory = factory;
        ComparisonGenerators = Factory.Create(this).ToList();
        CustomComparisons = [PersonalBestComparisonName];
        Metadata = metadata.Clone(this);
    }

    public int IndexOf(ISegment item)
    {
        return InternalList.IndexOf(item);
    }

    public void Insert(int index, ISegment item)
    {
        InternalList.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        InternalList.RemoveAt(index);
    }

    public ISegment this[int index]
    {
        get => InternalList[index];
        set => InternalList[index] = value;
    }

    public void Add(ISegment item)
    {
        InternalList.Add(item);
    }

    public void Clear()
    {
        InternalList.Clear();
    }

    public bool Contains(ISegment item)
    {
        return InternalList.Contains(item);
    }

    public void CopyTo(ISegment[] array, int arrayIndex)
    {
        InternalList.CopyTo(array, arrayIndex);
    }

    public int Count => InternalList.Count;

    public bool IsReadOnly => InternalList.IsReadOnly;

    public bool Remove(ISegment item)
    {
        return InternalList.Remove(item);
    }

    public IEnumerator<ISegment> GetEnumerator()
    {
        return InternalList.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Run Clone()
    {
        var newRun = new Run(this, Factory, Metadata)
        {
            GameIcon = GameIcon,
            GameName = GameName,
            CategoryName = CategoryName,
            Offset = Offset,
            AttemptCount = AttemptCount,
            AttemptHistory = new List<Attempt>(AttemptHistory),
            HasChanged = HasChanged,
            FilePath = FilePath,
            CustomComparisons = new List<string>(CustomComparisons),
            ComparisonGenerators = new List<IComparisonGenerator>(ComparisonGenerators),
            AutoSplitter = AutoSplitter?.Clone(),
            AutoSplitterSettings = AutoSplitterSettings,
            LayoutPath = LayoutPath
        };
        return newRun;
    }

    object ICloneable.Clone()
    {
        return Clone();
    }
}
