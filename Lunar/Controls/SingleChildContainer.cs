using Lunar.Core;
using Lunar.Native;
using SkiaSharp;
namespace Lunar.Controls
{
    public class SingleChildContainer : Container
    {
        public Control? Child { get; set; }
        public override void OnUpdate(double dt)
        {
            base.OnUpdate(dt);
            Child?.OnUpdate(dt);
        }
        public override void OnRender(SKCanvas canvas)
        {
            base.OnRender(canvas);
            Child?.OnRender(canvas);
        }
        public override void OnResized(Vector2 newSize)
        {
            base.OnResized(newSize);
            Child?.OnResized(newSize);
        }
        public override void ClearChildren()
        {
            Child = null;
        }
    }
}
