using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Model
{
    public static class ShortNameExtensions
    {
        private static bool endsWithRomanNumeral(string name)
        {
            var romanSymbols = new[] { 'I', 'V', 'X' };
            var charBeforeRomanNumeral = name
                .Reverse()
                .SkipWhile(c => romanSymbols.Contains(c))
                .FirstOrDefault();

            return charBeforeRomanNumeral == ' '
                || charBeforeRomanNumeral == default(char);
        }

        private static bool isAllCapsOrDigit(string name)
        {
            return name.All(c => (char.IsUpper(c) || char.IsDigit(c)));
        }

        private static bool tokenize(string name, string splitToken, List<string> list)
        {
            if (name.Contains(splitToken))
            {
                var splits = name.Split(new[] { splitToken }, 2, StringSplitOptions.None);
                var seriesTitle = splits[0];
                var subTitle = splits[1];
                var seriesTitleAbbreviations = seriesTitle.GetShortNames().ToList();
                var subTitleAbbreviations = subTitle.GetShortNames().ToList();
                var seriesTitleTrimmed = seriesTitle.Trim();

                var isSeriesTitleRepresentative = !string.IsNullOrEmpty(seriesTitleTrimmed)
                    && (char.IsDigit(seriesTitleTrimmed.Last())
                        || endsWithRomanNumeral(seriesTitleTrimmed));

                if (isSeriesTitleRepresentative)
                {
                    list.AddRange(seriesTitleAbbreviations);
                }
                list.AddRange(subTitleAbbreviations);

                var isThereOnlyOneSeriesTitleAbbreviation = seriesTitleAbbreviations.Count() == 1;

                foreach (var subTitleAbbreviation in subTitleAbbreviations)
                {
                    foreach (var seriesTitleAbbreviation in seriesTitleAbbreviations)
                    {
                        if (isSeriesTitleRepresentative 
                            || seriesTitleAbbreviation != seriesTitle 
                            || isThereOnlyOneSeriesTitleAbbreviation)
                        {
                            list.Add(seriesTitleAbbreviation + splitToken + subTitleAbbreviation);
                        }
                    }
                }

                return true;
            }
            return false;
        }

        public static IEnumerable<string> GetShortNames(this string name)
        {
            name = name.Trim();

            var list = new List<string>() { name };

            if (name.Contains(')'))
            {
                var endingBracketRemoved = name.Substring(0, name.LastIndexOf(')'));
                var startingBrackedRemoved = endingBracketRemoved.Substring(0, endingBracketRemoved.LastIndexOf('('));
                list.AddRange(GetShortNames(startingBrackedRemoved));
            }
            else if (tokenize(name, ": ", list)) { }
            else if (tokenize(name, " - ", list)) { }
            else if (name.ToLower().Contains(" and "))
            {
                var index = name.ToLower().IndexOf(" and ");
                var firstPart = name.Substring(0, index);
                var secondPart = name.Substring(index + " and ".Length);
                name = firstPart + " & " + secondPart;
                list.AddRange(name.GetShortNames());
            }
            else
            {
                if (name.ToLower().StartsWith("the "))
                {
                    var theDropped = name.Substring("the ".Length);
                    list.Add(theDropped);
                }
                else if (name.ToLower().StartsWith("a "))
                {
                    var aDropped = name.Substring("a ".Length);
                    list.Add(aDropped);
                }

                if (name.Contains(" "))
                {
                    var splits = name
                        .Replace('&', 'a')
                        .Split(new[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
                    var abbreviation = splits
                        .Select(x =>
                            {
                                if (char.IsDigit(x[0]))
                                    return x
                                        .TakeWhile(c => c != ' ')
                                        .Aggregate("", (a, b) => a + b);
                                if (x.Length <= 4 && isAllCapsOrDigit(x))
                                    return " " + x;
                                return x[0].ToString();
                            })
                        .Aggregate("", (a, b) => a + b)
                        .Trim();
                    list.Add(abbreviation);
                }
            }

            return list.OrderByDescending(x => x.Length).Distinct().ToArray();
        }
    }
}
