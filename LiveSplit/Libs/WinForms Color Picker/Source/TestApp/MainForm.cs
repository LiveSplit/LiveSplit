using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Fetze.WinFormsColor;

namespace TestApp
{
	public partial class MainForm : Form
	{
		private ColorPickerDialog	picker			= new ColorPickerDialog();
		private	ColorPickerDialog	pickerWithAlpha	= new ColorPickerDialog();

		public MainForm()
		{
			this.InitializeComponent();
			this.picker.AlphaEnabled = false;
		}

		private void buttonPicker_Click(object sender, EventArgs e)
		{
			this.picker.ShowDialog(this);
		}
		private void buttonPickerWithAlpha_Click(object sender, EventArgs e)
		{
			this.pickerWithAlpha.ShowDialog(this);
		}
	}
}
