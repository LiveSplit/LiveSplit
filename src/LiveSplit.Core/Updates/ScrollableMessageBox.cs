// This code was written by Mike Gold  .  This code may be used only for educational purposes.
// Any distribution of this code may only be carried out with written consent from the author.
// Copyright ©  2005 by Microgold Software Inc. 

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace LiveSplit.Updates
{
    public partial class ScrollableMessageBox : Form
    {
        public ScrollableMessageBox()
        {
            InitializeComponent();
        }

        public Font MessageFont
        {
            set
            {
                txtMessage.Font = value;
            }

            get
            {
                return txtMessage.Font;
            }
        }

        public Color MessageForeColor
        {
            get
            {
                return txtMessage.ForeColor;
            }

            set
            {
                txtMessage.ForeColor = value;
            }
        }

        public Color MessageBackColor
        {
            get
            {
                return txtMessage.BackColor;
            }

            set
            {
                txtMessage.BackColor = value;
                BackColor = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="text">text inside the message box</param>
        /// <param name="caption">caption on the title bar</param>
        public DialogResult Show(string text, string caption)
        {
            // populate the text box with the message
            txtMessage.Text = text;
            // populate the caption with the passed in caption
            Text = caption;
            //Assume OK Button, by default
            ChooseButtons(MessageBoxButtons.OK);
            // the active control should be the OK button
            ActiveControl = Controls[Controls.Count - 1];
            return ShowDialog();
        }

        public DialogResult Show(string text, string caption, MessageBoxButtons buttonType)
        {
            txtMessage.Text = text;
            Text = caption;
            //Assume OK Button
            ChooseButtons(buttonType);
            ActiveControl = Controls[Controls.Count - 1];
            return ShowDialog();
        }

        public DialogResult ShowFromFile(string filename, string caption, MessageBoxButtons buttonType)
        {
            // read the file into the message box
            using (StreamReader sr = new StreamReader(filename))
            {
                txtMessage.Text = sr.ReadToEnd();
            }

            Text = caption;
            ChooseButtons(buttonType);
            ActiveControl = Controls[Controls.Count - 1];
            return ShowDialog();
        }

        void RemoveButtons()
        {
            List<Button> buttons = new List<Button>();
            foreach (Control c in Controls)
            {
                if (c is Button)
                {
                    buttons.Add(c as Button);
                }
            }

            foreach (Button b in buttons)
            {
                Controls.Remove(b);
            }
        }

        void ChooseButtons(MessageBoxButtons buttonType)
        {
            // remove existing buttons
            RemoveButtons();

            // decide which button set to add from buttonType, and add it
            switch (buttonType)
            {
                case MessageBoxButtons.OK:
                    AddOKButton();
                    break;
                case MessageBoxButtons.YesNo:
                    AddYesNoButtons();
                    break;
                case MessageBoxButtons.OKCancel:
                    AddOkCancelButtons();
                    break;
                default:
                    AddOKButton(); // default is an OK button
                    break;
            }
        }

        private void AddOKButton()
        {
            Button btnOK = new Button();
            btnOK.Text = "OK";
            Controls.Add(btnOK);
            btnOK.Location = new Point(Width / 2 - 35, txtMessage.Bottom + 5);
            btnOK.Size = new Size(70, 20);
            btnOK.DialogResult = DialogResult.OK;
            AcceptButton = btnOK;
        }

        private void AddYesNoButtons()
        {
            Button btnYes = new Button();
            btnYes.Text = "Yes";
            Controls.Add(btnYes);

            // calculate the location of the buttons so that they are centered
            // at the bottom
            btnYes.Location = new Point(Width / 2 - 80, txtMessage.Bottom + 5);
            btnYes.Size = new Size(75, 23);
            btnYes.DialogResult = DialogResult.Yes;
            AcceptButton = btnYes;

            Button btnNo = new Button();
            btnNo.Text = "No";
            Controls.Add(btnNo);
            btnNo.Location = new Point(Width / 2 + 5, txtMessage.Bottom + 5);
            btnNo.Size = new Size(75, 23);
            btnNo.DialogResult = DialogResult.No;
            CancelButton = btnNo;
        }

        private void AddOkCancelButtons()
        {
            Button btnOK = new Button();
            btnOK.Text = "OK";
            Controls.Add(btnOK);
            btnOK.Location = new Point(Width / 2 - 75, txtMessage.Bottom + 5);
            btnOK.Size = new Size(70, 20);
            btnOK.DialogResult = DialogResult.OK;
            AcceptButton = btnOK;

            Button btnCancel = new Button();
            btnCancel.Text = "Cancel";
            Controls.Add(btnCancel);
            btnCancel.Location = new Point(Width / 2 + 5, txtMessage.Bottom + 5);
            btnCancel.Size = new Size(70, 20);
            btnCancel.DialogResult = DialogResult.Cancel;
            CancelButton = btnCancel;
        }

        public void Show(string text)
        {
            txtMessage.Text = text;
            ChooseButtons(MessageBoxButtons.OK);
            ShowDialog();
        }

        private void ScrollableMessageBox_Resize(object sender, EventArgs e)
        {
            // txtMessage.SetBounds(0, 0, this.ClientRectangle.Width - 10, this.Height - 55);
        }

        private void ScrollableMessageBox_Load(object sender, EventArgs e)
        {
        }
    }
}