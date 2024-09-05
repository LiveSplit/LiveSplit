using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace UpdateManager;

public partial class UpdateForm : Form
{
    private IEnumerable<IUpdateable> Updateables { get; set; }
    private string OtherProcess { get; set; }

    public UpdateForm(IEnumerable<IUpdateable> updateables, string otherProcess = null)
    {
        InitializeComponent();
        foreach (IUpdateable updateable in updateables)
        {
            Updater.GetUpdater(updateable).UpdatePercentageRefreshed += new UpdatePercentageRefreshedEventHandler(updater_UpdatePercentageRefreshed);
        }

        Updateables = updateables;
        OtherProcess = otherProcess;
    }

    private void updater_UpdatePercentageRefreshed(object sender, UpdatePercentageRefreshedEventArgs e)
    {
        Action a = () => prgUpdate.Value = (int)(100 * e.Percentage);

        if (prgUpdate.InvokeRequired)
        {
            prgUpdate.Invoke(a);
        }
        else
        {
            a();
        }
    }

    private void UpdateForm_Load(object sender, EventArgs e)
    {
        void a()
        {
            try
            {
                Updater.UpdateAllInternally(Updateables);

                if (OtherProcess != null)
                {
                    Process.Start(OtherProcess);
                }

                Invoke(new Action(Close));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        new Thread(new ThreadStart((Action)a)).Start();
    }
}
