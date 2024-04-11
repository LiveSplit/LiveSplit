using LiveSplit.Model.Comparisons;

namespace LiveSplit.Model.RunFactories
{
    public interface IRunFactory
    {
        IRun Create(IComparisonGeneratorsFactory factory);
    }
}
