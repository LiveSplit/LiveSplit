using System;
using System.Collections.Generic;
using System.Linq;

namespace LiveSplit.Model;

public static class AbbreviationExtensions
{
    private static bool endsWithRomanNumeral(string name)
    {
        char[] romanSymbols = ['I', 'V', 'X'];
        char charBeforeRomanNumeral = name
            .Reverse()
            .SkipWhile(c => romanSymbols.Contains(c))
            .FirstOrDefault();

        return charBeforeRomanNumeral is ' '
            or default(char);
    }

    private static bool isAllCapsOrDigit(string name)
    {
        return name.All(c => char.IsUpper(c) || char.IsDigit(c));
    }

    private static bool tokenize(string name, string splitToken, List<string> list)
    {
        if (name.Contains(splitToken))
        {
            string[] splits = name.Split(new[] { splitToken }, 2, StringSplitOptions.None);
            string seriesTitle = splits[0];
            string subTitle = splits[1];
            var seriesTitleAbbreviations = seriesTitle.GetAbbreviations().ToList();
            var subTitleAbbreviations = subTitle.GetAbbreviations().ToList();
            string seriesTitleTrimmed = seriesTitle.Trim();

            bool isSeriesTitleRepresentative = !string.IsNullOrEmpty(seriesTitleTrimmed)
                && (char.IsDigit(seriesTitleTrimmed.Last())
                    || endsWithRomanNumeral(seriesTitleTrimmed));

            if (isSeriesTitleRepresentative)
            {
                list.AddRange(seriesTitleAbbreviations);
            }

            list.AddRange(subTitleAbbreviations);

            bool isThereOnlyOneSeriesTitleAbbreviation = seriesTitleAbbreviations.Count == 1;

            foreach (string subTitleAbbreviation in subTitleAbbreviations)
            {
                foreach (string seriesTitleAbbreviation in seriesTitleAbbreviations)
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

    private static bool tokenizeAndKeepBoth(string name, string splitToken, List<string> list)
    {
        if (name.Contains(splitToken))
        {
            string[] splits = name.Split(new[] { splitToken }, 2, StringSplitOptions.None);
            string seriesTitle = splits[0];
            string subTitle = splits[1];
            var seriesTitleAbbreviations = seriesTitle.GetAbbreviations().ToList();
            var subTitleAbbreviations = subTitle.GetAbbreviations().ToList();

            foreach (string subTitleAbbreviation in subTitleAbbreviations)
            {
                foreach (string seriesTitleAbbreviation in seriesTitleAbbreviations)
                {
                    list.Add(seriesTitleAbbreviation + splitToken + subTitleAbbreviation);
                }
            }

            return true;
        }

        return false;
    }

    public static IEnumerable<string> GetAbbreviations(this string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            name = string.Empty;
            return new[] { name };
        }

        name = name.Trim();

        var list = new List<string>() { name };

        int indexStart = name.LastIndexOf('(');
        int indexEnd = name.IndexOf(')', indexStart + 1);
        if (indexStart >= 0 && indexEnd >= 0)
        {
            string beforeParentheses = name[..indexStart].Trim();
            string afterParentheses = name[(indexEnd + 1)..].Trim();
            name = $"{beforeParentheses} {afterParentheses}".Trim();
            list.AddRange(name.GetAbbreviations());
        }
        else if (tokenize(name, ": ", list))
        {
        }
        else if (tokenize(name, " - ", list))
        {
        }
        else if (tokenizeAndKeepBoth(name, " | ", list))
        {
        }
        else if (name.ToLowerInvariant().Contains(" and "))
        {
            int index = name.ToLower().IndexOf(" and ");
            string firstPart = name[..index];
            string secondPart = name[(index + " and ".Length)..];
            name = firstPart + " & " + secondPart;
            list.AddRange(name.GetAbbreviations());
        }
        else
        {
            if (name.ToLowerInvariant().StartsWith("the "))
            {
                string theDropped = name["the ".Length..];
                list.Add(theDropped);
            }
            else if (name.ToLowerInvariant().StartsWith("a "))
            {
                string aDropped = name["a ".Length..];
                list.Add(aDropped);
            }

            if (name.Contains(" "))
            {
                string[] splits = name
                    .Replace('&', 'a')
                    .Split(new[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
                string abbreviation = splits
                    .Select(x =>
                        {
                            if (char.IsDigit(x[0]))
                            {
                                return x
                                    .TakeWhile(c => c != ' ')
                                    .Aggregate("", (a, b) => a + b);
                            }

                            if (x.Length <= 4 && isAllCapsOrDigit(x))
                            {
                                return " " + x;
                            }

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
