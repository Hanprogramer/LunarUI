using Lunar.Native;
using SkiaSharp;
namespace Lunar
{
    public struct StyleBox
    {
        public Spacing Padding;
        public Spacing Margin;
        public float? TextSize;

        public StyleBoxFill? Background;
        public SKColor? FontColor;
        
        public StyleBox()
        {
        }
    }
}
