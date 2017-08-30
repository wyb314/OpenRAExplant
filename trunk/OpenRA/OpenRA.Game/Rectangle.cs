using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenRA
{
    public struct Rectangle
    {

        public static readonly Rectangle Empty = new Rectangle();

        private int x;
        private int y;
        private int width;
        private int height;

        public int X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        public int Right
        {
            get
            {
                return X + Width;
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        public int Bottom
        {
            get
            {
                return Y + Height;
            }
        }

        public int Top
        {
            get
            {
                return Y;
            }
        }

        public int Left
        {
            get
            {
                return X;
            }
        }

        public Size Size
        {
            get
            {
                return new Size(Width, Height);
            }
            set
            {
                this.Width = value.Width;
                this.Height = value.Height;
            }
        }

        public Rectangle(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public bool Contains(int x, int y)
        {
            return this.X <= x &&
            x < this.X + this.Width &&
            this.Y <= y &&
            y < this.Y + this.Height;
        }

        public bool IntersectsWith(Rectangle rect)
        {
            return (rect.X < this.X + this.Width) &&
            (this.X < (rect.X + rect.Width)) &&
            (rect.Y < this.Y + this.Height) &&
            (this.Y < rect.Y + rect.Height);
        }

        public bool Contains(Rectangle rect)
        {
            return (this.X <= rect.X) &&
            ((rect.X + rect.Width) <= (this.X + this.Width)) &&
            (this.Y <= rect.Y) &&
            ((rect.Y + rect.Height) <= (this.Y + this.Height));
        }

        public static Rectangle FromLTRB(int left, int top, int right, int bottom)
        {
            return new Rectangle(left,
                                 top,
                                 right - left,
                                 bottom - top);
        }

        public static bool operator !=(Rectangle left, Rectangle right)
        {
            return !(left == right);
        }

        public static bool operator ==(Rectangle left, Rectangle right)
        {
            return (left.X == right.X
                    && left.Y == right.Y
                    && left.Width == right.Width
                    && left.Height == right.Height);
        }

        public void Offset(int x, int y)
        {
            this.X += x;
            this.Y += y;
        }
    }
}
