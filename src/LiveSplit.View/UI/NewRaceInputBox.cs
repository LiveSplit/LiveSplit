using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.Utils;
using LiveSplit.Web.SRL;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveSplit.UI
{
    public class NewRaceInputBox : Form
    {
        public Label label { get; set; }
        public Label label2 { get; set; }
        public Label labelNote { get; set; }
        public ComboBox cbxGameName { get; set; }
        public ComboBox cbxRunCategory { get; set; }
        public Button buttonOk { get; set; }
        public Button buttonCancel { get; set; }
        public IRun Run { get; set; }

        public NewRaceInputBox()
        {
            label = new Label();
            label2 = new Label();
            labelNote = new Label();
            cbxGameName = new ComboBox();
            cbxRunCategory = new ComboBox();
            buttonOk = new Button();
            buttonCancel = new Button();

            Task.Factory.StartNew(() =>
            {
                try
                {
                    var gameNames = SpeedRunsLiveAPI.Instance.GetGameNames().ToArray();
                    this.InvokeIfRequired(() =>
                    {
                        try
                        {
                            cbxGameName.Items.AddRange(gameNames);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            });

            cbxGameName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cbxGameName.TextChanged += cbxGameName_TextChanged;

            cbxRunCategory.AutoCompleteSource = AutoCompleteSource.ListItems;
            cbxRunCategory.Items.AddRange(new [] { "Any%", "Low%", "100%" });
            cbxRunCategory.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

            Text = "New Race";
            label.Text = "Game:";
            label2.Text = "Category:";
            labelNote.Text = "Creating a race without any opponents is against the rules.";

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            label2.SetBounds(9, 66, 372, 13);
            cbxGameName.SetBounds(12, 36, 372, 20);
            cbxRunCategory.SetBounds(12, 82, 372, 20);
            labelNote.SetBounds(9, 118, 372, 13);
            buttonOk.SetBounds(228, 145, 75, 23);
            buttonCancel.SetBounds(309, 145, 75, 23);

            label.AutoSize = true;
            label2.AutoSize = true;
            labelNote.AutoSize = true;
            cbxGameName.Anchor = cbxGameName.Anchor | AnchorStyles.Right;
            cbxRunCategory.Anchor = cbxGameName.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            ClientSize = new Size(396, 180);
            Controls.AddRange(new Control[] { label, label2, labelNote, cbxGameName, cbxRunCategory, buttonOk, buttonCancel });
            ClientSize = new Size(Math.Max(300, label.Right + 10), ClientSize.Height);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;
            MinimizeBox = false;
            MaximizeBox = false;
            AcceptButton = buttonOk;
            CancelButton = buttonCancel;

            FormClosing += NewRaceInputBox_FormClosing;

            cbxGameName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

            cbxRunCategory.AutoCompleteSource = AutoCompleteSource.ListItems;
            cbxRunCategory.Items.AddRange(new string[] { "Any%", "Low%", "100%" });
            cbxRunCategory.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

            RefreshCategoryAutoCompleteList("");
        }

        void NewRaceInputBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                var gameID = SpeedRunsLiveAPI.Instance.GetGameIDFromName(cbxGameName.Text);
                if (string.IsNullOrEmpty(gameID))
                {
                    var result = MessageBox.Show(this, "The game you entered could not be found in the SpeedRunsLive Game List. Are you sure you would like to start a race with a New Game?", "Game Not Found", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.No)
                        e.Cancel = true;
                }
            }
        }
        void cbxGameName_TextChanged(object sender, EventArgs e)
        {
            RefreshCategoryAutoCompleteList(((ComboBox)sender).Text);
        }

        void RefreshCategoryAutoCompleteList(string gameName)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    string[] categoryNames;
                    try
                    {
                        categoryNames = SpeedRunsLiveAPI.Instance.GetCategories(SpeedRunsLiveAPI.Instance.GetGameIDFromName(gameName)).ToArray();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);

                        categoryNames = new [] { "Any%", "Low%", "100%" };
                    }

                    this.InvokeIfRequired(() =>
                    {
                        try
                        {
                            cbxRunCategory.Items.Clear();
                            cbxRunCategory.Items.AddRange(categoryNames);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            });
        }

        public DialogResult Show(ref string game, ref string category)
        {
            var gameNames = SpeedRunsLiveAPI.Instance.GetGameNames();
            cbxGameName.Text = gameNames.FindMostSimilarValueTo(game);
            cbxRunCategory.Text = category;
            var dialogResult = ShowDialog();
            game = cbxGameName.Text;
            category = cbxRunCategory.Text;
            return dialogResult;
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // NewRaceInputBox
            // 
            ClientSize = new Size(284, 317);
            Name = "NewRaceInputBox";
            ResumeLayout(false);
        }
    }
}
