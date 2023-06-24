using SkiaSharp;
namespace Lunar
{
    public class SolidFill : Fill
    {
        public SKColor Color { get; set; } = SKColors.Black;
        private readonly SKPaint paint;
        
        public SolidFill(SKColor color)
        {
            Color = color;
            paint = new SKPaint()
            {
                Color = color,
                IsAntialias = true
            };
        }
        public override void OnDraw(SKCanvas canvas, float x, float y, float width, float height, float borderRadius = 0)
        {
            paint.Color = Color;
            canvas.DrawRoundRect(x,y,width,height, borderRadius, borderRadius, paint);
        }
    }
}
