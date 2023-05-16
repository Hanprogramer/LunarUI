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
}
