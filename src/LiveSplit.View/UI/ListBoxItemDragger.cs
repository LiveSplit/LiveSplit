using System;
using System.Windows.Forms;

using LiveSplit.Options;
using LiveSplit.Utils;

namespace LiveSplit.UI;

/// <summary>
/// Turn on item dragging for some ListBox control
/// </summary>
public class ListBoxItemDragger
{
    private ListBox listBox;
    public Form Form { get; set; }

    /// <summary>
    /// Gets the index of the dragged item.
    /// </summary>
    /// <value>The index of the dragged item.</value>
    public int DragItemIndex { get; private set; } = -1;

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

    public Cursor DragCursor { get; set; } = Cursors.SizeNS;

    /// <summary>
    /// Raises the <see cref="E:ItemMoved"> event.
    /// </see></summary>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    protected void OnItemMoved(EventArgs e)
    {
        ItemMoved?.Invoke(this, e);
    }

    /// <summary>
    /// Occurs when some item has been moved
    /// </summary>
    public event EventHandler ItemMoved;

    private void MouseDownHandler(object sender, MouseEventArgs e)
    {
        DragItemIndex = listBox.SelectedIndex;
    }

    private Cursor prevCursor = Cursors.Default;

    private void MouseUpHandler(object sender, MouseEventArgs e)
    {
        DragItemIndex = -1;
        if (dragging)
        {
            listBox.Cursor = prevCursor;
            dragging = false;
        }
    }

    private void MouseMoveHandler(object sender, MouseEventArgs e)
    {
        Form.InvokeIfRequired(() =>
        {
            try
            {
                if (DragItemIndex >= 0 && e.Y > 0)
                {
                    if (!dragging)
                    {
                        dragging = true;
                        prevCursor = listBox.Cursor;
                        listBox.Cursor = DragCursor;
                    }

                    int dstIndex = listBox.IndexFromPoint(e.X, e.Y);

                    if (DragItemIndex != dstIndex)
                    {
                        dynamic item = listBox.Items[DragItemIndex];
                        listBox.BeginUpdate();
                        try
                        {
                            dynamic bindingList = listBox.DataSource;

                            bindingList.RemoveAt(DragItemIndex);
                            if (dstIndex != ListBox.NoMatches)
                            {
                                bindingList.Insert(dstIndex, item);
                            }
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

                        DragItemIndex = dstIndex;
                        OnItemMoved(EventArgs.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        });
    }
}
