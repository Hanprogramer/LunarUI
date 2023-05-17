using Lunar.Native;
using SkiaSharp;
namespace Lunar
{
    public abstract class StyleBoxFill
    {
        public virtual void OnDraw(SKCanvas canvas, Vector2 position, Vector2 size){}
        public virtual void OnUpdate(double dt){}
    }

    public class StyleBoxFillSolid : StyleBoxFill
    {
        public SKColor Color { get; set; } = SKColors.Black;
        private readonly SKPaint paint = new SKPaint();
        
        public StyleBoxFillSolid(SKColor color)
        {
            Color = color;
        }
        public override void OnDraw(SKCanvas canvas, Vector2 position, Vector2 size)
        {
            paint.Color = Color;
            canvas.DrawRect(position.X, position.Y, size.X, size.Y, paint);
        }
    }
}
