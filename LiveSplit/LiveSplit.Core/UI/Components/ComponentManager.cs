using LiveSplit.Model;
using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LiveSplit.UI.Components
{
    public class ComponentManager
    {
        public const string PATH_COMPONENTS = "Components\\";
        public static string BasePath { get; set; }
        public static IDictionary<string, IComponentFactory> ComponentFactories { get; protected set; }

        public static ILayoutComponent LoadLayoutComponent(string path, LiveSplitState state)
        {
            if (ComponentFactories == null)
                LoadAllFactories();
            IComponent component = null;

            if (string.IsNullOrEmpty(path))
                component = new SeparatorComponent();
            else
                component = ComponentFactories[path].Create(state);

            return new LayoutComponent(path, component);
        }

        public static IDictionary<string, IComponentFactory> LoadAllFactories()
        {
            var path = Path.GetFullPath(Path.Combine(BasePath ?? "", PATH_COMPONENTS));
            ComponentFactories = Directory
                .EnumerateFiles(path)
                .Select(x => 
                    {
                        var factory = LoadFactory(x);
                        return new KeyValuePair<string, IComponentFactory>(Path.GetFileName(x),factory);
                    })
                .Where(x => x.Value != null)
                .ToDictionary(x => x.Key, x => x.Value);

            return ComponentFactories;
        }

        public static IComponentFactory LoadFactory(string path)
        {
            IComponentFactory factory = null;
            try
            {
                factory = (IComponentFactory)(((ComponentFactoryAttribute)Attribute
                    .GetCustomAttribute(
                        Assembly.UnsafeLoadFrom(path),
                        typeof(ComponentFactoryAttribute)))
                    .ComponentFactoryClassType
                    .GetConstructor(new Type[0])
                    .Invoke(null));
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return factory;
        }
    }
}
