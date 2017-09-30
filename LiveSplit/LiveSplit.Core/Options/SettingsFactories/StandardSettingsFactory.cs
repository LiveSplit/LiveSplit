using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.Model.Input;
using LiveSplit.Web.SRL.RaceViewers;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LiveSplit.Options.SettingsFactories
{
    public class StandardSettingsFactory : ISettingsFactory
    {
        public ISettings Create()
        {
            return new Settings()
            {
                HotkeyProfiles = new Dictionary<string, HotkeyProfile>()
                {
                    {HotkeyProfile.DefaultHotkeyProfileName, new HotkeyProfile()
                        {
                            SplitKey = new KeyOrButton(Keys.NumPad1),
                            ResetKey = new KeyOrButton(Keys.NumPad3),
                            UndoKey = new KeyOrButton(Keys.NumPad8),
                            SkipKey = new KeyOrButton(Keys.NumPad2),
                            SwitchComparisonPrevious = new KeyOrButton(Keys.NumPad4),
                            SwitchComparisonNext = new KeyOrButton(Keys.NumPad6),
                            PauseKey = null,
                            ToggleGlobalHotkeys = null,
                            GlobalHotkeysEnabled = false,
                            DeactivateHotkeysForOtherPrograms = false,
                            DoubleTapPrevention = true,
                            HotkeyDelay = 0f
                        }
                    }
                },
                WarnOnReset = true,
                LastComparison = Run.PersonalBestComparisonName,
                RaceViewer = new SRLRaceViewer(),
                AgreedToSRLRules = false,
                SimpleSumOfBest = false,
                ComparisonGeneratorStates = new Dictionary<string, bool>()
                {
                    { BestSegmentsComparisonGenerator.ComparisonName, true },
                    { BestSplitTimesComparisonGenerator.ComparisonName, false },
                    { AverageSegmentsComparisonGenerator.ComparisonName, true },
                    { WorstSegmentsComparisonGenerator.ComparisonName, false},
                    { PercentileComparisonGenerator.ComparisonName, false },
                    { LatestRunComparisonGenerator.ComparisonName, false },
                    { NoneComparisonGenerator.ComparisonName, false }
                }
            };
        }
    }
}
