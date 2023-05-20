namespace Lunar.Native
{public class Rect
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        
        public Vector2 Size { get => new Vector2(Width, Height); }

        public Rect(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rect(Vector2 pos, Vector2 size)
        {
            X = pos.X;
            Y = pos.Y;
            Width = size.X;
            Height = size.Y;
        }
        public Rect()
        {
            X = 0;
            Y = 0;
            Width = 0;
            Height = 0;
        }

        public float Area
        {
            get { return Width * Height; }
        }

        public static Rect operator +(Rect rect, Vector2 offset)
        {
            return new Rect(rect.X + offset.X, rect.Y + offset.Y, rect.Width, rect.Height);
        }

        public static Rect operator -(Rect rect, Vector2 offset)
        {
            return new Rect(rect.X - offset.X, rect.Y - offset.Y, rect.Width, rect.Height);
        }

        public static Rect operator *(Rect rect, float scalar)
        {
            return new Rect(rect.X, rect.Y, rect.Width * scalar, rect.Height * scalar);
        }

        public static Rect operator *(float scalar, Rect rect)
        {
            return rect * scalar;
        }
    }
}
