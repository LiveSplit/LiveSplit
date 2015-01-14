using LiveSplit.Options;
using System;
using System.Windows.Forms;

namespace LiveSplit.UI
{

    /// <summary>
    /// Turn on item dragging for some ListBox control
    /// </summary>
    public class ListBoxItemDragger
    {
        private ListBox listBox;
        public Form Form { get; set; }

        //public ReaderWriterLockSlim DrawLock { get; set; }

        private int dragItemIndex = -1;

        /// <summary>
        /// Gets the index of the dragged item.
        /// </summary>
        /// <value>The index of the dragged item.</value>
        public int DragItemIndex
        {
            get { return dragItemIndex; }
        }

        private bool dragging = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ListBoxItemDragger"> class.
        /// </see></summary>
        /// <param name="listBox">The list box</param>
        /// <param name="form">The form</param>
        public ListBoxItemDragger(ListBox listBox, Form form)
        {
            Attach(listBox);
            Form = form;
        }

        /// <summary>
        /// Attaches current instance to some ListBox control.
        /// </summary>
        public void Attach(ListBox listBox)
        {
            this.listBox = listBox;
            this.listBox.MouseDown += new MouseEventHandler(MouseDownHandler);
            this.listBox.MouseUp += new MouseEventHandler(MouseUpHandler);
            this.listBox.MouseMove += new MouseEventHandler(MouseMoveHandler);
        }

        /// <summary>
        /// Detaches current instance from ListBox control.
        /// </summary>
        public void Detach()
        {
            listBox.MouseDown -= new MouseEventHandler(MouseDownHandler);
            listBox.MouseUp -= new MouseEventHandler(MouseUpHandler);
            listBox.MouseMove -= new MouseEventHandler(MouseMoveHandler);
        }

        private Cursor dragCursor = Cursors.SizeNS;
        public Cursor DragCursor
        {
            get { return dragCursor; }
            set { dragCursor = value; }
        }

        /// <summary>
        /// Raises the <see cref="E:ItemMoved"> event.
        /// </see></summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void OnItemMoved(EventArgs e)
        {
            if (ItemMoved != null) ItemMoved(this, e);
        }

        /// <summary>
        /// Occurs when some item has been moved
        /// </summary>
        public event EventHandler ItemMoved;

        private void MouseDownHandler(object sender, MouseEventArgs e)
        {
            dragItemIndex = listBox.SelectedIndex;
        }


        private Cursor prevCursor = Cursors.Default;

        private void MouseUpHandler(object sender, MouseEventArgs e)
        {
            dragItemIndex = -1;
            if (dragging)
            {
                listBox.Cursor = prevCursor;
                dragging = false;
            }
        }

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            //if (!DrawLock.TryEnterWriteLock(500))
            //return;
            Action x = () =>
            {
                try
                {
                    if (dragItemIndex >= 0 && e.Y > 0)
                    {
                        if (!dragging)
                        {
                            dragging = true;
                            prevCursor = listBox.Cursor;
                            listBox.Cursor = DragCursor;
                        }
                        int dstIndex = listBox.IndexFromPoint(e.X, e.Y);

                        if (dragItemIndex != dstIndex)
                        {
                            dynamic item = listBox.Items[dragItemIndex];
                            listBox.BeginUpdate();
                            try
                            {
                                dynamic bindingList = listBox.DataSource;

                                bindingList.RemoveAt(dragItemIndex);
                                if (dstIndex != ListBox.NoMatches)
                                    bindingList.Insert(dstIndex, item);
                                else
                                {
                                    bindingList.Add(item);
                                    dstIndex = bindingList.Count - 1;
                                }

                                listBox.SelectedIndex = dstIndex;
                            }
                            finally
                            {
                                listBox.EndUpdate();
                            }
                            dragItemIndex = dstIndex;
                            OnItemMoved(EventArgs.Empty);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            };

            if (Form.InvokeRequired)
                Form.Invoke(x);
            else
                x();
            //DrawLock.ExitWriteLock();
        }

    }
}