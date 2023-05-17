namespace Lunar
{
    public abstract class NativeContext
    {
        public IApplication Application { get; set; }
        public abstract void OnAddFeatures(IApplication application);
        public abstract Window CreateWindow(IApplication application, string path, string title, int w = 800, int h = 600, bool isMultiThreaded = false);
        public abstract double GetScaling();
    }
}
