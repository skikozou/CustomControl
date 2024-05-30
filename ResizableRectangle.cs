using System;
using System.Drawing;
using System.Windows.Forms;

public class ResizableRectangle : Control
{
    private bool isDragging = false;
    private Point clickOffset;

    private bool isResizing = false;
    private ResizeDirection resizeDirection;
    private const int handleSize = 10;

    public ResizableRectangle()
    {
        this.SetStyle(ControlStyles.ResizeRedraw, true);
        this.DoubleBuffered = true;
        this.BackColor = Color.LightBlue;
        this.Cursor = Cursors.Default;
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            resizeDirection = GetResizeDirection(e.Location);
            if (resizeDirection != ResizeDirection.None)
            {
                isResizing = true;
                clickOffset = e.Location;
                this.Cursor = GetCursor(resizeDirection);
            }
            else
            {
                isDragging = true;
                clickOffset = e.Location;
                this.Cursor = Cursors.SizeAll;
            }
        }
        base.OnMouseDown(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (isDragging)
        {
            this.Left += e.X - clickOffset.X;
            this.Top += e.Y - clickOffset.Y;
        }
        else if (isResizing)
        {
            ResizeRectangle(e.Location);
        }
        else
        {
            resizeDirection = GetResizeDirection(e.Location);
            this.Cursor = GetCursor(resizeDirection);
        }
        base.OnMouseMove(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            isDragging = false;
            isResizing = false;
            this.Cursor = Cursors.Default;
        }
        base.OnMouseUp(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        // Draw resize handles
        DrawResizeHandles(e.Graphics);
    }

    private enum ResizeDirection
    {
        None,
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left
    }

    private ResizeDirection GetResizeDirection(Point location)
    {
        if (new Rectangle(0, 0, handleSize, handleSize).Contains(location)) return ResizeDirection.TopLeft;
        if (new Rectangle(handleSize, 0, this.Width - 2 * handleSize, handleSize).Contains(location)) return ResizeDirection.Top;
        if (new Rectangle(this.Width - handleSize, 0, handleSize, handleSize).Contains(location)) return ResizeDirection.TopRight;
        if (new Rectangle(this.Width - handleSize, handleSize, handleSize, this.Height - 2 * handleSize).Contains(location)) return ResizeDirection.Right;
        if (new Rectangle(this.Width - handleSize, this.Height - handleSize, handleSize, handleSize).Contains(location)) return ResizeDirection.BottomRight;
        if (new Rectangle(handleSize, this.Height - handleSize, this.Width - 2 * handleSize, handleSize).Contains(location)) return ResizeDirection.Bottom;
        if (new Rectangle(0, this.Height - handleSize, handleSize, handleSize).Contains(location)) return ResizeDirection.BottomLeft;
        if (new Rectangle(0, handleSize, handleSize, this.Height - 2 * handleSize).Contains(location)) return ResizeDirection.Left;
        return ResizeDirection.None;
    }

    private void ResizeRectangle(Point location)
    {
        int dx = location.X - clickOffset.X;
        int dy = location.Y - clickOffset.Y;

        switch (resizeDirection)
        {
            case ResizeDirection.TopLeft:
                this.Left += dx;
                this.Top += dy;
                this.Width -= dx;
                this.Height -= dy;
                break;
            case ResizeDirection.Top:
                this.Top += dy;
                this.Height -= dy;
                break;
            case ResizeDirection.TopRight:
                this.Top += dy;
                this.Width += dx;
                this.Height -= dy;
                break;
            case ResizeDirection.Right:
                this.Width += dx;
                break;
            case ResizeDirection.BottomRight:
                this.Width += dx;
                this.Height += dy;
                break;
            case ResizeDirection.Bottom:
                this.Height += dy;
                break;
            case ResizeDirection.BottomLeft:
                this.Left += dx;
                this.Width -= dx;
                this.Height += dy;
                break;
            case ResizeDirection.Left:
                this.Left += dx;
                this.Width -= dx;
                break;
        }
        clickOffset = location;
    }

    private Cursor GetCursor(ResizeDirection direction)
    {
        switch (direction)
        {
            case ResizeDirection.TopLeft:
            case ResizeDirection.BottomRight:
                return Cursors.SizeNWSE;
            case ResizeDirection.TopRight:
            case ResizeDirection.BottomLeft:
                return Cursors.SizeNESW;
            case ResizeDirection.Top:
            case ResizeDirection.Bottom:
                return Cursors.SizeNS;
            case ResizeDirection.Left:
            case ResizeDirection.Right:
                return Cursors.SizeWE;
            default:
                return Cursors.Default;
        }
    }

    private void DrawResizeHandles(Graphics g)
    {
        Brush brush = Brushes.DarkBlue;

        // Top-left
        g.FillRectangle(brush, 0, 0, handleSize, handleSize);
        // Top
        g.FillRectangle(brush, handleSize, 0, this.Width - 2 * handleSize, handleSize);
        // Top-right
        g.FillRectangle(brush, this.Width - handleSize, 0, handleSize, handleSize);
        // Right
        g.FillRectangle(brush, this.Width - handleSize, handleSize, handleSize, this.Height - 2 * handleSize);
        // Bottom-right
        g.FillRectangle(brush, this.Width - handleSize, this.Height - handleSize, handleSize, handleSize);
        // Bottom
        g.FillRectangle(brush, handleSize, this.Height - handleSize, this.Width - 2 * handleSize, handleSize);
        // Bottom-left
        g.FillRectangle(brush, 0, this.Height - handleSize, handleSize, handleSize);
        // Left
        g.FillRectangle(brush, 0, handleSize, handleSize, this.Height - 2 * handleSize);
    }
}
