using Silk.NET.Maths;
namespace Lunar.Native
{
    public class Vector2
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

        public static float DotProduct(Vector2 left, Vector2 right)
        {
            return left.X * right.X + left.Y * right.Y;
        }

        public void Set(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
