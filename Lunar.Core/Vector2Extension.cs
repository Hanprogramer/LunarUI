// using Silk.NET.Maths;

global using System.Numerics;
namespace Lunar.Native
{
    public static class Vector2Extension
    {
        public static Vector2 WithX(this Vector2 a, float x)
        {
            return a with
            {
                X = x
            };
        }
        public static Vector2 WithY(this Vector2 a, float y)
        {
            return a with
            {
                Y = y
            };
        }
    }
}