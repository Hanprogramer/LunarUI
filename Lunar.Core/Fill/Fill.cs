using Lunar.Native;
using SkiaSharp;
namespace Lunar
{
    public abstract class Fill
    {
        public virtual void OnDraw(SKCanvas canvas, float x, float y, float width, float height, float borderRadius = 0){}
        public virtual void OnUpdate(double dt){}
        public virtual void OnResize(Vector2 position, Vector2 newSize){}

        public static implicit operator Fill(SKColor color)
        {
            return new SolidFill(color);
        }
    }
}
