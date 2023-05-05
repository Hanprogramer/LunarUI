using Lunar.Core;
using SkiaSharp;
namespace Lunar.Controls
{
    public class SingleChildContainer : Container
    {
        public Control Child { get; set; }
        public override void UpdateChildren(double dt)
        {
            base.UpdateChildren(dt);
            Child.OnUpdate(dt);
        }
        public override void RenderChildren(SKCanvas canvas)
        {
            base.RenderChildren(canvas);
            Child.OnRender(canvas);
        }
    }
}
