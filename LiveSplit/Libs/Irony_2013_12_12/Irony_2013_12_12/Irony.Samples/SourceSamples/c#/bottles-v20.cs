// C# 2.0 Ninety Nine Bottles of Beer Example
// Bradley Tetzlaff
// 2005-11-23

using System;

namespace NinetyNineBottlesOfBeer
{
    /// <summary>
    /// An example of the 99 Bottles of Beer song in C# 2.0,
    /// with intentional use of new features of version 2.0.
    /// </summary>
    [CLSCompliant(true)]
    public static class NinetyNineBottlesOfBeerSong
    {
        /// <summary>Singing method type.</summary>
        /// <param name="bi">The bottle index.</param>
        /// <returns>The song verse.</returns>
        private delegate string SingVerseMethod(int bi);

        /// <summary>The pluralization method type.</summary>
        /// <param name="bi">The bottle index.</param>
        /// <returns>
        /// An &quot;bottles&quot; if plural, otherwise &quot;bottle&quot;.
        /// </returns>
        private delegate string Pluralizer(int bi);

        /// <summary>
        /// Prints the 99 Bottles of Beer song to the console.
        /// </summary>
        public static void Main()
        {
            // Lyric Parts
            const string LYRICS_SOME = @"
{0} of beer on the wall,
{0} of beer.
Take one down, pass it around,
{1}";
            const string LYRICS_NEXT = " of beer on the wall.";
            const string LYRICS_NONE = "No more bottles of beer on the wall.";

            const string LYRICS_ZERO = @"
No more bottles of beer on the wall,
no more bottles of beer.
Go to the store and buy some more,
99 bottles of beer on the wall.";

            // Anonymous Methods
            // Returns the correct pluralization of "bottle".
            Pluralizer bottles = delegate(int bi)
                { return (bi == 1) ? "1 bottle" : bi + " bottles"; };

            // Sings one verse of the 99 Bottles of Beer song.
            SingVerseMethod sing = delegate(int bi)
            {
                if (bi > 0) // More than one beer.
                {
                    return string.Format(LYRICS_SOME, bottles(bi),
                        bottles(bi - 1) + LYRICS_NEXT);
                }
                else if (bi == 1)   // One beer.
                {
                    return string.Format(LYRICS_SOME, bottles(bi),
                        LYRICS_NONE);
                }
                else if (bi == 0)   // No beers.
                    return LYRICS_ZERO;
                else
                    throw new IndexOutOfRangeException("Invalid beer amount.");
            };
            // Sing all 99.
            for (int bi = 99; bi >= 0; bi--)
            {
                Console.WriteLine(sing(bi));
                Console.ReadLine();
            }
        }
    }
}
