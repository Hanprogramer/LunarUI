using SkiaSharp;
namespace Lunar.Native
{
    /// <summary>
    /// A class to add extra feature to a window
    /// </summary>
    public class WindowFeature
    {
        protected Window Window { get; set; }
        public WindowFeature(Window window)
        {
            Window = window;
        }
        public virtual void OnWindowReady() { }
        public virtual void OnWindowUpdate(double dt) { }
        public virtual void OnWindowRender(SKCanvas canvas) { }
        public virtual void OnWindowClosing() { }
    }
}
