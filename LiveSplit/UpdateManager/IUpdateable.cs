using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateManager
{
    public interface IUpdateable
    {
        string UpdateName { get; }
        string XMLURL { get; }
        String UpdateURL { get; }
        Version Version { get; }
    }
}
