namespace TestApp
{
	partial class MainForm
	{
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Vom Windows Form-Designer generierter Code

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.colorPanel = new Fetze.WinFormsColor.ColorPanel();
			this.colorSliderHue = new Fetze.WinFormsColor.ColorSlider();
			this.colorSliderDisabled = new Fetze.WinFormsColor.ColorSlider();
			this.colorSliderSmall = new Fetze.WinFormsColor.ColorSlider();
			this.buttonPicker = new System.Windows.Forms.Button();
			this.buttonPickerWithAlpha = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// colorPanel
			// 
			this.colorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.colorPanel.BottomLeftColor = System.Drawing.SystemColors.ControlDarkDark;
			this.colorPanel.BottomRightColor = System.Drawing.SystemColors.Control;
			this.colorPanel.Location = new System.Drawing.Point(12, 12);
			this.colorPanel.Name = "colorPanel";
			this.colorPanel.Size = new System.Drawing.Size(181, 236);
			this.colorPanel.TabIndex = 0;
			this.colorPanel.TopLeftColor = System.Drawing.SystemColors.ActiveBorder;
			this.colorPanel.TopRightColor = System.Drawing.SystemColors.ActiveCaption;
			this.colorPanel.ValuePercentual = ((System.Drawing.PointF)(resources.GetObject("colorPanel.ValuePercentual")));
			// 
			// colorSliderHue
			// 
			this.colorSliderHue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.colorSliderHue.Location = new System.Drawing.Point(258, 12);
			this.colorSliderHue.Name = "colorSliderHue";
			this.colorSliderHue.Size = new System.Drawing.Size(31, 179);
			this.colorSliderHue.TabIndex = 1;
			// 
			// colorSliderDisabled
			// 
			this.colorSliderDisabled.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.colorSliderDisabled.Enabled = false;
			this.colorSliderDisabled.Location = new System.Drawing.Point(221, 12);
			this.colorSliderDisabled.Name = "colorSliderDisabled";
			this.colorSliderDisabled.Size = new System.Drawing.Size(31, 179);
			this.colorSliderDisabled.TabIndex = 2;
			// 
			// colorSliderSmall
			// 
			this.colorSliderSmall.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.colorSliderSmall.Location = new System.Drawing.Point(199, 12);
			this.colorSliderSmall.Maximum = System.Drawing.Color.Red;
			this.colorSliderSmall.Minimum = System.Drawing.Color.Black;
			this.colorSliderSmall.Name = "colorSliderSmall";
			this.colorSliderSmall.PickerSize = 3;
			this.colorSliderSmall.ShowInnerPicker = false;
			this.colorSliderSmall.Size = new System.Drawing.Size(16, 179);
			this.colorSliderSmall.TabIndex = 3;
			// 
			// buttonPicker
			// 
			this.buttonPicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonPicker.Location = new System.Drawing.Point(199, 225);
			this.buttonPicker.Name = "buttonPicker";
			this.buttonPicker.Size = new System.Drawing.Size(90, 23);
			this.buttonPicker.TabIndex = 4;
			this.buttonPicker.Text = "ColorPicker";
			this.buttonPicker.UseVisualStyleBackColor = true;
			this.buttonPicker.Click += new System.EventHandler(this.buttonPicker_Click);
			// 
			// buttonPickerWithAlpha
			// 
			this.buttonPickerWithAlpha.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonPickerWithAlpha.Location = new System.Drawing.Point(199, 197);
			this.buttonPickerWithAlpha.Name = "buttonPickerWithAlpha";
			this.buttonPickerWithAlpha.Size = new System.Drawing.Size(90, 23);
			this.buttonPickerWithAlpha.TabIndex = 5;
			this.buttonPickerWithAlpha.Text = "ColorPicker +A";
			this.buttonPickerWithAlpha.UseVisualStyleBackColor = true;
			this.buttonPickerWithAlpha.Click += new System.EventHandler(this.buttonPickerWithAlpha_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(301, 260);
			this.Controls.Add(this.buttonPickerWithAlpha);
			this.Controls.Add(this.buttonPicker);
			this.Controls.Add(this.colorSliderSmall);
			this.Controls.Add(this.colorSliderDisabled);
			this.Controls.Add(this.colorSliderHue);
			this.Controls.Add(this.colorPanel);
			this.Name = "MainForm";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}

		#endregion

		private Fetze.WinFormsColor.ColorPanel colorPanel;
		private Fetze.WinFormsColor.ColorSlider colorSliderHue;
		private Fetze.WinFormsColor.ColorSlider colorSliderDisabled;
		private Fetze.WinFormsColor.ColorSlider colorSliderSmall;
		private System.Windows.Forms.Button buttonPicker;
		private System.Windows.Forms.Button buttonPickerWithAlpha;
	}
}

