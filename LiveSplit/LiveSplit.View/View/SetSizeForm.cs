using System;
using System.Windows.Forms;

namespace LiveSplit.View
{
    public partial class SetSizeForm : Form
    {
        public Form TimerForm { get; set; }

        public int FormWidth
        {
            get { return TimerForm.Width; }
            set
            {
                TimerForm.Width = value;
                WidthChanged();
            }
        }
        public int FormHeight
        {
            get { return TimerForm.Height; }
            set
            {
                TimerForm.Height = value;
                HeightChanged();
            }
        }

        protected int OldWidth { get; set; }
        protected int OldHeight { get; set; }

        public bool KeepAspectRatio { get; set; }

        public SetSizeForm(Form form)
        {
            InitializeComponent();

            TimerForm = form;
            KeepAspectRatio = false;
            OldHeight = FormHeight;
            OldWidth = FormWidth;

            nmWidth.DataBindings.Add("Value", this, "FormWidth", false, DataSourceUpdateMode.OnPropertyChanged);
            nmHeight.DataBindings.Add("Value", this, "FormHeight", false, DataSourceUpdateMode.OnPropertyChanged);
            chkKeepAspectRatio.DataBindings.Add("Checked", this, "KeepAspectRatio", false, DataSourceUpdateMode.OnPropertyChanged);

        }

        protected void HeightChanged()
        {
            if (KeepAspectRatio && OldHeight != FormHeight)
            {
                var newValue = (int)(FormWidth * (float)FormHeight / OldHeight + 0.5f);
                OldHeight = FormHeight;
                OldWidth = newValue;
                FormWidth = newValue;
            }
            else
                OldHeight = FormHeight;
        }

        protected void WidthChanged()
        {
            if (KeepAspectRatio && OldWidth != FormWidth)
            {
                var newValue = (int)(FormHeight * (float)FormWidth / OldWidth + 0.5f);
                OldWidth = FormWidth;
                OldHeight = newValue;
                FormHeight = newValue;
            }
            else
                OldWidth = FormWidth;
        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
