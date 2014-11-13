/// Implementation of Ninety-Nine Bottles of Beer Song in C#.
/// What's neat is that .NET makes the Binge class a 
/// full-fledged component that may be called from any other 
/// .NET component.
///
/// Paul M. Parks
/// http://www.parkscomputing.com/
/// February 8, 2002
/// 

using System;

namespace NinetyNineBottles
{
    /// <summary>
    /// References the method of output.
    /// </summary>
    public delegate void Writer(string format, params object[] arg);

    /// <summary>
    /// References the corrective action to take when we run out.
    /// </summary>
    public delegate int MakeRun();

    /// <summary>
    /// The act of consuming all those beverages.
    /// </summary>
    public class Binge
    {
        /// <summary>
        /// What we'll be drinking.
        /// </summary>
        private string beverage;

        /// <summary>
        /// The starting count.
        /// </summary>
        private int count = 0;

        /// <summary>
        /// The manner in which the lyrics are output.
        /// </summary>
        private Writer Sing;

        /// <summary>
        /// What to do when it's all gone.
        /// </summary>
        private MakeRun RiskDUI;

        public event MakeRun OutOfBottles;


        /// <summary>
        /// Initializes the binge.
        /// </summary>
        /// <param name="count">How many we're consuming.</param>
        /// <param name="disasterWaitingToHappen">
        /// Our instructions, should we succeed.
        /// </param>
        /// <param name="writer">How our drinking song will be heard.</param>
        /// <param name="beverage">What to drink during this binge.</param>
        public Binge(string beverage, int count, Writer writer)
        {
            this.beverage = beverage;
            this.count = count;
            this.Sing = writer;
        }

        /// <summary>
        /// Let's get started.
        /// </summary>
        public void Start()
        {
            while (count > 0)
            {
                Sing(
                    @"
{0} bottle{1} of {2} on the wall,
{0} bottle{1} of {2}.
Take one down, pass it around,", 
                    count, (count == 1) ? "" : "s", beverage);

                count--;

                if (count > 0)
                {
                    Sing("{0} bottle{1} of {2} on the wall.",
                        count, (count == 1) ? "" : "s", beverage);
                }
                else
                {
                    Sing("No more bottles of {0} on the wall.", beverage, null);
                }

            }

            Sing(
                @"
No more bottles of {0} on the wall,
No more bottles of {0}.", beverage, null);

            if (this.OutOfBottles != null)
            {
                count = this.OutOfBottles();
                Sing("{0} bottles of {1} on the wall.", count, beverage);
            }
            else
            {
                Sing("First we weep, then we sleep.");
                Sing("No more bottles of {0} on the wall.", beverage, null);
            }
        }
    }

    /// <summary>
    /// The song remains the same.
    /// </summary>
    class SingTheSong
    {
        /// <summary>
        /// Any other number would be strange.
        /// </summary>
        const int bottleCount = 99;

        /// <summary>
        /// The entry point. Sets the parameters of the Binge and starts it.
        /// </summary>
        /// <param name="args">unused</param>
        static void Main(string[] args)
        {
            Binge binge = 
               new Binge("beer", bottleCount, new Writer(Console.WriteLine));
            binge.OutOfBottles += new MakeRun(SevenEleven);
            binge.Start();
        }

        /// <summary>
        /// There's bound to be one nearby.
        /// </summary>
        /// <returns>Whatever would fit in the trunk.</returns>
        static int SevenEleven()
        {
            Console.WriteLine("Go to the store, get some more...");
            return bottleCount;
        }
    }
}