using LiveSplit.Model;
using LiveSplit.Model.Input;
using LiveSplit.UI;
using LiveSplit.Web.SRL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveSplit.Options
{
    public interface ISettings : ICloneable
    {
        KeyOrButton SplitKey { get; set; }
        KeyOrButton ResetKey { get; set; }
        KeyOrButton SkipKey { get; set; }
        KeyOrButton UndoKey { get; set; }
        KeyOrButton PauseKey { get; set; }
        KeyOrButton ToggleGlobalHotkeys { get; set; }
        KeyOrButton ScrollUp { get; set; }
        KeyOrButton ScrollDown { get; set; }
        KeyOrButton SwitchComparisonPrevious { get; set; }
        KeyOrButton SwitchComparisonNext { get; set; }
        IList<String> RecentSplits { get; set; }
        IList<String> RecentLayouts { get; set; }
        String LastComparison { get; set; }
        TimingMethod LastTimingMethod { get; set; }
        float HotkeyDelay { get; set; }
        bool GlobalHotkeysEnabled { get; set; }
        bool DeactivateHotkeysForOtherPrograms { get; set; }
        bool WarnOnReset { get; set; }
        bool DoubleTapPrevention { get; set; }
        bool SimpleSumOfBest { get; set; }
        IRaceViewer RaceViewer { get; set; }
        IList<String> ActiveAutoSplitters { get; set; }

        bool AgreedToSRLRules { get; set; }

        void AddToRecentSplits(string path);
        void AddToRecentLayouts(string path);
        void RegisterHotkeys(CompositeHook hook);
        void UnregisterAllHotkeys(CompositeHook hook);
    }
}
