﻿using LiveSplit.Model;
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
                SplitKey = new KeyOrButton(Keys.NumPad1),
                ResetAndStartKey = null,
                ResetKey = new KeyOrButton(Keys.NumPad3),
                UndoKey = new KeyOrButton(Keys.NumPad8),
                SkipKey = new KeyOrButton(Keys.NumPad2),
                SwitchComparisonPrevious = new KeyOrButton(Keys.NumPad4),
                SwitchComparisonNext = new KeyOrButton(Keys.NumPad6),
                PauseKey = null,
                ToggleGlobalHotkeys = null,
                GlobalHotkeysEnabled = false,
                DeactivateHotkeysForOtherPrograms = false,
                WarnOnReset = true,
                DoubleTapPrevention = true,
                LastComparison = Run.PersonalBestComparisonName,
                LastTimingMethod = TimingMethod.RealTime,
                HotkeyDelay = 0f,
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
                    { NoneComparisonGenerator.ComparisonName, false }
                }
            };
        }
    }
}
