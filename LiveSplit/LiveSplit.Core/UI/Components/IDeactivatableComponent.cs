namespace LiveSplit.UI.Components
{
    public interface IDeactivatableComponent : IComponent
    {
        bool Activated { get; set; }
    }
}
