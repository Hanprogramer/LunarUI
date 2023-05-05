namespace Lunar
{
    public abstract class ApplicationFeature : IFeature
    {
        public virtual void OnApplicationRender() { }
        public virtual void OnApplicationLoop(double dt) { }
        public virtual void OnBeforeApplicationReady() { }
        public virtual void OnApplicationReady() { }

        public virtual void OnAddWindow(Window window) { }
        public virtual void OnWindowReady(Window window) { }
        public virtual void OnRemoveWindow(Window window) { }
    }
}
