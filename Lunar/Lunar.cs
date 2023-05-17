namespace Lunar
{
    public class Lunar
    {
        public delegate void LunarEvent();
    }

    public enum TextAlign
    {
        Left,
        Right,
        Center,
        Justify
    }

    public enum ParagraphAlign
    {
        Top,
        Bottom,
        Center
    }
    /// <summary>
    /// Orientation enum
    /// </summary>
    public enum Orientation
    {
        Vertical,
        Horizontal
    }
    /// <summary>
    /// Axis alignment that's used by some Controls
    /// </summary>
    public enum AxisAlignment
    {
        Begin,
        Center,
        End,
        Fill
    }

    public static class FontSize
    {
        public static float Parse(string val)
        {
            switch (val)
            {
                case "Small":
                    return 12;
                case "Medium":
                    return 16;
                case "Large":
                    return 24;
                case "ExtraLarge":
                    return 32;
            }
            return float.Parse(val);
        }
    }
}
