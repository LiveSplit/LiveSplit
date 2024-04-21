using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Fetze.WinFormsColor
{
	public class ColorShowBox : UserControl
	{
		private	Color	upperColor	= Color.Transparent;
		private	Color	lowerColor	= Color.Transparent;


		public event EventHandler UpperClick = null;
		public event EventHandler LowerClick = null;


		public Color Color
		{
			get { return this.upperColor; }
			set { this.upperColor = this.lowerColor = value; this.Invalidate(); }
		}
		public Color UpperColor
		{
			get { return this.upperColor; }
			set { this.upperColor = value; this.Invalidate(); }
		}
		public Color LowerColor
		{
			get { return this.lowerColor; }
			set { this.lowerColor = value; this.Invalidate(); }
		}


		public ColorShowBox()
		{
			this.DoubleBuffered = true;
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		}
		
		protected void OnUpperClick()
		{
			if (this.UpperClick != null)
				this.UpperClick(this, null);
		}
		protected void OnLowerClick()
		{
			if (this.LowerClick != null)
				this.LowerClick(this, null);
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
			if (e.Y > (this.ClientRectangle.Top + this.ClientRectangle.Bottom) / 2)
				this.OnLowerClick();
			else
				this.OnUpperClick();
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			e.Graphics.FillRectangle(new HatchBrush(HatchStyle.LargeCheckerBoard, Color.LightGray, Color.Gray), this.ClientRectangle);
			
			e.Graphics.FillRectangle(new SolidBrush(this.upperColor),
				this.ClientRectangle.X,
				this.ClientRectangle.Y,
				this.ClientRectangle.Width,
				this.ClientRectangle.Height / 2 + 1);
			e.Graphics.FillRectangle(new SolidBrush(this.lowerColor),
				this.ClientRectangle.X,
				this.ClientRectangle.Y + this.ClientRectangle.Height - this.ClientRectangle.Height / 2,
				this.ClientRectangle.Width,
				this.ClientRectangle.Height / 2);
		}
	}
}
