using Lunar.Native;
namespace Lunar.Controls
{
    public class StackContainer : MultiChildContainer
    {
        
        public override void OnResized(Vector2 newSize)
        {
            base.OnResized(newSize);
            foreach (var child in Children)
            {
                child.MeasuredSize = Size;
                child.Position = new Vector2(
                    Position.X + Margin.Left + Padding.Left + child.Margin.Left + child.Padding.Left,
                    Position.Y + Margin.Top + Padding.Top + child.Margin.Top + child.Padding.Top);
                
                MinSize = new Vector2(Math.Max(child.MeasuredMinSize.X, MinSize.X),Math.Max(child.MeasuredMinSize.Y, MinSize.Y));
            }
        }
        public StackContainer(Window window) : base(window)
        {
        }
    }
}
