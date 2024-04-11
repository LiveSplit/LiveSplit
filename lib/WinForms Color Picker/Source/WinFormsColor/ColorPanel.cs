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
    public class ColorPanel : UserControl
    {
        private	Bitmap	srcImage		= null;
        private	bool	pickerDrag		= false;
        private	int		pickerSize		= 8;
        private	PointF	pickerPos		= new PointF(0.5f, 0.5f);
		private	Color	clrTopLeft		= Color.Transparent;
		private	Color	clrTopRight		= Color.Transparent;
		private	Color	clrBottomLeft	= Color.Transparent;
		private	Color	clrBottomRight	= Color.Transparent;
        private	Color	valTemp			= Color.Transparent;
		private	Timer	pickerDragTimer	= null;
		private	bool	designSerializeColor = false;


		public event EventHandler ValueChanged = null;
		public event EventHandler PercentualValueChanged = null;


        public Rectangle ColorAreaRectangle
        {
            get { return new Rectangle(
                this.ClientRectangle.X + 2,
                this.ClientRectangle.Y + 2,
                this.ClientRectangle.Width - 4,
                this.ClientRectangle.Height - 4); }
        }
		[DefaultValue(8)]
        public int PickerSize
        {
            get { return this.pickerSize; }
            set { this.pickerSize = value; this.Invalidate(); }
        }
		[DefaultValue(0.5f)]
        public PointF ValuePercentual
        {
            get { return this.pickerPos; }
            set
            {
				PointF last = this.pickerPos;
                this.pickerPos = new PointF(
                    Math.Min(1.0f, Math.Max(0.0f, value.X)),
                    Math.Min(1.0f, Math.Max(0.0f, value.Y)));
				if (this.pickerPos != last)
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
		public Color TopLeftColor
		{
			get { return this.clrTopLeft; }
			set
			{
				if (this.clrTopLeft != value)
				{
					this.SetupGradient(value, this.clrTopRight, this.clrBottomLeft, this.clrBottomRight);
					this.designSerializeColor = true;
				}
			}
		}
		public Color TopRightColor
		{
			get { return this.clrTopRight; }
			set
			{
				if (this.clrTopRight != value)
				{
					this.SetupGradient(this.clrTopLeft, value, this.clrBottomLeft, this.clrBottomRight);
					this.designSerializeColor = true;
				}
			}
		}
		public Color BottomLeftColor
		{
			get { return this.clrBottomLeft; }
			set
			{
				if (this.clrBottomLeft != value)
				{
					this.SetupGradient(this.clrTopLeft, this.clrTopRight, value, this.clrBottomRight);
					this.designSerializeColor = true;
				}
			}
		}
		public Color BottomRightColor
		{
			get { return this.clrBottomRight; }
			set
			{
				if (this.clrBottomRight != value)
				{
					this.SetupGradient(this.clrTopLeft, this.clrTopRight, this.clrBottomLeft, value);
					this.designSerializeColor = true;
				}
			}
		}


        public ColorPanel()
        {
			this.pickerDragTimer = new Timer();
			this.pickerDragTimer.Interval = 10;
			this.pickerDragTimer.Tick += new EventHandler(pickerDragTimer_Tick);

			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.Opaque, true);
			this.SetStyle(ControlStyles.Selectable, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.SetupHueBrightnessGradient();
        }
		
		public void SetupGradient(Color tl, Color tr, Color bl, Color br, int accuracy = 256)
		{
			accuracy = Math.Max(1, accuracy);

			this.clrTopLeft = tl;
			this.clrTopRight = tr;
			this.clrBottomLeft = bl;
			this.clrBottomRight = br;
			this.srcImage = new Bitmap(accuracy, accuracy);
			Bitmap tempImg = new Bitmap(2, 2);
			using (Graphics g = Graphics.FromImage(tempImg))
			{
				g.DrawRectangle(new Pen(tl), 0, 0, 1, 1);
				g.DrawRectangle(new Pen(tr), 1, 0, 1, 1);
				g.DrawRectangle(new Pen(bl), 0, 1, 1, 1);
				g.DrawRectangle(new Pen(br), 1, 1, 1, 1);
			}
			using (Graphics g = Graphics.FromImage(this.srcImage))
			{
				g.DrawImage(
					tempImg, 
					new Rectangle(0, 0, this.srcImage.Width, this.srcImage.Height), 
					0, 0, tempImg.Width - 1, tempImg.Height - 1,
					GraphicsUnit.Pixel);
			}
			this.UpdateColorValue();
			this.Invalidate();
		}
		public void SetupXYGradient(Color left, Color right, Color bottom, Color top, int accuracy = 256)
		{
			accuracy = Math.Max(1, accuracy);
			
			this.srcImage = new Bitmap(accuracy, accuracy);
			using (Graphics g = Graphics.FromImage(this.srcImage))
			{
				LinearGradientBrush gradient;
				gradient = new LinearGradientBrush(
					new Point(0, 0),
					new Point(this.srcImage.Width - 1, 0),
					left,
					right);
				g.FillRectangle(gradient, g.ClipBounds);
				gradient = new LinearGradientBrush(
					new Point(0, this.srcImage.Height - 1),
					new Point(0, 0),
					bottom,
					top);
				g.FillRectangle(gradient, g.ClipBounds);
			}
			this.clrTopLeft		= this.srcImage.GetPixel(0, 0);
			this.clrTopRight	= this.srcImage.GetPixel(this.srcImage.Width - 1, 0);
			this.clrBottomLeft	= this.srcImage.GetPixel(0, this.srcImage.Height - 1);
			this.clrBottomRight	= this.srcImage.GetPixel(this.srcImage.Width - 1, this.srcImage.Height - 1);
			this.UpdateColorValue();
			this.Invalidate();
		}
		public void SetupXYGradient(ColorBlend blendX, ColorBlend blendY, int accuracy = 256)
		{
			accuracy = Math.Max(1, accuracy);
			
			this.srcImage = new Bitmap(accuracy, accuracy);
			using (Graphics g = Graphics.FromImage(this.srcImage))
			{
				LinearGradientBrush gradient;
				gradient = new LinearGradientBrush(
					new Point(0, 0),
					new Point(this.srcImage.Width - 1, 0),
					Color.Transparent,
					Color.Transparent);
				gradient.InterpolationColors = blendX;
				g.FillRectangle(gradient, g.ClipBounds);
				gradient = new LinearGradientBrush(
					new Point(0, this.srcImage.Height - 1),
					new Point(0, 0),
					Color.Transparent,
					Color.Transparent);
				gradient.InterpolationColors = blendY;
				g.FillRectangle(gradient, g.ClipBounds);
			}
			this.clrTopLeft		= this.srcImage.GetPixel(0, 0);
			this.clrTopRight	= this.srcImage.GetPixel(this.srcImage.Width - 1, 0);
			this.clrBottomLeft	= this.srcImage.GetPixel(0, this.srcImage.Height - 1);
			this.clrBottomRight	= this.srcImage.GetPixel(this.srcImage.Width - 1, this.srcImage.Height - 1);
			this.UpdateColorValue();
			this.Invalidate();
		}
		public void SetupHueBrightnessGradient(float saturation = 1.0f, int accuracy = 256)
		{
			ColorBlend blendX = new ColorBlend();
			blendX.Colors = new Color[] {
				ExtMethodsSystemDrawingColor.ColorFromHSV(0.0f, saturation, 1.0f),
				ExtMethodsSystemDrawingColor.ColorFromHSV(1.0f / 6.0f, saturation, 1.0f),
				ExtMethodsSystemDrawingColor.ColorFromHSV(2.0f / 6.0f, saturation, 1.0f),
				ExtMethodsSystemDrawingColor.ColorFromHSV(3.0f / 6.0f, saturation, 1.0f),
				ExtMethodsSystemDrawingColor.ColorFromHSV(4.0f / 6.0f, saturation, 1.0f),
				ExtMethodsSystemDrawingColor.ColorFromHSV(5.0f / 6.0f, saturation, 1.0f),
				ExtMethodsSystemDrawingColor.ColorFromHSV(1.0f, saturation, 1.0f) };
			blendX.Positions = new float[] {
				0.0f,
				1.0f / 6.0f,
				2.0f / 6.0f,
				3.0f / 6.0f,
				4.0f / 6.0f,
				5.0f / 6.0f,
				1.0f};

			ColorBlend blendY = new ColorBlend();
			blendY.Colors = new Color[] {
				Color.FromArgb(0, 0, 0),
				Color.Transparent };
			blendY.Positions = new float[] {
				0.0f,
				1.0f};
			this.SetupXYGradient(blendX, blendY, accuracy);
		}
		public void SetupHueSaturationGradient(float brightness = 1.0f, int accuracy = 256)
		{
			ColorBlend blendX = new ColorBlend();
			blendX.Colors = new Color[] {
				ExtMethodsSystemDrawingColor.ColorFromHSV(0.0f, 1.0f, brightness),
				ExtMethodsSystemDrawingColor.ColorFromHSV(1.0f / 6.0f, 1.0f, brightness),
				ExtMethodsSystemDrawingColor.ColorFromHSV(2.0f / 6.0f, 1.0f, brightness),
				ExtMethodsSystemDrawingColor.ColorFromHSV(3.0f / 6.0f, 1.0f, brightness),
				ExtMethodsSystemDrawingColor.ColorFromHSV(4.0f / 6.0f, 1.0f, brightness),
				ExtMethodsSystemDrawingColor.ColorFromHSV(5.0f / 6.0f, 1.0f, brightness),
				ExtMethodsSystemDrawingColor.ColorFromHSV(1.0f, 1.0f, brightness) };
			blendX.Positions = new float[] {
				0.0f,
				1.0f / 6.0f,
				2.0f / 6.0f,
				3.0f / 6.0f,
				4.0f / 6.0f,
				5.0f / 6.0f,
				1.0f};

			ColorBlend blendY = new ColorBlend();
			blendY.Colors = new Color[] {
				Color.FromArgb((int)Math.Round(255.0f * brightness), (int)Math.Round(255.0f * brightness), (int)Math.Round(255.0f * brightness)),
				Color.Transparent };
			blendY.Positions = new float[] {
				0.0f,
				1.0f};
			this.SetupXYGradient(blendX, blendY, accuracy);
		}

        protected void UpdateColorValue()
        {
			Color oldVal = this.valTemp;
            this.valTemp = this.srcImage.GetPixel(
                (int)Math.Round((this.srcImage.Width - 1) * this.pickerPos.X), 
                (int)Math.Round((this.srcImage.Height - 1) * (1.0f - this.pickerPos.Y)));
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle colorBoxOuter = new Rectangle(
                this.ClientRectangle.X,
                this.ClientRectangle.Y,
                this.ClientRectangle.Width - 1,
                this.ClientRectangle.Height - 1);
            Rectangle colorBoxInner = new Rectangle(
                colorBoxOuter.X + 1,
                colorBoxOuter.Y + 1,
                colorBoxOuter.Width - 2,
                colorBoxOuter.Height - 2);
            Rectangle colorArea = this.ColorAreaRectangle;
            Point pickerVisualPos = new Point(
				colorArea.X + (int)Math.Round(this.pickerPos.X * colorArea.Width),
				colorArea.Y + (int)Math.Round((1.0f - this.pickerPos.Y) * colorArea.Height));

			if (this.clrBottomLeft.A < 255 || this.clrBottomRight.A < 255 || this.clrTopLeft.A < 255 || this.clrTopRight.A < 255)
				e.Graphics.FillRectangle(new HatchBrush(HatchStyle.LargeCheckerBoard, Color.LightGray, Color.Gray), colorArea);
           
			e.Graphics.DrawImage(this.srcImage, colorArea, 0, 0, this.srcImage.Width - 1, this.srcImage.Height - 1, GraphicsUnit.Pixel);

			Pen innerPickerPen = this.valTemp.GetLuminance() > 0.5f ? Pens.Black : Pens.White;
			if (this.Enabled)
			{
				e.Graphics.DrawEllipse(innerPickerPen,
					pickerVisualPos.X - this.pickerSize / 2,
					pickerVisualPos.Y - this.pickerSize / 2,
					this.pickerSize,
					this.pickerSize);
			}
			else
			{
				e.Graphics.DrawRectangle(innerPickerPen,
					pickerVisualPos.X - this.pickerSize / 4,
					pickerVisualPos.Y - this.pickerSize / 4,
					this.pickerSize / 2,
					this.pickerSize / 2);
			}

			if (!this.Enabled)
			{
				e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, SystemColors.Control)), colorArea);
			}

            e.Graphics.DrawRectangle(SystemPens.ControlDark, colorBoxOuter);
            e.Graphics.DrawRectangle(SystemPens.ControlLightLight, colorBoxInner);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.Focus();
                this.ValuePercentual = new PointF(
					(float)(e.X - this.ColorAreaRectangle.X) / (float)this.ColorAreaRectangle.Width,
					1.0f - (float)(e.Y - this.ColorAreaRectangle.Y) / (float)this.ColorAreaRectangle.Height);
				this.pickerDragTimer.Start();
                this.pickerDrag = true;
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            this.pickerDrag = false;
			this.pickerDragTimer.Stop();
        }
		private void pickerDragTimer_Tick(object sender, EventArgs e)
		{
			Point pos = this.PointToClient(System.Windows.Forms.Cursor.Position);
			this.ValuePercentual = new PointF(
				(float)(pos.X - this.ColorAreaRectangle.X) / (float)this.ColorAreaRectangle.Width,
				1.0f - (float)(pos.Y - this.ColorAreaRectangle.Y) / (float)this.ColorAreaRectangle.Height);
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

		private void ResetTopLeftColor()
		{
			this.SetupHueBrightnessGradient();
			this.designSerializeColor = false;
		}
		private void ResetTopRightColor()
		{
			this.SetupHueBrightnessGradient();
			this.designSerializeColor = false;
		}
		private void ResetBottomLeftColor()
		{
			this.SetupHueBrightnessGradient();
			this.designSerializeColor = false;
		}
		private void ResetBottomRightColor()
		{
			this.SetupHueBrightnessGradient();
			this.designSerializeColor = false;
		}
		private bool ShouldSerializeTopLeftColor()
		{
			return this.designSerializeColor;
		}
		private bool ShouldSerializeTopRightColor()
		{
			return this.designSerializeColor;
		}
		private bool ShouldSerializeBottomLeftColor()
		{
			return this.designSerializeColor;
		}
		private bool ShouldSerializeBottomRightColor()
		{
			return this.designSerializeColor;
		}
    }
}
