using LiveSplit.Model;
using LiveSplit.Model.Input;
using LiveSplit.Web.SRL.RaceViewers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                SimpleSumOfBest = false
            };
        }
    }
}
