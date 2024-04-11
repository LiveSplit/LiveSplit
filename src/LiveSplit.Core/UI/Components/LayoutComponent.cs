namespace LiveSplit.UI.Components
{
    public class LayoutComponent : ILayoutComponent
    {
        public IComponent Component { get; set; }
        public string Path { get; set; }

        public LayoutComponent(string path, IComponent component)
        {
            Component = component;
            Path = path;
        }

        public override string ToString()
        {
            return Component.ComponentName;
        }
    }
}
