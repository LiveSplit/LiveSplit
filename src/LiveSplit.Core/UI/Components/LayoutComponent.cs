using LiveSplit.Options;

namespace LiveSplit.UI.Components;

public class LayoutComponent : ILayoutComponent
{
    public IComponent Component { get; set; }
    public string Path { get; set; }
    public FontOverrides FontOverrides { get; set; }

    public LayoutComponent(string path, IComponent component)
    {
        Component = component;
        Path = path;
        FontOverrides = new FontOverrides();
    }

    public override string ToString()
    {
        return Component.ComponentName;
    }

    public LayoutComponent Clone()
    {
        return new LayoutComponent(Path, Component)
        {
            FontOverrides = (FontOverrides)FontOverrides.Clone()
        };
    }
}
