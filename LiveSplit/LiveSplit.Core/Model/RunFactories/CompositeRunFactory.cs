using LiveSplit.Model.Comparisons;
using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.IO;

namespace LiveSplit.Model.RunFactories
{
    public class CompositeRunFactory : IRunFactory, IDictionary<IRunFactory, Action<Stream, string>>
    {
        protected IDictionary<IRunFactory, Action<Stream, string>> RunFactories { get; set; }

        public Stream Stream { get; set; }
        public string FilePath { get; set; }

        public CompositeRunFactory()
        {
            RunFactories = new Dictionary<IRunFactory, Action<Stream, string>>();
        }

        public IRun Create(IComparisonGeneratorsFactory factory)
        {
            foreach (var runFactory in RunFactories)
            {
                try
                {
                    Stream?.Seek(0, SeekOrigin.Begin);

                    runFactory.Value(Stream, FilePath);
                    var run = runFactory.Key.Create(factory);
                    if (run.Count < 1)
                        throw new Exception("Run factory created a run without at least one segment");
                    return run;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }

            throw new ArgumentException();
        }

        public void Add(IRunFactory key, Action<Stream, string> value) => RunFactories.Add(key, value);

        public bool ContainsKey(IRunFactory key) => RunFactories.ContainsKey(key);

        public ICollection<IRunFactory> Keys => RunFactories.Keys;

        public bool Remove(IRunFactory key) => RunFactories.Remove(key);

        public bool TryGetValue(IRunFactory key, out Action<Stream, string> value) => RunFactories.TryGetValue(key, out value);

        public ICollection<Action<Stream, string>> Values => RunFactories.Values;

        public Action<Stream, string> this[IRunFactory key]
        {
            get
            {
                return RunFactories[key];
            }
            set
            {
                RunFactories[key] = value;
            }
        }

        public void Add(KeyValuePair<IRunFactory, Action<Stream, string>> item) => RunFactories.Add(item);

        public void Clear() => RunFactories.Clear();

        public bool Contains(KeyValuePair<IRunFactory, Action<Stream, string>> item) => RunFactories.Contains(item);

        public void CopyTo(KeyValuePair<IRunFactory, Action<Stream, string>>[] array, int arrayIndex) => RunFactories.CopyTo(array, arrayIndex);

        public int Count => RunFactories.Count;

        public bool IsReadOnly => RunFactories.IsReadOnly;

        public bool Remove(KeyValuePair<IRunFactory, Action<Stream, string>> item) => RunFactories.Remove(item);

        public IEnumerator<KeyValuePair<IRunFactory, Action<Stream, string>>> GetEnumerator() => RunFactories.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
