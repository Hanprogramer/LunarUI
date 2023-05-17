using Lunar.Core;
using Lunar.Native;
namespace Lunar
{
    public abstract class Window
    {
        public IApplication Application { get; set; }
        /// <summary>
        /// Window's Current Filepath
        /// </summary>
        public LunarURI Path { get; set; } = new LunarURI("/"); //TODO: implement setter to load specific path and replace with LunarURI
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
        public List<WindowFeature> Features = new();
        public void OnReady()
        {
            Ready?.Invoke();
        }

        public void OnClosing()
        {
            Closing?.Invoke();
        }

        public void AddFeature(WindowFeature feature)
        {
            Features.Add(feature);
        }
        public void RemoveFeature(WindowFeature feature)
        {
            Features.Remove(feature);
        }

        public void RemoveFeature<T>() where T : WindowFeature
        {
            for (var i = 0; i < Features.Count; i++)
            {
                if (Features[i] is not T)
                    continue;
                Features.RemoveAt(i);
                break;
            }
        }

        public T GetFeature<T>() where T : WindowFeature
        {
            foreach (var feature in Features)
            {
                if (feature is T t)
                    return t;
            }
            throw new Exception("Feature not found!");
        }
    }
}
