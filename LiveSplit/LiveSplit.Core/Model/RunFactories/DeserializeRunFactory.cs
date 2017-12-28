using LiveSplit.Model.Comparisons;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace LiveSplit.Model.RunFactories
{
    public class DeserializeRunFactory : IRunFactory
    {
        public Stream Stream { get; protected set; }

        public DeserializeRunFactory(Stream stream)
        {
            Stream = stream;
        }

        public IRun Create(IComparisonGeneratorsFactory factory)
        {
            var formatter = new BinaryFormatter();
            return formatter.Deserialize(Stream) as IRun;
        }
    }
}
