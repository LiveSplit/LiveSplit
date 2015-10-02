using System;

namespace UpdateManager
{
    public interface IUpdateable
    {
        string UpdateName { get; }
        string XMLURL { get; }
        string UpdateURL { get; }
        Version Version { get; }
    }
}
