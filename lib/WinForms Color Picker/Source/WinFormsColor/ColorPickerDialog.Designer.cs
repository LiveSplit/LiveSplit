namespace Fetze.WinFormsColor
{
	partial class ColorPickerDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorPickerDialog));
            this.radioHue = new System.Windows.Forms.RadioButton();
            this.radioSaturation = new System.Windows.Forms.RadioButton();
            this.radioValue = new System.Windows.Forms.RadioButton();
            this.radioRed = new System.Windows.Forms.RadioButton();
            this.radioBlue = new System.Windows.Forms.RadioButton();
            this.radioGreen = new System.Windows.Forms.RadioButton();
            this.numHue = new System.Windows.Forms.NumericUpDown();
            this.numSaturation = new System.Windows.Forms.NumericUpDown();
            this.numValue = new System.Windows.Forms.NumericUpDown();
            this.numRed = new System.Windows.Forms.NumericUpDown();
            this.numGreen = new System.Windows.Forms.NumericUpDown();
            this.numBlue = new System.Windows.Forms.NumericUpDown();
            this.textBoxHex = new System.Windows.Forms.TextBox();
            this.labelHex = new System.Windows.Forms.Label();
            this.labelOld = new System.Windows.Forms.Label();
            this.labelNew = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.numAlpha = new System.Windows.Forms.NumericUpDown();
            this.labelAlpha = new System.Windows.Forms.Label();
            this.labelHueUnit = new System.Windows.Forms.Label();
            this.labelSaturationUnit = new System.Windows.Forms.Label();
            this.labelValueUnit = new System.Windows.Forms.Label();
            this.btnPickColor = new System.Windows.Forms.Button();
            this.alphaSlider = new Fetze.WinFormsColor.ColorSlider();
            this.colorShowBox = new Fetze.WinFormsColor.ColorShowBox();
            this.colorSlider = new Fetze.WinFormsColor.ColorSlider();
            this.colorPanel = new Fetze.WinFormsColor.ColorPanel();
            ((System.ComponentModel.ISupportInitialize)(this.numHue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSaturation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAlpha)).BeginInit();
            this.SuspendLayout();
            // 
            // radioHue
            // 
            resources.ApplyResources(this.radioHue, "radioHue");
            this.radioHue.Checked = true;
            this.radioHue.Name = "radioHue";
            this.radioHue.TabStop = true;
            this.radioHue.UseVisualStyleBackColor = true;
            this.radioHue.CheckedChanged += new System.EventHandler(this.radioHue_CheckedChanged);
            // 
            // radioSaturation
            // 
            resources.ApplyResources(this.radioSaturation, "radioSaturation");
            this.radioSaturation.Name = "radioSaturation";
            this.radioSaturation.UseVisualStyleBackColor = true;
            this.radioSaturation.CheckedChanged += new System.EventHandler(this.radioSaturation_CheckedChanged);
            // 
            // radioValue
            // 
            resources.ApplyResources(this.radioValue, "radioValue");
            this.radioValue.Name = "radioValue";
            this.radioValue.UseVisualStyleBackColor = true;
            this.radioValue.CheckedChanged += new System.EventHandler(this.radioValue_CheckedChanged);
            // 
            // radioRed
            // 
            resources.ApplyResources(this.radioRed, "radioRed");
            this.radioRed.Name = "radioRed";
            this.radioRed.UseVisualStyleBackColor = true;
            this.radioRed.CheckedChanged += new System.EventHandler(this.radioRed_CheckedChanged);
            // 
            // radioBlue
            // 
            resources.ApplyResources(this.radioBlue, "radioBlue");
            this.radioBlue.Name = "radioBlue";
            this.radioBlue.UseVisualStyleBackColor = true;
            this.radioBlue.CheckedChanged += new System.EventHandler(this.radioBlue_CheckedChanged);
            // 
            // radioGreen
            // 
            resources.ApplyResources(this.radioGreen, "radioGreen");
            this.radioGreen.Name = "radioGreen";
            this.radioGreen.UseVisualStyleBackColor = true;
            this.radioGreen.CheckedChanged += new System.EventHandler(this.radioGreen_CheckedChanged);
            // 
            // numHue
            // 
            this.numHue.Increment = new decimal(new int[] {
            15,
            0,
            0,
            0});
            resources.ApplyResources(this.numHue, "numHue");
            this.numHue.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numHue.Name = "numHue";
            this.numHue.ValueChanged += new System.EventHandler(this.numHue_ValueChanged);
            // 
            // numSaturation
            // 
            this.numSaturation.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            resources.ApplyResources(this.numSaturation, "numSaturation");
            this.numSaturation.Name = "numSaturation";
            this.numSaturation.ValueChanged += new System.EventHandler(this.numSaturation_ValueChanged);
            // 
            // numValue
            // 
            this.numValue.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            resources.ApplyResources(this.numValue, "numValue");
            this.numValue.Name = "numValue";
            this.numValue.ValueChanged += new System.EventHandler(this.numValue_ValueChanged);
            // 
            // numRed
            // 
            resources.ApplyResources(this.numRed, "numRed");
            this.numRed.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numRed.Name = "numRed";
            this.numRed.ValueChanged += new System.EventHandler(this.numRed_ValueChanged);
            // 
            // numGreen
            // 
            resources.ApplyResources(this.numGreen, "numGreen");
            this.numGreen.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numGreen.Name = "numGreen";
            this.numGreen.ValueChanged += new System.EventHandler(this.numGreen_ValueChanged);
            // 
            // numBlue
            // 
            resources.ApplyResources(this.numBlue, "numBlue");
            this.numBlue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numBlue.Name = "numBlue";
            this.numBlue.ValueChanged += new System.EventHandler(this.numBlue_ValueChanged);
            // 
            // textBoxHex
            // 
            resources.ApplyResources(this.textBoxHex, "textBoxHex");
            this.textBoxHex.Name = "textBoxHex";
            this.textBoxHex.TextChanged += new System.EventHandler(this.textBoxHex_TextChanged);
            // 
            // labelHex
            // 
            resources.ApplyResources(this.labelHex, "labelHex");
            this.labelHex.Name = "labelHex";
            // 
            // labelOld
            // 
            resources.ApplyResources(this.labelOld, "labelOld");
            this.labelOld.Name = "labelOld";
            // 
            // labelNew
            // 
            resources.ApplyResources(this.labelNew, "labelNew");
            this.labelNew.Name = "labelNew";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // numAlpha
            // 
            resources.ApplyResources(this.numAlpha, "numAlpha");
            this.numAlpha.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numAlpha.Name = "numAlpha";
            this.numAlpha.ValueChanged += new System.EventHandler(this.numAlpha_ValueChanged);
            // 
            // labelAlpha
            // 
            resources.ApplyResources(this.labelAlpha, "labelAlpha");
            this.labelAlpha.Name = "labelAlpha";
            // 
            // labelHueUnit
            // 
            resources.ApplyResources(this.labelHueUnit, "labelHueUnit");
            this.labelHueUnit.Name = "labelHueUnit";
            // 
            // labelSaturationUnit
            // 
            resources.ApplyResources(this.labelSaturationUnit, "labelSaturationUnit");
            this.labelSaturationUnit.Name = "labelSaturationUnit";
            // 
            // labelValueUnit
            // 
            resources.ApplyResources(this.labelValueUnit, "labelValueUnit");
            this.labelValueUnit.Name = "labelValueUnit";
            // 
            // btnPickColor
            // 
            resources.ApplyResources(this.btnPickColor, "btnPickColor");
            this.btnPickColor.Name = "btnPickColor";
            this.btnPickColor.UseVisualStyleBackColor = true;
            this.btnPickColor.Click += new System.EventHandler(this.btnPickColor_Click);
            this.btnPickColor.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnPickColor_MouseDown);
            // 
            // alphaSlider
            // 
            resources.ApplyResources(this.alphaSlider, "alphaSlider");
            this.alphaSlider.Maximum = System.Drawing.Color.White;
            this.alphaSlider.Minimum = System.Drawing.Color.Transparent;
            this.alphaSlider.Name = "alphaSlider";
            this.alphaSlider.PercentualValueChanged += new System.EventHandler(this.alphaSlider_PercentualValueChanged);
            // 
            // colorShowBox
            // 
            this.colorShowBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorShowBox.Color = System.Drawing.Color.DarkRed;
            resources.ApplyResources(this.colorShowBox, "colorShowBox");
            this.colorShowBox.LowerColor = System.Drawing.Color.Maroon;
            this.colorShowBox.Name = "colorShowBox";
            this.colorShowBox.UpperColor = System.Drawing.Color.DarkRed;
            this.colorShowBox.UpperClick += new System.EventHandler(this.colorShowBox_UpperClick);
            // 
            // colorSlider
            // 
            resources.ApplyResources(this.colorSlider, "colorSlider");
            this.colorSlider.Name = "colorSlider";
            this.colorSlider.PercentualValueChanged += new System.EventHandler(this.colorSlider_PercentualValueChanged);
            // 
            // colorPanel
            // 
            this.colorPanel.BottomLeftColor = System.Drawing.Color.Black;
            this.colorPanel.BottomRightColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.colorPanel, "colorPanel");
            this.colorPanel.Name = "colorPanel";
            this.colorPanel.TopLeftColor = System.Drawing.Color.White;
            this.colorPanel.TopRightColor = System.Drawing.Color.Red;
            this.colorPanel.ValuePercentual = ((System.Drawing.PointF)(resources.GetObject("colorPanel.ValuePercentual")));
            this.colorPanel.PercentualValueChanged += new System.EventHandler(this.colorPanel_PercentualValueChanged);
            // 
            // ColorPickerDialog
            // 
            this.AcceptButton = this.buttonOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.btnPickColor);
            this.Controls.Add(this.labelValueUnit);
            this.Controls.Add(this.labelSaturationUnit);
            this.Controls.Add(this.labelHueUnit);
            this.Controls.Add(this.labelAlpha);
            this.Controls.Add(this.numAlpha);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.labelNew);
            this.Controls.Add(this.labelOld);
            this.Controls.Add(this.labelHex);
            this.Controls.Add(this.textBoxHex);
            this.Controls.Add(this.alphaSlider);
            this.Controls.Add(this.numBlue);
            this.Controls.Add(this.numGreen);
            this.Controls.Add(this.numRed);
            this.Controls.Add(this.numValue);
            this.Controls.Add(this.numSaturation);
            this.Controls.Add(this.numHue);
            this.Controls.Add(this.radioGreen);
            this.Controls.Add(this.radioBlue);
            this.Controls.Add(this.radioRed);
            this.Controls.Add(this.radioValue);
            this.Controls.Add(this.radioSaturation);
            this.Controls.Add(this.radioHue);
            this.Controls.Add(this.colorShowBox);
            this.Controls.Add(this.colorSlider);
            this.Controls.Add(this.colorPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ColorPickerDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ColorPickerDialog_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.numHue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSaturation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAlpha)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private ColorPanel colorPanel;
		private ColorSlider colorSlider;
		private ColorShowBox colorShowBox;
		private System.Windows.Forms.RadioButton radioHue;
		private System.Windows.Forms.RadioButton radioSaturation;
		private System.Windows.Forms.RadioButton radioValue;
		private System.Windows.Forms.RadioButton radioRed;
		private System.Windows.Forms.RadioButton radioBlue;
		private System.Windows.Forms.RadioButton radioGreen;
		private System.Windows.Forms.NumericUpDown numHue;
		private System.Windows.Forms.NumericUpDown numSaturation;
		private System.Windows.Forms.NumericUpDown numValue;
		private System.Windows.Forms.NumericUpDown numRed;
		private System.Windows.Forms.NumericUpDown numGreen;
		private System.Windows.Forms.NumericUpDown numBlue;
		private ColorSlider alphaSlider;
		private System.Windows.Forms.TextBox textBoxHex;
		private System.Windows.Forms.Label labelHex;
		private System.Windows.Forms.Label labelOld;
		private System.Windows.Forms.Label labelNew;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.NumericUpDown numAlpha;
		private System.Windows.Forms.Label labelAlpha;
		private System.Windows.Forms.Label labelHueUnit;
		private System.Windows.Forms.Label labelSaturationUnit;
		private System.Windows.Forms.Label labelValueUnit;
        private System.Windows.Forms.Button btnPickColor;
	}
}