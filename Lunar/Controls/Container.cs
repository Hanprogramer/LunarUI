using Lunar.Core;
using SkiaSharp;
namespace Lunar.Controls
{
    /// <summary>
    /// Base class for container
    /// </summary>
    public class Container : Control
    {
        public virtual void UpdateChildren(double dt) { }
        public virtual void RenderChildren(SKCanvas canvas) { }

        public override void OnUpdate(double dt)
        {
            base.OnUpdate(dt);
            UpdateChildren(dt);
        }
        public override void OnRender(SKCanvas canvas)
        {
            base.OnRender(canvas);
            RenderChildren(canvas);
        }
    }
}
