using Lunar.Core;
using Silk.NET.Input;
using Lunar.Native;
namespace Lunar.Controls
{
    /// <summary>
    /// BoxContainer
    /// a container that will try to fit every child it has to its size
    /// if set to stretch the proportion of the children is defined by the child's Weight attribute
    /// if not set to stretch will set it based on the children's Size attribute
    /// </summary>
    public class BoxContainer : MultiChildContainer
    {
        public Orientation Orientation { get; set; } = Orientation.Horizontal;
        public AxisAlignment MainAxisAlignment { get; set; } = AxisAlignment.Fill;
        public AxisAlignment CrossAxisAlignment { get; set; } = AxisAlignment.Fill;
        public override void OnResized(Vector2 newSize)
        {
            if (Orientation == Orientation.Horizontal)
            {
                // Main Axis Alignment
                if (MainAxisAlignment == AxisAlignment.Fill)
                {
                    float x = 0;
                    float w = Size.X - Children.Sum(child => child.Weight == 0 ? child.Size.X : 0);
                    var totalWeight = Children.Sum(
                        child => child.Weight);
                    foreach (var child in Children)
                    {
                        child.Position.X = Position.X + x + child.Padding.Left + child.Margin.Left;
                        if (child.Weight == 0)
                            child.Size.X = child.MinSize?.X ?? 0;
                        else
                            child.Size.X = w * (child.Weight / totalWeight) - child.Padding.Width - child.Margin.Width;
                        x += child.MeasuredSize.X;
                    }
                }
                else
                {
                    var totalWidth = Children.Sum(child => child.MeasuredSize.X);
                    var x = MainAxisAlignment switch
                    {
                        AxisAlignment.Begin => 0,
                        AxisAlignment.End => Size.X - totalWidth,
                        AxisAlignment.Center => (Size.X - totalWidth) / 2.0f
                    };
                    foreach (var child in Children)
                    {
                        child.Position.X = Position.X + x + child.Margin.Left + child.Padding.Left;
                        child.Size.X = child.MinSize.X;
                        x += child.MeasuredSize.X;
                    }
                }
                // Cross Axis Alignment
                if (CrossAxisAlignment == AxisAlignment.Fill)
                {
                    foreach (var child in Children)
                    {
                        child.Size.Y = Size.Y;
                        child.Position.Y = Position.Y;
                    }
                }
                else
                {
                    var maxHeight = Children.Max(child => child.MeasuredSize.Y);
                    var y = CrossAxisAlignment switch
                    {
                        AxisAlignment.Begin => 0,
                        AxisAlignment.End => Size.Y - maxHeight,
                        AxisAlignment.Center => (Size.Y / 2.0f - maxHeight / 2.0f)
                    };
                    foreach (var child in Children)
                    {
                        child.Position.Y = Position.Y + y + child.Padding.Top + child.Margin.Top;
                    }
                }
            }
            else if (Orientation == Orientation.Vertical)
            {
                // Main Axis Alignment
                if (MainAxisAlignment == AxisAlignment.Fill)
                {
                    float y = 0;
                    float h = Size.Y - Children.Sum(child => child.Weight == 0 ? child.Size.Y : 0);
                    var totalWeight = Children.Sum(
                        child => child.Weight);
                    foreach (var child in Children)
                    {
                        child.Position.Y = Position.Y + y + child.Padding.Top + child.Margin.Top;
                        if (child.Weight == 0)
                            child.Size.Y = child.MinSize?.Y ?? 0;
                        else
                            child.Size.Y = h * (child.Weight / totalWeight) - child.Padding.Height - child.Margin.Height;
                        y += child.MeasuredSize.Y;
                    }
                }
                else
                {
                    var totalHeight = Children.Sum(child => child.MeasuredSize.Y);
                    var y = MainAxisAlignment switch
                    {
                        AxisAlignment.Begin => 0,
                        AxisAlignment.End => Size.Y - totalHeight,
                        AxisAlignment.Center => (Size.Y - totalHeight) / 2.0f
                    };
                    foreach (var child in Children)
                    {
                        child.Position.Y = Position.Y + y + child.Margin.Top + child.Padding.Top;
                        child.Size.Y = child.MinSize.Y;
                        y += child.MeasuredSize.Y;
                    }
                }
                // Cross Axis Alignment
                if (CrossAxisAlignment == AxisAlignment.Fill)
                {
                    foreach (var child in Children)
                    {
                        child.Size.X = Size.X - child.Padding.Width - child.Margin.Width;
                        child.Position.X = Position.X + child.Padding.Left + child.Margin.Left;
                    }
                }
                else
                {
                    var maxWidth = Children.Max(child => child.MeasuredSize.X);
                    var x = CrossAxisAlignment switch
                    {
                        AxisAlignment.Begin => 0,
                        AxisAlignment.End => Size.X - maxWidth,
                        AxisAlignment.Center => (Size.X - maxWidth) / 2.0f
                    };
                    foreach (var child in Children)
                    {
                        child.Position.X = Position.X + x + child.Padding.Left + child.Margin.Left;
                    }
                }
            }

            base.OnResized(newSize);
        }
        public BoxContainer(Window window) : base(window)
        {
        }
    }
}
