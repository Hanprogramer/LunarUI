namespace Lunar
{
    public abstract class ApplicationFeature : IFeature
    {
        public virtual void OnApplicationRender(IApplication application) { }
        public virtual void OnApplicationLoop(double dt) { }
        public virtual void OnBeforeApplicationReady(IApplication application) { }
        public virtual void OnApplicationReady(IApplication application) { }

        public virtual void OnAddWindow(Window window) { }
        public virtual void OnWindowReady(Window window) { }
        public virtual void OnRemoveWindow(Window window) { }
    }
}
