using System;
using System.Drawing;
using System.Windows.Forms;

public class ResizableRectangle : Control
{
    private bool _dragging = false;
    private Point _dragStart;
    private bool _resizing = false;
    private Rectangle _resizeZone;
    private int _resizeMargin = 8;
    private Point _resizeStart;
    private Size _originalSize;
    private Point _originalLocation;
    private ResizeDirection _resizeDirection;

    private enum ResizeDirection
    {
        None, Top, Bottom, Left, Right,
        TopLeft, TopRight, BottomLeft, BottomRight
    }

    public ResizableRectangle()
    {
        this.BackColor = Color.LightBlue;
        this.Size = new Size(100, 100);
        this.MouseDown += OnMouseDown;
        this.MouseMove += OnMouseMove;
        this.MouseUp += OnMouseUp;
        this.Resize += (s, e) => UpdateResizeZone();
    }

    private void UpdateResizeZone()
    {
        // 四辺と四隅をリサイズゾーンとして設定
    }

    private void OnMouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            _resizeDirection = GetResizeDirection(e.Location);

            if (_resizeDirection != ResizeDirection.None)
            {
                _resizing = true;
                _resizeStart = Cursor.Position;
                _originalSize = this.Size;
                _originalLocation = this.Location;
            }
            else
            {
                _dragging = true;
                _dragStart = e.Location;
            }
        }
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (_resizing)
        {
            ResizeControl();
        }
        else if (_dragging)
        {
            MoveControl();
        }
        else
        {
            // カーソルの変更
            this.Cursor = GetCursorForDirection(GetResizeDirection(e.Location));
        }
    }

    private void OnMouseUp(object sender, MouseEventArgs e)
    {
        _dragging = false;
        _resizing = false;
    }

    private ResizeDirection GetResizeDirection(Point location)
    {
        bool left = location.X <= _resizeMargin;
        bool right = location.X >= this.Width - _resizeMargin;
        bool top = location.Y <= _resizeMargin;
        bool bottom = location.Y >= this.Height - _resizeMargin;

        if (top && left) return ResizeDirection.TopLeft;
        if (top && right) return ResizeDirection.TopRight;
        if (bottom && left) return ResizeDirection.BottomLeft;
        if (bottom && right) return ResizeDirection.BottomRight;
        if (top) return ResizeDirection.Top;
        if (bottom) return ResizeDirection.Bottom;
        if (left) return ResizeDirection.Left;
        if (right) return ResizeDirection.Right;

        return ResizeDirection.None;
    }

    private Cursor GetCursorForDirection(ResizeDirection direction)
    {
        return direction switch
        {
            ResizeDirection.TopLeft or ResizeDirection.BottomRight => Cursors.SizeNWSE,
            ResizeDirection.TopRight or ResizeDirection.BottomLeft => Cursors.SizeNESW,
            ResizeDirection.Top or ResizeDirection.Bottom => Cursors.SizeNS,
            ResizeDirection.Left or ResizeDirection.Right => Cursors.SizeWE,
            _ => Cursors.Hand
        };
    }

    private void MoveControl()
    {
        if (Parent == null) return;

        Point newLocation = this.Parent.PointToClient(Cursor.Position);
        newLocation.Offset(-_dragStart.X, -_dragStart.Y);

        // 親の範囲内に制限
        newLocation.X = Math.Max(0, Math.Min(Parent.Width - this.Width, newLocation.X));
        newLocation.Y = Math.Max(0, Math.Min(Parent.Height - this.Height, newLocation.Y));

        this.Location = newLocation;
    }

    private void ResizeControl()
    {
        if (Parent == null) return;

        Point delta = new Point(Cursor.Position.X - _resizeStart.X, Cursor.Position.Y - _resizeStart.Y);
        int newWidth = _originalSize.Width;
        int newHeight = _originalSize.Height;
        int newX = _originalLocation.X;
        int newY = _originalLocation.Y;

        if (_resizeDirection.HasFlag(ResizeDirection.Left))
        {
            newX = Math.Max(0, _originalLocation.X + delta.X);
            newWidth = Math.Max(20, _originalSize.Width - delta.X);
        }
        else if (_resizeDirection.HasFlag(ResizeDirection.Right))
        {
            newWidth = Math.Max(20, _originalSize.Width + delta.X);
        }

        if (_resizeDirection.HasFlag(ResizeDirection.Top))
        {
            newY = Math.Max(0, _originalLocation.Y + delta.Y);
            newHeight = Math.Max(20, _originalSize.Height - delta.Y);
        }
        else if (_resizeDirection.HasFlag(ResizeDirection.Bottom))
        {
            newHeight = Math.Max(20, _originalSize.Height + delta.Y);
        }

        // 親の範囲内に制限
        newWidth = Math.Min(newWidth, Parent.Width - newX);
        newHeight = Math.Min(newHeight, Parent.Height - newY);

        this.Location = new Point(newX, newY);
        this.Size = new Size(newWidth, newHeight);
    }
}
