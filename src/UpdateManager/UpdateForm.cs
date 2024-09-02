using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace UpdateManager;

public partial class UpdateForm : Form
{
    IEnumerable<IUpdateable> Updateables { get; set; }
    string OtherProcess { get; set; }

    public UpdateForm(IEnumerable<IUpdateable> updateables, string otherProcess = null)
    {
        InitializeComponent();
        foreach (IUpdateable updateable in updateables)
            Updater.GetUpdater(updateable).UpdatePercentageRefreshed += new UpdatePercentageRefreshedEventHandler(updater_UpdatePercentageRefreshed);
        Updateables = updateables;
        OtherProcess = otherProcess;
    }

    void updater_UpdatePercentageRefreshed(object sender, UpdatePercentageRefreshedEventArgs e)
    {
        Action a = () => prgUpdate.Value = (int)(100 * e.Percentage);

        if (prgUpdate.InvokeRequired)
            prgUpdate.Invoke(a);
        else
            a();
    }

    private void UpdateForm_Load(object sender, EventArgs e)
    {
        Action a = () =>
            {
                try
                {
                    Updater.UpdateAllInternally(Updateables);

                    if (OtherProcess != null)
                        Process.Start(OtherProcess);
                    Invoke(new Action(() => Close()));
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
            };
        new Thread(new ThreadStart(a)).Start();
    }
}
