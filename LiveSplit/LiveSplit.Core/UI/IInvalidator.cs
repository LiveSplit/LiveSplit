using System.Drawing.Drawing2D;

namespace LiveSplit.UI
{
    public interface IInvalidator
    {
        Matrix Transform { get; set; }
        void Invalidate(float x, float y, float width, float height);
    }
    public static class InvalidatorExtensions
    {
        /*public static void Invalidate(this IInvalidator invalidator, SimpleLabel label)
        {
            invalidator.Invalidate(label.X, label.Y, label.Width, label.Height);
        }*/
    }
}
