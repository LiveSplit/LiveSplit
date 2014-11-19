using System;
using System.Collections.Generic;

namespace LiveSplit.Model.Comparisons
{
    public class CompositeComparisons : IComparisons
    {
        protected IDictionary<String, Time> Comparisons { get; set; }

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

        public ICollection<string> Keys
        {
            get { return Comparisons.Keys; }
        }

        public bool Remove(string key)
        {
            return Comparisons.Remove(key);
        }

        public bool TryGetValue(string key, out Time value)
        {
            return Comparisons.TryGetValue(key, out value);
        }

        public ICollection<Time> Values
        {
            get { return Comparisons.Values; }
        }

        public Time this[string key]
        {
            get
            {
                return Comparisons.ContainsKey(key) ? Comparisons[key] : default(Time);
            }
            set
            {
                if (Comparisons.ContainsKey(key))
                    Comparisons[key] = value;
                else
                    Comparisons.Add(key, value);
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

        public int Count
        {
            get { return Comparisons.Count; }
        }

        public bool IsReadOnly
        {
            get { return Comparisons.IsReadOnly; }
        }

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

        public object Clone()
        {
            var clone = new CompositeComparisons();
            clone.Comparisons = new Dictionary<String, Time>(Comparisons);
            return clone;
        }

        private static Func<String, String> GetShortComparisonNameFunc { get; set; }

        public static void AddShortComparisonName(String longName, String shortName)
        {
            if (GetShortComparisonNameFunc == null)
            {
                GetShortComparisonNameFunc = x =>
                {
                    if (x == longName)
                        return shortName;
                    return x;
                };
            }
            else
            {
                var oldFunc = GetShortComparisonNameFunc;
                GetShortComparisonNameFunc = x =>
                {
                    if (x == longName)
                        return shortName;
                    return oldFunc(x);
                };
            }
        }

        public static String GetShortComparisonName(String longName)
        {
            if (GetShortComparisonNameFunc == null)
                return longName;
            else
                return GetShortComparisonNameFunc(longName);
        }
    }
}
