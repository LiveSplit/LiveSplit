// Simplistic, yet working C# sample
// Author: Mark Hurley	(markph@mailcan.com)
// May 30, 2005

using System;

namespace NinetyNineBottlesOfBeer
{
	/// <summary>
	/// Infamous 99 bottles of beer song in C#.Net
	/// </summary>
	class NinetyNineBottlesOfBeerSong
	{
		/// <summary>
		/// beer verse more beer left
		/// </summary>
		private const string BEER_LYRICS_MORE = @"
{0} bottle{1} of beer on the wall,
{0} bottle{1} of beer.
Take one down, pass it around,
{2} bottle{3} of beer on the wall.";

		/// <summary>
		/// beer verse no more beer left
		/// </summary>
		private const string BEER_LYRICS_NONE = @"
{0} bottle{1} of beer on the wall,
{0} bottle{1} of beer.
Take one down, pass it around,
No more bottles of beer on the wall.";

		/// <summary>
		/// Determine the proper verse, then merge it with <c>count</c>.
		/// </summary>
		/// <param name="count">Number of bottles remaining.</param>
		/// <returns>Properly formated string verse for song.</returns>
		public string Sing(int count)
		{
			string tmp = "";
			if (count == 1)
				return string.Format(BEER_LYRICS_NONE, 
					count, 
					(count==1) ? "" : "s");
			else if (count > 0)
				return string.Format(BEER_LYRICS_MORE, 
					count, 
					(count==1) ? "" : "s",
					(count-1),
					((count-1)==1) ? "" : "s");
			else
				tmp = "";

			return tmp;
		}

		[STAThread]
		static void Main(string[] args)
		{
			NinetyNineBottlesOfBeerSong song = new NinetyNineBottlesOfBeerSong();

			for(int i=99; i>0; i--)
			{
				Console.WriteLine(song.Sing(i));
				Console.ReadLine();
			}
		}
	}
}
