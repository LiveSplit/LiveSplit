using LiveSplit.Model.Comparisons;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.Model
{
    [Serializable]
    public class Segment : ISegment
    {
        public Image Icon { get; set; }
        public String Name { get; set; }
        public Time PersonalBestSplitTime
        {
            get { return Comparisons[Run.PersonalBestComparisonName]; }
            set { Comparisons[Run.PersonalBestComparisonName] = value; }
        }
        public IComparisons Comparisons { get; set; }
        public Time BestSegmentTime { get; set; }
        public Time SplitTime { get; set; }
        public IList<IIndexedTime> SegmentHistory { get; set;}
        
        public Segment(
            String name, Time pbSplitTime = default(Time), 
            Time bestSegmentTime = default(Time), Image icon = null,
            Time splitTime = default(Time))
        {
            Comparisons = new CompositeComparisons();
            Name = name;
            PersonalBestSplitTime = pbSplitTime;
            BestSegmentTime = bestSegmentTime;
            SplitTime = splitTime;
            Icon = icon;
            SegmentHistory = new List<IIndexedTime>();
        }

        public object Clone()
        {
            var newSegmentHistory = new List<IIndexedTime>();
            foreach (var element in this.SegmentHistory)
                newSegmentHistory.Add(new IndexedTime(element.Time, element.Index));

            return new Segment(Name)
            {
                BestSegmentTime = this.BestSegmentTime,
                SplitTime = this.SplitTime,
                Icon = this.Icon, //TODO: Should be a clone //(this.Icon != null) ? this.Icon.Clone() as Image : null,
                SegmentHistory = newSegmentHistory,
                Comparisons = (IComparisons)this.Comparisons.Clone()
            };
        }
    }
}
