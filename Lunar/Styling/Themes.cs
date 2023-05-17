using Lunar.Native;
using SkiaSharp;
namespace Lunar
{

    public struct Themes
    {
        public static Theme Dark = new Theme()
        {
            Background = SKColors.Black,
            Foreground = SKColors.White,
            Accent = SKColors.Aquamarine,
            Font = new SKFont()
            {
                Size = 16
            }
        };
        public static Theme Light = new Theme()
        {
            Background = SKColors.White,
            Foreground = SKColors.Black,
            Accent = SKColors.Aquamarine,
            Font = new SKFont()
            {
                Size = 16
            }
        };
    }
}
