using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.UI.Components;
using LiveSplit.Updates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveSplit.UI.Components
{
    public class ComponentManager
    {
        public const String PATH_COMPONENTS = "Components\\";
        public static String BasePath { get; set; }
        public static IDictionary<String, IComponentFactory> ComponentFactories { get; protected set; }

        public static ILayoutComponent LoadLayoutComponent(String path, LiveSplitState state)
        {
            if (ComponentFactories == null)
                LoadAllFactories();
            IComponent component = null;

            if (String.IsNullOrEmpty(path))
                component = new SeparatorComponent();
            else
                component = ComponentFactories[path].Create(state);

            return new LayoutComponent(path, component);
        }

        public static IDictionary<String,IComponentFactory> LoadAllFactories()
        {
            var path = Path.GetFullPath(Path.Combine(BasePath ?? "", PATH_COMPONENTS));
            ComponentFactories = Directory
                .EnumerateFiles(path)
                .Select(x => 
                    {
                        var factory = LoadFactory(x);
                        return new KeyValuePair<String,IComponentFactory>(Path.GetFileName(x),factory);
                    })
                .Where(x => x.Value != null)
                .ToDictionary(x => x.Key, x => x.Value);

            return ComponentFactories;
        }

        public static IComponentFactory LoadFactory(String path)
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
