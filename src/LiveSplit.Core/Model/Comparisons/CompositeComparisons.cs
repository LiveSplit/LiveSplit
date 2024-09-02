﻿using System;
using System.Collections.Generic;

namespace LiveSplit.Model.Comparisons;

public class CompositeComparisons : IComparisons
{
    protected IDictionary<string, Time> Comparisons { get; set; }

    public CompositeComparisons()
    {
        Comparisons = new Dictionary<string, Time>();
    }

    public void Add(string key, Time value)
    {
        Comparisons.Add(key, value);
    }

    public bool ContainsKey(string key)
    {
        return Comparisons.ContainsKey(key);
    }

    public ICollection<string> Keys => Comparisons.Keys;

    public bool Remove(string key)
    {
        return Comparisons.Remove(key);
    }

    public bool TryGetValue(string key, out Time value)
    {
        return Comparisons.TryGetValue(key, out value);
    }

    public ICollection<Time> Values => Comparisons.Values;

    public Time this[string key]
    {
        get => Comparisons.ContainsKey(key) ? Comparisons[key] : default;
        set
        {
            if (Comparisons.ContainsKey(key))
            {
                Comparisons[key] = value;
            }
            else
            {
                Comparisons.Add(key, value);
            }
        }
    }

    public void Add(KeyValuePair<string, Time> item)
    {
        Comparisons.Add(item);
    }

    public void Clear()
    {
        Comparisons.Clear();
    }

    public bool Contains(KeyValuePair<string, Time> item)
    {
        return Comparisons.Contains(item);
    }

    public void CopyTo(KeyValuePair<string, Time>[] array, int arrayIndex)
    {
        Comparisons.CopyTo(array, arrayIndex);
    }

    public int Count => Comparisons.Count;

    public bool IsReadOnly => Comparisons.IsReadOnly;

    public bool Remove(KeyValuePair<string, Time> item)
    {
        return Comparisons.Remove(item);
    }

    public IEnumerator<KeyValuePair<string, Time>> GetEnumerator()
    {
        return Comparisons.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public CompositeComparisons Clone()
    {
        var clone = new CompositeComparisons
        {
            Comparisons = new Dictionary<string, Time>(Comparisons)
        };
        return clone;
    }

    object ICloneable.Clone()
    {
        return Clone();
    }

    private static Func<string, string> GetShortComparisonNameFunc { get; set; }

    public static void AddShortComparisonName(string longName, string shortName)
    {
        if (GetShortComparisonNameFunc == null)
        {
            GetShortComparisonNameFunc = x =>
            {
                if (x == longName)
                {
                    return shortName;
                }

                return x;
            };
        }
        else
        {
            Func<string, string> oldFunc = GetShortComparisonNameFunc;
            GetShortComparisonNameFunc = x =>
            {
                if (x == longName)
                {
                    return shortName;
                }

                return oldFunc(x);
            };
        }
    }

    public static string GetShortComparisonName(string longName)
    {
        if (GetShortComparisonNameFunc == null)
        {
            return longName;
        }
        else
        {
            return GetShortComparisonNameFunc(longName);
        }
    }
}
