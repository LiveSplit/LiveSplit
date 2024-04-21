using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace Fetze.WinFormsColor
{
	public class ColorSlider : UserControl
	{
		private	Bitmap	srcImage	= null;
		private	bool	pickerDrag	= false;
		private	int		pickerSize	= 5;
		private	float	pickerPos	= 0.5f;
		private	Color	min			= Color.Transparent;
		private	Color	max			= Color.Transparent;
		private	Color	valTemp		= Color.Transparent;
		private	bool	innerPicker	= true;
		private	Timer	pickerDragTimer	= null;
		private	bool	designSerializeColor = false;


		public event EventHandler ValueChanged = null;
		public event EventHandler PercentualValueChanged = null;


		public Rectangle ColorAreaRectangle
		{
			get { return new Rectangle(
				this.ClientRectangle.X + this.pickerSize + 2,
				this.ClientRectangle.Y + this.pickerSize + 2,
				this.ClientRectangle.Width - this.pickerSize * 2 - 4,
				this.ClientRectangle.Height - this.pickerSize * 2 - 4); }
		}
		[DefaultValue(5)]
		public int PickerSize
		{
			get { return this.pickerSize; }
			set { this.pickerSize = value; this.Invalidate(); }
		}
		[DefaultValue(true)]
		public bool ShowInnerPicker
		{
			get { return this.innerPicker; }
			set { this.innerPicker = value; }
		}
		[DefaultValue(0.5f)]
		public float ValuePercentual
		{
			get { return this.pickerPos; }
			set
			{
				float lastVal = this.pickerPos;
				this.pickerPos = Math.Min(1.0f, Math.Max(0.0f, value));
				if (this.pickerPos != lastVal)
				{
					this.OnPercentualValueChanged();
					this.UpdateColorValue();
					this.Invalidate();
				}
			}
		}
		public Color Value
		{
			get
			{
				return this.valTemp;
			}
		}
		public Color Minimum
		{
			get { return this.min; }
			set
			{
				if (this.min != value)
				{
					this.SetupGradient(value, this.max);
					this.designSerializeColor = true;
				}
			}
		}
		public Color Maximum
		{
			get { return this.max; }
			set
			{
				if (this.max != value)
				{
					this.SetupGradient(this.min, value);
					this.designSerializeColor = true;
				}
			}
		}


		public ColorSlider()
		{
			this.pickerDragTimer = new Timer();
			this.pickerDragTimer.Interval = 10;
			this.pickerDragTimer.Tick += new EventHandler(pickerDragTimer_Tick);

			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.Selectable, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.SetupHueGradient();
		}

		public void SetupGradient(Color min, Color max, int accuracy = 256)
		{
			accuracy = Math.Max(1, accuracy);

			this.min = min;
			this.max = max;
			this.srcImage = new Bitmap(1, accuracy);
			using (Graphics g = Graphics.FromImage(this.srcImage))
			{
				LinearGradientBrush gradient = new LinearGradientBrush(
					new Point(0, this.srcImage.Height - 1),
					new Point(0, 0),
					min,
					max);
				g.FillRectangle(gradient, g.ClipBounds);
			}
			this.UpdateColorValue();
			this.Invalidate();
		}
		public void SetupGradient(ColorBlend blend, int accuracy = 256)
		{
			accuracy = Math.Max(1, accuracy);

			this.srcImage = new Bitmap(1, accuracy);
			using (Graphics g = Graphics.FromImage(this.srcImage))
			{
				LinearGradientBrush gradient = new LinearGradientBrush(
					new Point(0, this.srcImage.Height - 1),
					new Point(0, 0),
					Color.Transparent,
					Color.Transparent);
				gradient.InterpolationColors = blend;
				g.FillRectangle(gradient, g.ClipBounds);
			}
			this.min = this.srcImage.GetPixel(0, this.srcImage.Height - 1);
			this.max = this.srcImage.GetPixel(0, 0);
			this.UpdateColorValue();
			this.Invalidate();
		}
		public void SetupHueGradient(float saturation = 1.0f, float brightness = 1.0f, int accuracy = 256)
		{
			ColorBlend blend = new ColorBlend();
			blend.Colors = new Color[] {
				ExtMethodsSystemDrawingColor.ColorFromHSV(0.0f, saturation, brightness),
				ExtMethodsSystemDrawingColor.ColorFromHSV(1.0f / 6.0f, saturation, brightness),
				ExtMethodsSystemDrawingColor.ColorFromHSV(2.0f / 6.0f, saturation, brightness),
				ExtMethodsSystemDrawingColor.ColorFromHSV(3.0f / 6.0f, saturation, brightness),
				ExtMethodsSystemDrawingColor.ColorFromHSV(4.0f / 6.0f, saturation, brightness),
				ExtMethodsSystemDrawingColor.ColorFromHSV(5.0f / 6.0f, saturation, brightness),
				ExtMethodsSystemDrawingColor.ColorFromHSV(1.0f, saturation, brightness) };
			blend.Positions = new float[] {
				0.0f,
				1.0f / 6.0f,
				2.0f / 6.0f,
				3.0f / 6.0f,
				4.0f / 6.0f,
				5.0f / 6.0f,
				1.0f};
			this.SetupGradient(blend, accuracy);
		}

		protected void UpdateColorValue()
		{
			Color oldVal = this.valTemp;
			this.valTemp = this.srcImage.GetPixel(0, (int)Math.Round((this.srcImage.Height - 1) * (1.0f - this.pickerPos)));
			if (oldVal != this.valTemp) this.OnValueChanged();
		}

		protected void OnValueChanged()
		{
			if (this.ValueChanged != null)
				this.ValueChanged(this, null);
		}
		protected void OnPercentualValueChanged()
		{
			if (this.PercentualValueChanged != null)
				this.PercentualValueChanged(this, null);
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);
			this.pickerDrag = false;
			this.Invalidate();
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			Rectangle colorBoxOuter = new Rectangle(
				this.ClientRectangle.X + this.pickerSize,
				this.ClientRectangle.Y + this.pickerSize,
				this.ClientRectangle.Width - this.pickerSize * 2 - 1,
				this.ClientRectangle.Height - this.pickerSize * 2 - 1);
			Rectangle colorBoxInner = new Rectangle(
				colorBoxOuter.X + 1,
				colorBoxOuter.Y + 1,
				colorBoxOuter.Width - 2,
				colorBoxOuter.Height - 2);
			Rectangle colorArea = this.ColorAreaRectangle;
			int pickerVisualPos = colorArea.Y + (int)Math.Round((1.0f - this.pickerPos) * colorArea.Height);

			if (this.min.A < 255 || this.max.A < 255)
				e.Graphics.FillRectangle(new HatchBrush(HatchStyle.LargeCheckerBoard, Color.LightGray, Color.Gray), colorArea);

			System.Drawing.Imaging.ImageAttributes colorAreaImageAttr = new System.Drawing.Imaging.ImageAttributes();
			colorAreaImageAttr.SetWrapMode(WrapMode.TileFlipXY);
			e.Graphics.DrawImage(this.srcImage, colorArea, 0, 0, this.srcImage.Width, this.srcImage.Height - 1, GraphicsUnit.Pixel, colorAreaImageAttr);

			e.Graphics.DrawRectangle(SystemPens.ControlDark, colorBoxOuter);
			e.Graphics.DrawRectangle(SystemPens.ControlLightLight, colorBoxInner);

			e.Graphics.DrawLines(this.Enabled ? Pens.Black : SystemPens.ControlDark, new Point[] {
				new Point(0, pickerVisualPos - this.pickerSize),
				new Point(this.pickerSize, pickerVisualPos),
				new Point(0, pickerVisualPos + this.pickerSize),
				new Point(0, pickerVisualPos - this.pickerSize)});
			e.Graphics.DrawLines(this.Enabled ? Pens.Black : SystemPens.ControlDark, new Point[] {
				new Point(colorBoxOuter.Right + this.pickerSize, pickerVisualPos - this.pickerSize),
				new Point(colorBoxOuter.Right, pickerVisualPos),
				new Point(colorBoxOuter.Right + this.pickerSize, pickerVisualPos + this.pickerSize),
				new Point(colorBoxOuter.Right + this.pickerSize, pickerVisualPos - this.pickerSize)});
			
			if (this.innerPicker)
			{
				Pen innerPickerPen = this.valTemp.GetLuminance() > 0.5f ? Pens.Black : Pens.White;
				e.Graphics.DrawLine(innerPickerPen,
					new Point(colorArea.Left, pickerVisualPos),
					new Point(colorArea.Left + 2, pickerVisualPos));
				e.Graphics.DrawLine(innerPickerPen,
					new Point(colorArea.Right - 1, pickerVisualPos),
					new Point(colorArea.Right - 1 - 2, pickerVisualPos));
			}

			if (!this.Enabled)
			{
				e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, SystemColors.Control)), colorArea);
			}
		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				this.Focus();
				this.ValuePercentual = 1.0f - (float)(e.Y - this.ColorAreaRectangle.Y) / (float)this.ColorAreaRectangle.Height;
				this.pickerDrag = true;
				this.pickerDragTimer.Enabled = true;
			}
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			this.pickerDrag = false;
			this.pickerDragTimer.Enabled = false;
		}
		
		private void pickerDragTimer_Tick(object sender, EventArgs e)
		{
			Point pos = this.PointToClient(System.Windows.Forms.Cursor.Position);
			this.ValuePercentual = 1.0f - (float)(pos.Y - this.ColorAreaRectangle.Y) / (float)this.ColorAreaRectangle.Height;
		}
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			this.pickerDrag = false;
		}
		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			this.Invalidate();
		}
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			this.Invalidate();
		}

		private void ResetMinimum()
		{
			this.SetupHueGradient();
			this.designSerializeColor = false;
		}
		private void ResetMaximum()
		{
			this.SetupHueGradient();
			this.designSerializeColor = false;
		}
		private bool ShouldSerializeMinimum()
		{
			return this.designSerializeColor;
		}
		private bool ShouldSerializeMaximum()
		{
			return this.designSerializeColor;
		}
	}
}
