using LiveSplit.Model.Comparisons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

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
