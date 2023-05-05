namespace Lunar.Controls
{
    public class StackContainer : MultiChildContainer
    {
        public override void UpdateChildren(double dt)
        {
            base.UpdateChildren(dt);
            foreach (var child in Children)
            {
                child.MeasuredSize = Size;
                child.Position = Position;
            }
        }
    }
}
