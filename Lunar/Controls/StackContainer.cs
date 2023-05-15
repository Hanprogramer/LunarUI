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
                child.Position = Position;
            }
        }
    }
}
