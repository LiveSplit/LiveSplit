using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using LiveSplit.Model;
using LiveSplit.Web.Share;

namespace LiveSplit.View;

public partial class SpeedrunComSubmitDialog : Form
{
    private readonly RunMetadata metadata;
    private readonly bool hasPersonalBestDateTime;
    private readonly DateTimePicker datePicker;
    private readonly TextBox txtWithoutLoads;
    private readonly TextBox txtGameTime;

    public SpeedrunComSubmitDialog(RunMetadata metadata)
    {
        this.metadata = metadata;

        InitializeComponent();

        hasPersonalBestDateTime = SpeedrunCom.FindPersonalBestAttemptDate(metadata.LiveSplitRun).HasValue;

        int row = 2;

        if (!hasPersonalBestDateTime)
        {
            var dateLabel = new Label
            {
                Text = "Date:"
            };
            tableLayoutPanel.Controls.Add(dateLabel, 0, row);
            dateLabel.Anchor = AnchorStyles.Left;
            dateLabel.AutoSize = true;

            datePicker = new DateTimePicker
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                TabIndex = row
            };
            tableLayoutPanel.Controls.Add(datePicker, 1, row);
            tableLayoutPanel.SetColumnSpan(datePicker, 2);

            MinimumSize = new Size(MinimumSize.Width, MinimumSize.Height + datePicker.Height);
            Size = new Size(Size.Width, Size.Height + datePicker.Height);

            row++;
        }

        Time runTime = metadata.LiveSplitRun.Last().PersonalBestSplitTime;

        System.Collections.ObjectModel.ReadOnlyCollection<SpeedrunComSharp.TimingMethod> timingMethods = metadata.Game.Ruleset.TimingMethods;
        bool usesGameTime = timingMethods.Contains(SpeedrunComSharp.TimingMethod.GameTime);
        bool usesWithoutLoads = timingMethods.Contains(SpeedrunComSharp.TimingMethod.RealTimeWithoutLoads);
        bool usesBoth = usesGameTime && usesWithoutLoads;
        if (!runTime.GameTime.HasValue || usesBoth)
        {
            if (usesWithoutLoads)
            {
                var label = new Label
                {
                    Text = "Without Loads:"
                };
                tableLayoutPanel.Controls.Add(label, 0, row);
                label.Anchor = AnchorStyles.Left;
                label.AutoSize = true;

                txtWithoutLoads = new TextBox
                {
                    Anchor = AnchorStyles.Left | AnchorStyles.Right,
                    TabIndex = row
                };
                tableLayoutPanel.Controls.Add(txtWithoutLoads, 1, row);
                tableLayoutPanel.SetColumnSpan(txtWithoutLoads, 2);

                MinimumSize = new Size(MinimumSize.Width, MinimumSize.Height + txtWithoutLoads.Height);
                Size = new Size(Size.Width, Size.Height + txtWithoutLoads.Height);

                row++;
            }

            if (usesGameTime)
            {
                var label = new Label
                {
                    Text = "Game Time:"
                };
                tableLayoutPanel.Controls.Add(label, 0, row);
                label.Anchor = AnchorStyles.Left;
                label.AutoSize = true;

                txtGameTime = new TextBox
                {
                    Anchor = AnchorStyles.Left | AnchorStyles.Right,
                    TabIndex = row
                };
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
            string videoText = txtVideo.Text;
            if (!videoText.StartsWith("http"))
            {
                videoText = "http://" + videoText;
            }

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

        string comment = txtComment.Text;

        DateTime? date = null;

        if (!hasPersonalBestDateTime)
        {
            date = new DateTime(datePicker.Value.Year, datePicker.Value.Month, datePicker.Value.Day, 0, 0, 0, DateTimeKind.Utc);
        }

        if (txtGameTime != null)
        {
            try
            {
                TimeSpan? gameTime = TimeSpanParser.ParseNullable(txtGameTime.Text);
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

        bool submitted = SpeedrunCom.SubmitRun(metadata.LiveSplitRun, out string reason,
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
        {
            return;
        }

        IRun run = metadata.LiveSplitRun;
        ISegment lastSplit = run.Last();
        Time runTime = lastSplit.PersonalBestSplitTime;

        if (runTime.GameTime.HasValue)
        {
            return;
        }

        Attempt attempt = run.AttemptHistory.FirstOrDefault(x =>
            x.Time.GameTime == runTime.GameTime
            && x.Time.RealTime == runTime.RealTime);

        runTime.GameTime = gameTime;

        if (attempt.Time.RealTime.HasValue)
        {
            run.AttemptHistory.Remove(attempt);

            attempt.Time = runTime;

            run.AttemptHistory = [.. run.AttemptHistory.Concat(new[] { attempt }).OrderBy(x => x.Index)];
        }

        lastSplit.PersonalBestSplitTime = runTime;
    }
}
