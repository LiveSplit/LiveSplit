using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LiveSplit.UI.Components
{
    public abstract class ControlComponent : IDeactivatableComponent
    {
        public abstract string ComponentName { get; }

        protected Form Form { get; set; }
        protected Control Control { get; set; }

        public bool Activated
        {
            get { return Control.Visible; }
            set { Control.Visible = activated = value; }
        }
        protected bool activated;

        protected bool HasInvalidated = false;

        public abstract float HorizontalWidth { get; }
        public abstract float MinimumHeight { get; }
        public abstract float VerticalHeight { get; }
        public abstract float MinimumWidth { get; }

        public float PaddingTop
        {
            get { return 0; }
        }
        public float PaddingBottom
        {
            get { return 0; }
        }
        public float PaddingLeft
        {
            get { return 0; }
        }
        public float PaddingRight
        {
            get { return 0; }
        }

        public IDictionary<string, Action> ContextMenuControls { get; protected set; }

        protected ControlComponent(LiveSplitState state, Control control, Action<Exception> errorCallback = null)
        {
            try
            {
                Form = state.Form;
                Control = control;
                activated = control.Visible;
                Form.SuspendLayout();
                Form.Controls.Add(control);
                Form.ResumeLayout(false);
            }
            catch (Exception ex)
            {
                if (errorCallback != null)
                    errorCallback(ex);
                throw ex;
            }
        }

        public void InvokeIfNeeded(Action x)
        {
            if (Form != null && Form.InvokeRequired)
                Form.Invoke(x);
            else
                x();
        }

        public void Reposition(float width, float height, Graphics g)
        {
            var points = new PointF[]
            {
                new PointF(0, 0),
                new PointF(width, height)
            };
            g.Transform.TransformPoints(points);

            InvokeIfNeeded(() =>
            {
                lock (Control)
                {
                    Control.Location = new System.Drawing.Point((int)(points[0].X + 0.5f) + 1, (int)(points[0].Y + 0.5f) + 1);
                    Control.Size = new System.Drawing.Size((int)(points[1].X - points[0].X + 0.5f) - 2, (int)(points[1].Y - points[0].Y + 0.5f) - 2);
                }
            });
        }

        public virtual void DrawHorizontal(System.Drawing.Graphics g, Model.LiveSplitState state, float height, System.Drawing.Region clipRegion)
        {
            Reposition(HorizontalWidth, height, g);
        }

        public virtual void DrawVertical(System.Drawing.Graphics g, Model.LiveSplitState state, float width, System.Drawing.Region clipRegion)
        {
            Reposition(width, VerticalHeight, g);
        }

        public abstract System.Windows.Forms.Control GetSettingsControl(LayoutMode mode);

        public abstract System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document);

        public abstract void SetSettings(System.Xml.XmlNode settings);

        public virtual void Update(IInvalidator invalidator, Model.LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (!HasInvalidated && invalidator != null)
            {
                invalidator.Invalidate(0, 0, width, height);
                HasInvalidated = true;
            }
        }

        public virtual void Dispose()
        {
            Form.Controls.Remove(Control);
            Control.Dispose();
        }
    }
}
