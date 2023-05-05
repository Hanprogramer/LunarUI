namespace Lunar
{
    public abstract class NativeContext
    {
        public abstract void OnAddFeatures(IApplication application);
        public abstract Window CreateWindow(string title, int w = 800, int h = 600, bool isMultiThreaded = false);
    }
}
