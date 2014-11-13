using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XSplit.Wpf;

namespace LiveSplit.XSplitPlugin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TimedBroadcasterPlugin plugin;

        public MainWindow()
        {
            InitializeComponent();

            const int OutputWidth = 1000;
            const int OutputHeight = 200;

            // Outputs a 1000x200 image every 50ms (20 FPS)
            plugin = TimedBroadcasterPlugin.CreateInstance(
                "livesplit", new irrelevant(), OutputWidth, OutputHeight, 50);

            if (this.plugin != null)
            {
                // The correct version of XSplit was installed, so we can start our output.
                this.plugin.StartTimer();
            }
        }
    }
}
