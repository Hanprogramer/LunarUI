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
        public override void ApplyStyles()
        {
            base.ApplyStyles();
            Child?.ApplyStyles();
        }
        public override void OnMouseMove(ref MouseEvent e, Vector2 position)
        {
            Child?.OnMouseMove(ref e,position);
            base.OnMouseMove(ref e,position);
        }

        public override void OnMouseButton(ref MouseEvent ev, MouseButton button, bool pressed, Vector2 position)
        {
            base.OnMouseButton(ref ev, button, pressed, position);
            Child?.OnMouseButton(ref ev, button, pressed, position);
        }
        public SingleChildContainer(Window window) : base(window)
        {
        }
    }
}
