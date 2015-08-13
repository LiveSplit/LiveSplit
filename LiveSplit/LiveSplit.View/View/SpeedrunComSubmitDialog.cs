using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
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

        public SpeedrunComSubmitDialog(RunMetadata metadata)
        {
            this.metadata = metadata;

            InitializeComponent();

            hasPersonalBestDateTime = SpeedrunCom.FindPersonalBestAttemptDate(metadata.LiveSplitRun).HasValue;

            if (!hasPersonalBestDateTime)
            {
                var dateLabel = new Label();
                dateLabel.Text = "Date:";
                tableLayoutPanel.Controls.Add(dateLabel, 0, 2);
                dateLabel.Anchor = AnchorStyles.Left;
                dateLabel.AutoSize = true;

                datePicker = new DateTimePicker();
                datePicker.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                tableLayoutPanel.Controls.Add(datePicker, 1, 2);
                tableLayoutPanel.SetColumnSpan(datePicker, 2);
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
                if (Uri.IsWellFormedUriString(txtVideo.Text, UriKind.Absolute))
                {
                    videoUri = new Uri(txtVideo.Text);
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

            string reason;
            var submitted = SpeedrunCom.SubmitRun(metadata.LiveSplitRun, out reason, 
                comment: comment, videoUri: videoUri, date: date);

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
    }
}
