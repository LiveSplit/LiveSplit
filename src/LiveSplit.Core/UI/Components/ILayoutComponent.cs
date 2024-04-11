namespace LiveSplit.UI.Components
{
    public interface ILayoutComponent
    {
        string Path { get; set; }
        IComponent Component { get; set; }
    }
}
