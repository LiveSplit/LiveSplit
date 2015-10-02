using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LiveSplit.Model;
using LiveSplit.Web.Share;

namespace LiveSplit.View
{
    public partial class SpeedrunComSubmitDialog : Form
    {
        private RunMetadata metadata;
        private bool hasPersonalBestDateTime;
        private DateTimePicker datePicker;
        private TextBox txtWithoutLoads;
        private TextBox txtGameTime;

        public SpeedrunComSubmitDialog(RunMetadata metadata)
        {
            this.metadata = metadata;

            InitializeComponent();

            hasPersonalBestDateTime = SpeedrunCom.FindPersonalBestAttemptDate(metadata.LiveSplitRun).HasValue;

            var row = 2;

            if (!hasPersonalBestDateTime)
            {
                var dateLabel = new Label();
                dateLabel.Text = "Date:";
                tableLayoutPanel.Controls.Add(dateLabel, 0, row);
                dateLabel.Anchor = AnchorStyles.Left;
                dateLabel.AutoSize = true;

                datePicker = new DateTimePicker();
                datePicker.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                datePicker.TabIndex = row;
                tableLayoutPanel.Controls.Add(datePicker, 1, row);
                tableLayoutPanel.SetColumnSpan(datePicker, 2);

                MinimumSize = new Size(MinimumSize.Width, MinimumSize.Height + datePicker.Height);
                Size = new Size(Size.Width, Size.Height + datePicker.Height);

                row++;
            }

            var runTime = metadata.LiveSplitRun.Last().PersonalBestSplitTime;

            var timingMethods = metadata.Game.Ruleset.TimingMethods;
            var usesGameTime = timingMethods.Contains(SpeedrunComSharp.TimingMethod.GameTime);
            var usesWithoutLoads = timingMethods.Contains(SpeedrunComSharp.TimingMethod.RealTimeWithoutLoads);
            var usesBoth = usesGameTime && usesWithoutLoads;
            if (!runTime.GameTime.HasValue || usesBoth)
            {
                if (usesWithoutLoads)
                {
                    var label = new Label();
                    label.Text = "Without Loads:";
                    tableLayoutPanel.Controls.Add(label, 0, row);
                    label.Anchor = AnchorStyles.Left;
                    label.AutoSize = true;

                    txtWithoutLoads = new TextBox();
                    txtWithoutLoads.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                    txtWithoutLoads.TabIndex = row;
                    tableLayoutPanel.Controls.Add(txtWithoutLoads, 1, row);
                    tableLayoutPanel.SetColumnSpan(txtWithoutLoads, 2);

                    MinimumSize = new Size(MinimumSize.Width, MinimumSize.Height + txtWithoutLoads.Height);
                    Size = new Size(Size.Width, Size.Height + txtWithoutLoads.Height);

                    row++;
                }
                
                if (usesGameTime)
                {
                    var label = new Label();
                    label.Text = "Game Time:";
                    tableLayoutPanel.Controls.Add(label, 0, row);
                    label.Anchor = AnchorStyles.Left;
                    label.AutoSize = true;

                    txtGameTime = new TextBox();
                    txtGameTime.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                    txtGameTime.TabIndex = row;
                    tableLayoutPanel.Controls.Add(txtGameTime, 1, row);
                    tableLayoutPanel.SetColumnSpan(txtGameTime, 2);

                    MinimumSize = new Size(MinimumSize.Width, MinimumSize.Height + txtGameTime.Height);
                    Size = new Size(Size.Width, Size.Height + txtGameTime.Height);

                    row++;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            Uri videoUri = null;

            if (!string.IsNullOrEmpty(txtVideo.Text))
            {
                var videoText = txtVideo.Text;
                if (!videoText.StartsWith("http"))
                    videoText = "http://" + videoText;
                if (Uri.IsWellFormedUriString(videoText, UriKind.Absolute))
                {
                    videoUri = new Uri(videoText);
                }
                else
                {
                    MessageBox.Show(this, "You didn't provide a valid Video URL.", "Submitting Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            var comment = txtComment.Text;

            DateTime? date = null;

            if (!hasPersonalBestDateTime)
            {
                date = new DateTime(datePicker.Value.Year, datePicker.Value.Month, datePicker.Value.Day, 0, 0, 0, DateTimeKind.Utc);
            }

            if (txtGameTime != null)
            {
                try
                {
                    var gameTime = TimeSpanParser.ParseNullable(txtGameTime.Text);
                    patchGameTime(gameTime);
                }
                catch
                {
                    MessageBox.Show(this, "You didn't enter a valid Game Time.", "Submitting Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            TimeSpan? withoutLoads = null;

            if (txtWithoutLoads != null)
            {
                try
                {
                    withoutLoads = TimeSpanParser.ParseNullable(txtWithoutLoads.Text);
                    patchGameTime(withoutLoads);
                }
                catch
                {
                    MessageBox.Show(this, "You didn't enter a valid Real Time without Loads.", "Submitting Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            string reason;
            var submitted = SpeedrunCom.SubmitRun(metadata.LiveSplitRun, out reason, 
                comment: comment, videoUri: videoUri, date: date, withoutLoads: withoutLoads);

            if (submitted)
            {
                Process.Start(metadata.Run.WebLink.AbsoluteUri);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show(this, reason, "Submitting Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void patchGameTime(TimeSpan? gameTime)
        {
            if (!gameTime.HasValue)
                return;

            var run = metadata.LiveSplitRun;
            var lastSplit = run.Last();
            var runTime = lastSplit.PersonalBestSplitTime;

            if (runTime.GameTime.HasValue)
                return;

            var attempt = run.AttemptHistory.FirstOrDefault(x =>
                x.Time.GameTime == runTime.GameTime
                && x.Time.RealTime == runTime.RealTime);

            runTime.GameTime = gameTime;

            if (attempt.Time.RealTime.HasValue)
            {
                run.AttemptHistory.Remove(attempt);

                attempt.Time = runTime;

                run.AttemptHistory = run.AttemptHistory.Concat(new[] { attempt }).OrderBy(x => x.Index).ToList();
            }

            lastSplit.PersonalBestSplitTime = runTime;
        }
    }
}
