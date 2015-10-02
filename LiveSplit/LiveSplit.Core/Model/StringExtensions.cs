using System;
using System.Collections.Generic;
using System.Linq;

namespace LiveSplit.Model
{
    public static class StringExtensions
    {
        /// <summary>
        /// Calculates the similarity of two strings. The lower the result is, the more similar they are.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static int Similarity(this string s, string other)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (string.IsNullOrEmpty(other))
                    return 0;
                return other.Length;
            }

            if (string.IsNullOrEmpty(other))
            {
                return s.Length;
            }

            s = s.ToLowerInvariant();
            other = other.ToLowerInvariant();

            int n = s.Length;
            int m = other.Length;
            int[,] d = new int[n + 1, m + 1];

            // initialize the top and right of the table to 0, 1, 2, ...
            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 1; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (other[j - 1] == s[i - 1]) ? 0 : 1;
                    int min1 = d[i - 1, j] + 1;
                    int min2 = d[i, j - 1] + 1;
                    int min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return d[n, m];
        }

        public static IEnumerable<string> OrderBySimilarityTo(this IEnumerable<string> list, string value)
            => list.OrderBy(x => x.Similarity(value));

        public static string FindMostSimilarValueTo(this IEnumerable<string> list, string value)
            => list.OrderBySimilarityTo(value).FirstOrDefault();
        
        public static string EscapeMenuItemText(this string text) 
            => text.Replace("&", "&&");
    }
}
