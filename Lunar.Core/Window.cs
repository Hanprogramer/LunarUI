using Lunar.Core;
using System.Numerics;
namespace Lunar
{
    public abstract class Window
    {
        public IApplication Application { get; set; }
        public Control Control { get; set; } = new Control();
        public int Width { get; set; }
        public int Height { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsInitialized { get; }
        public bool IsClosing { get; }
        public bool IsRunning { get; set; }

        public abstract void DoUpdate();
        public abstract void Initialize();
        public abstract void Close();

        public delegate void WindowEvent();
        public event WindowEvent Ready;
        public event WindowEvent Closing;
        public void OnReady()
        {
            Ready?.Invoke();
        }

        public void OnClosing()
        {
            Closing?.Invoke();
        }
    }
}
