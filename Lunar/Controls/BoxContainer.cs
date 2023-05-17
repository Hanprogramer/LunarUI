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
            var maxWeight = Children.Sum(child => child.Weight);
            switch (Orientation)
            {
                case Orientation.Horizontal:
                {
                    // Main Axis Alignment
                    if (MainAxisAlignment == AxisAlignment.Fill)
                    {
                        float x = 0;
                        foreach (var child in Children)
                        {
                            child.Position.X = Position.X + x;
                            child.Size.X = Size.X * (child.Weight / maxWeight);
                            x += child.Size.X;
                        }
                    }
                    else
                    {
                        var totalWidth = Children.Sum(child => child.Size.X);
                        var x = MainAxisAlignment switch
                        {
                            AxisAlignment.Begin => 0,
                            AxisAlignment.End => Size.X - totalWidth,
                            AxisAlignment.Center => Size.X / 2.0f - totalWidth / 2.0f
                        };
                        foreach (var child in Children)
                        {
                            child.Position.X = x;
                            x += child.Size.X;
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
                        var totalHeight = Children.Sum(child => child.Size.Y);
                        var y = CrossAxisAlignment switch
                        {
                            AxisAlignment.Begin => 0,
                            AxisAlignment.End => Size.Y - totalHeight,
                            AxisAlignment.Center => Size.Y / 2.0f - totalHeight / 2.0f
                        };
                        foreach (var child in Children)
                        {
                            child.Position.Y = y;
                            y += child.Size.Y;
                        }
                    }
                    break;
                }
                case Orientation.Vertical:
                {
                    // Main Axis Alignment
                    if (MainAxisAlignment == AxisAlignment.Fill)
                    {
                        float y = 0;
                        foreach (var child in Children)
                        {
                            child.Position.Y = Position.Y + y;
                            child.Size.Y = Size.Y * (child.Weight / maxWeight);
                            y += child.Size.Y;
                        }
                    }
                    else
                    {
                        var totalHeight = Children.Sum(child => child.Size.Y);
                        var y = MainAxisAlignment switch
                        {
                            AxisAlignment.Begin => 0,
                            AxisAlignment.End => Size.Y - totalHeight,
                            AxisAlignment.Center => Size.Y / 2.0f - totalHeight / 2.0f
                        };
                        foreach (var child in Children)
                        {
                            child.Position.Y = y;
                            y += child.Size.Y;
                        }
                    }
                    // Cross Axis Alignment
                    if (CrossAxisAlignment == AxisAlignment.Fill)
                    {
                        foreach (var child in Children)
                        {
                            child.Size.X = Size.X;
                            child.Position.X = Position.X;
                        }
                    }
                    else
                    {
                        var totalWidth = Children.Sum(child => child.Size.X);
                        var x = CrossAxisAlignment switch
                        {
                            AxisAlignment.Begin => 0,
                            AxisAlignment.End => Size.X - totalWidth,
                            AxisAlignment.Center => Size.X / 2.0f - totalWidth / 2.0f
                        };
                        foreach (var child in Children)
                        {
                            child.Position.X = x;
                            x += child.Size.X;
                        }
                    }
                    break;
                }
            }
            base.OnResized(newSize);
        }
        public BoxContainer(Window window) : base(window)
        {
        }
    }
}
