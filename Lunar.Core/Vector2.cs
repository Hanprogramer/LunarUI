using Silk.NET.Maths;
namespace Lunar.Native
{
    public struct Vector2
    {
        public static readonly Vector2 Zero = new Vector2();
        public static readonly Vector2 One = new Vector2(1);

        public float X { get; set; }
        public float Y { get; set; }
        public Vector2()
        {
            X = 0;
            Y = 0;
        }
        public Vector2(float val)
        {
            X = val;
            Y = val;
        }
        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2 Clone()
        {
            return new Vector2(X, Y);
        }

        public float Magnitude
        {
            get { return (float)Math.Sqrt(X * X + Y * Y); }
        }

        public void Normalize()
        {
            float magnitude = Magnitude;
            X /= magnitude;
            Y /= magnitude;
        }

        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X + right.X, left.Y + right.Y);
        }

        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X - right.X, left.Y - right.Y);
        }

        public static Vector2 operator *(Vector2 vector, float scalar)
        {
            return new Vector2(vector.X * scalar, vector.Y * scalar);
        }

        public static Vector2 operator *(float scalar, Vector2 vector)
        {
            return vector * scalar;
        }

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return !(a == b);
        }
        public static float DotProduct(Vector2 left, Vector2 right)
        {
            return left.X * right.X + left.Y * right.Y;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Vector2 v)
            {
                return X == v.X && Y == v.Y;
            }
            return false;
        }

        public Vector2 WithX(float x)
        {
            return new Vector2(x, Y);
        }
        public Vector2 WithY(float y)
        {
            return new Vector2(X, y);
        }
    }
}
