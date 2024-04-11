using System;

namespace LiveSplit.UI.Components
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=false)]
    public sealed class ComponentFactoryAttribute : Attribute
    {
        public Type ComponentFactoryClassType { get; private set; }

        public ComponentFactoryAttribute(Type componentFactoryClassType)
        {
            ComponentFactoryClassType = componentFactoryClassType;
        }
    }
}
