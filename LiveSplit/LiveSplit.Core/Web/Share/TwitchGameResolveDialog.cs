using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveSplit.Web.Share
{
    public partial class TwitchGameResolveDialog : Form
    {
        public String GameName { get; protected set; }

        public TwitchGameResolveDialog(String oldGame)
        {
            InitializeComponent();
            cbxGames.Text = oldGame;
            cbxGames.AutoCompleteMode = AutoCompleteMode.Suggest;
            cbxGames.AutoCompleteSource = AutoCompleteSource.ListItems;
            SystemSounds.Hand.Play();
            Search();
        }

        private void Search()
        {
            try
            {
                var suggestedGames = Twitch.Instance.FindGame(cbxGames.Text);
                cbxGames.Items.Clear();
                cbxGames.Items.AddRange(suggestedGames.ToArray());
            }
            catch { }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            GameName = cbxGames.Text;
        }

        private void cbxGames_TextUpdate(object sender, EventArgs e)
        {
        }

        private void btnNoGame_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            GameName = null;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }
    }
}
