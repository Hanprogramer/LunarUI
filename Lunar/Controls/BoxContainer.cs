using Lunar.Core;
using Silk.NET.Input;
using Lunar.Native;
namespace Lunar.Controls
{
    public enum Orientation
    {
        Vertical,
        Horizontal
    }
    public enum AxisAlignment
    {
        Begin,
        Center,
        End,
        Stretch
    }
    public class BoxContainer : MultiChildContainer
    {
        public Orientation Orientation { get; set; } = Orientation.Vertical;
        public AxisAlignment MainAxisAlignment { get; set; } = AxisAlignment.Stretch;
        public AxisAlignment CrossAxisAlignment { get; set; } = AxisAlignment.Stretch;
        public override void OnResized(Vector2 newSize)
        {
            base.OnResized(newSize);
            if (Orientation == Orientation.Horizontal)
            {
                if (MainAxisAlignment == AxisAlignment.Stretch)
                {
                    var x = 0;
                    var w = Size.X;
                    foreach (var child in Children)
                    {
                        child.Position.X = 12;
                    }
                }
            }
        }
    }
}
