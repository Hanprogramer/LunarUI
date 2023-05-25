using HarfBuzzSharp;
using SkiaSharp;
namespace Lunar.Native
{
    public class LinearGradientFill : Fill
    {
        private List<LinearGradientColor> Values = new List<LinearGradientColor>();
        private SKPaint _paint;
        private SKShader _shader;
        private SKRect rect;
        /// <summary>
        /// The gradient direction in degrees
        /// </summary>
        public int Direction { get; set; } = 0;
        // Used by the xml parser so have to follow this pattern
        public LinearGradientFill(object?[] values)
        {
            foreach (var v in values)
            {
                if (v is LinearGradientColor col)
                    Values.Add(col);
                else
                    throw new Exception("LinearGradientFill can only have values of LinearGradientColor");
            }
            _paint = new SKPaint();
        }


        public override void OnDraw(SKCanvas canvas, float x, float y, float width, float height, float borderRadius = 0)
        {
            base.OnDraw(canvas, x, y, width, height, borderRadius);
            canvas.DrawRoundRect(rect, borderRadius, borderRadius, _paint);
        }
        public override void OnResize(Vector2 position, Vector2 newSize)
        {
            base.OnResize(position, newSize);
            rect = new SKRect(position.X, position.Y, position.X + newSize.X, position.Y + newSize.Y);
            var mx = newSize.X / 2;
            var my = newSize.Y / 2;
            var angle = (Direction * MathF.PI) / 180f;
            var startPoint = new SKPoint(rect.MidX - MathF.Cos(angle) * mx, rect.MidY - MathF.Sin(angle) * my);
            var endPoint = new SKPoint(rect.MidX + MathF.Cos(angle) * mx, rect.MidY + MathF.Sin(angle) * my);
            _shader = SKShader.CreateLinearGradient(
                startPoint,
                endPoint,
                Values.Select(color => color.Color).ToArray(),
                Values.Select(color => color.Position).ToArray(),
                SKShaderTileMode.Clamp);
            _paint.Shader = _shader;
        }
    }

    public struct LinearGradientColor
    {
        public SKColor Color = SKColor.Empty;
        public float Position { get; set; } = 0;
        public LinearGradientColor(SKColor value, float pos = 0)
        {
            Color = value;
            Position = pos;
        }
    }
}
