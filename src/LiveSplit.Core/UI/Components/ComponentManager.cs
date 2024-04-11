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
        public static IDictionary<string, IRaceProviderFactory> RaceProviderFactories { get; set; }

        public static ILayoutComponent LoadLayoutComponent(string path, LiveSplitState state)
        {
            if (ComponentFactories == null)
                ComponentFactories = LoadAllFactories<IComponentFactory>();
            IComponent component = null;

            if (string.IsNullOrEmpty(path))
                component = new SeparatorComponent();
            else if (!ComponentFactories.ContainsKey(path))
                return null;
            else
                component = ComponentFactories[path].Create(state);

            return new LayoutComponent(path, component);
        }

        public static IDictionary<string, T> LoadAllFactories<T>()
        {
            var path = Path.GetFullPath(Path.Combine(BasePath ?? "", PATH_COMPONENTS));
            return Directory
                .EnumerateFiles(path, "*.dll")
                .Select(x =>
                {
                    var factory = LoadFactory<T>(x);
                    return new KeyValuePair<string, T>(Path.GetFileName(x), factory);
                })
                .Where(x => x.Value != null)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public static T LoadFactory<T>(string path)
        {
            T factory = default(T);
            try
            {
                var attr = (ComponentFactoryAttribute)Attribute
                    .GetCustomAttribute(Assembly.UnsafeLoadFrom(path), typeof(ComponentFactoryAttribute));

                if (attr != null)
                {
                    factory = (T)(attr.
                        ComponentFactoryClassType.
                        GetConstructor(new Type[0]).
                        Invoke(null));
                }
            }
            catch { }
            return factory;
        }
    }
}
