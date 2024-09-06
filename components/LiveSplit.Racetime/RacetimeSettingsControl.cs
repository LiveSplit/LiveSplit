using System;
using System.Windows.Forms;

namespace LiveSplit.Racetime;

public partial class RacetimeSettingsControl : UserControl
{
    private RacetimeSettings settings;

    public RacetimeSettings Settings
    {
        get => settings;
        set
        {
            settings = value;
            RacetimeSettingsControl_VisibleChanged(this, null);
        }
    }

    public RacetimeSettingsControl()
    {
        InitializeComponent();
    }

    private void RacetimeSettingsControl_VisibleChanged(object sender, EventArgs e)
    {
    }
}
