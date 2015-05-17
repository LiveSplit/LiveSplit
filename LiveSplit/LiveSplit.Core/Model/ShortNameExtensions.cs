using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Model
{
    public static class ShortNameExtensions
    {
        public static IEnumerable<string> GetShortNames(this string name)
        {
            name = name.Trim();

            var list = new List<string>() { name };

            if (name.Contains(": "))
            {
                var splits = name.Split(new[] { ": " }, 2, StringSplitOptions.None);
                var firstPart = splits[0];
                var secondPart = splits[1];
                var firstPartShortNames = firstPart.GetShortNames();
                var secondPartShortNames = secondPart.GetShortNames();
                list.AddRange(secondPartShortNames);

                foreach (var secondPartShortName in secondPartShortNames)
                {
                    foreach (var firstPartShortName in firstPartShortNames)
                    {
                        list.Add(firstPartShortName + ": " + secondPartShortName);
                    }
                }
            }
            else if (name.Contains(" - "))
            {
                var splits = name.Split(new[] { " - " }, 2, StringSplitOptions.None);
                var firstPart = splits[0];
                var secondPart = splits[1];
                var firstPartShortNames = firstPart.GetShortNames();
                var secondPartShortNames = secondPart.GetShortNames();
                list.AddRange(secondPartShortNames);

                foreach (var secondPartShortName in secondPartShortNames)
                {
                    foreach (var firstPartShortName in firstPartShortNames)
                    {
                        list.Add(firstPartShortName + " - " + secondPartShortName);
                    }
                }
            }
            else if (name.ToLower().Contains(" and "))
            {
                var index = name.ToLower().IndexOf(" and ");
                var firstPart = name.Substring(0, index);
                var secondPart = name.Substring(index + " and ".Length);
                name = firstPart + " & " + secondPart;
                list.AddRange(name.GetShortNames());
            }
            else if (name.Contains(" "))
            {
                var splits = name.Split(new[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
                var abbreviation = splits
                    .Select(x =>
                        char.IsDigit(x[0])
                        ? x.TakeWhile(c => char.IsDigit(c)).Aggregate("", (a, b) => a + b)
                        : x[0].ToString())
                    .Aggregate("", (a, b) => a + b);
                list.Add(abbreviation);
            }

            return list.OrderByDescending(x => x.Length).Distinct().ToArray();
        }
    }
}
