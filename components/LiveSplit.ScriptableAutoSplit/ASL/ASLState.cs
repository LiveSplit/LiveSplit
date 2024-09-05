using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;

using LiveSplit.ComponentUtil;

namespace LiveSplit.ASL;

public class ASLValueDefinition
{
    public string Type { get; set; }
    public string Identifier { get; set; }
    public DeepPointer Pointer { get; set; }
}

public class ASLState : ICloneable
{
    public ExpandoObject Data { get; set; }
    public List<ASLValueDefinition> ValueDefinitions { get; set; }
    public string GameVersion { get; set; }

    public ASLState()
    {
        Data = new ExpandoObject();
        ValueDefinitions = [];
        GameVersion = string.Empty;
    }

    public ASLState RefreshValues(Process p)
    {
        var clone = (ASLState)Clone();
        var dict = (IDictionary<string, object>)Data;

        foreach (ASLValueDefinition value_definition in ValueDefinitions)
        {
            dynamic value = GetValue(p, value_definition.Type, value_definition.Pointer);

            if (dict.ContainsKey(value_definition.Identifier))
            {
                dict[value_definition.Identifier] = value;
            }
            else
            {
                dict.Add(value_definition.Identifier, value);
            }
        }

        return clone;
    }

    private static dynamic GetValue(Process p, string type, DeepPointer pointer)
    {
        switch (type)
        {
            case "int":
                return pointer.Deref<int>(p);
            case "uint":
                return pointer.Deref<uint>(p);
            case "long":
                return pointer.Deref<long>(p);
            case "ulong":
                return pointer.Deref<ulong>(p);
            case "float":
                return pointer.Deref<float>(p);
            case "double":
                return pointer.Deref<double>(p);
            case "byte":
                return pointer.Deref<byte>(p);
            case "sbyte":
                return pointer.Deref<sbyte>(p);
            case "short":
                return pointer.Deref<short>(p);
            case "ushort":
                return pointer.Deref<ushort>(p);
            case "bool":
                return pointer.Deref<bool>(p);
            default:
                if (type.StartsWith("string"))
                {
                    int length = int.Parse(type["string".Length..]);
                    return pointer.DerefString(p, length);
                }
                else if (type.StartsWith("byte"))
                {
                    int length = int.Parse(type["byte".Length..]);
                    return pointer.DerefBytes(p, length);
                }

                break;
        }

        throw new ArgumentException($"The provided type, '{type}', is not supported");
    }

    public object Clone()
    {
        var clone = new ExpandoObject();
        foreach (KeyValuePair<string, object> pair in Data)
        {
            ((IDictionary<string, object>)clone).Add(pair);
        }

        return new ASLState() { Data = clone, ValueDefinitions = new List<ASLValueDefinition>(ValueDefinitions) };
    }
}
