**XSplit.Wpf**

These are several C# helper classes to aide in creating XSplit broadcaster plugins using WPF/XAML. TimedBroadcasterPlugin implements BroadcasterPlugin as a simple method for sending a Visual object to XSplit at a specific frame-rate.

The following code shows an example of how to use the classes in the .xaml.cs of an object to initialize and send updates to XSplit.

```C#
TimedBroadcasterPlugin plugin;

/// <summary> Initializes a new instance of the <see cref="ScoreboardControlPanelView"/> class. </summary>
public ScoreboardControlPanelView()
{
	this.InitializeComponent();

	const int OutputWidth = 1000;
	const int OutputHeight = 200;
	
	// Outputs a 1000x200 image every 50ms (20 FPS)
	plugin = TimedBroadcasterPlugin.CreateInstance(
		"YOUR-UNIQUE-ID", this.scoreboardView, OutputWidth, OutputHeight, 50);
		
	if (this.plugin != null)
	{
		// The correct version of XSplit was installed, so we can start our output.
		this.plugin.StartTimer();
	}
}
```
