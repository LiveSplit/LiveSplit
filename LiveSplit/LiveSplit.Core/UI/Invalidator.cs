using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LiveSplit.UI
{
    public class Invalidator : IInvalidator
    {
        public Form Form { get; protected set; }
        public Matrix Transform { get; set; }

        public Invalidator(Form form)
        {
            Transform = new Matrix();
            Form = form;
        }

        public void Restart()
        {
            try
            {
                if (Transform != null)
                    Transform.Dispose();
            }
            catch { }

            Transform = new Matrix();
        }

        public void Invalidate(float x, float y, float width, float height)
        {
            var points = new PointF[]
                {
                new PointF(x, y),
                new PointF(x+width, y+height)
                };
            Transform.TransformPoints(points);
            var roundedX = (int)(points[0].X + 0.5f);
            var roundedY = (int)(points[0].Y + 0.5f);
            var rect = new Rectangle(
                roundedX,
                roundedY,
                (int)(points[1].X - roundedX + 1.5f),
                (int)(points[1].Y - roundedY + 1.5f));
            Form.Invalidate(rect);
        }
    }
}
