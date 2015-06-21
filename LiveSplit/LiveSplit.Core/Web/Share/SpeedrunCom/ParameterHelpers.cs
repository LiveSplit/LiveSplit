using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Web.Share.SpeedrunCom
{
    internal static class ParameterHelpers
    {
        internal static string ToParameters(this string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
                return "";
            else
                return "?" + parameters;
        }

        internal static string ToParameters(this IEnumerable<string> parameters)
        {
            var list = parameters.Where(x => !string.IsNullOrEmpty(x)).ToList();
            if (list.Any())
                return "?" + list.Aggregate((a, b) => a + "&" + b);
            else
                return "";
        }
    }
}
