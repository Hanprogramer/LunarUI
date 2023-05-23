namespace Lunar.Native
{
    public struct Spacing
    {
        public float Left, Right, Top, Bottom;
        public float Width { get => Left + Right; }
        public float Height { get => Top + Bottom; }
        public Spacing(float spacing)
        {
            Left = spacing;
            Right = spacing;
            Top = spacing;
            Bottom = spacing;
        }
        public Spacing(float left, float right, float top, float bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }
        public static Spacing Parse(string value)
        {
            if (!value.Contains(','))
                return new Spacing(float.Parse(value));
            var list = value.Split(",");
            switch (list.Length)
            {
                case 2:
                    var horizontal = float.Parse(list[0]);
                    var vertical = float.Parse(list[1]);
                    return new Spacing(horizontal,horizontal,vertical,vertical);
                case 4:
                    var left = float.Parse(list[0]);
                    var right = float.Parse(list[1]);
                    var top = float.Parse(list[0]);
                    var bottom = float.Parse(list[1]);
                    return new Spacing(left,right,top, bottom);
                default:
                    throw new Exception("Error parsing spacing variable: " + value);
            }
        }
    }
}
