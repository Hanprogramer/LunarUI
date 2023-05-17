using Lunar.Native;
namespace Lunar
{
    public interface IApplication
    {
        public void AddFeature(ApplicationFeature feature);
        public void RemoveFeature(ApplicationFeature feature);
        public IControlRegistry GetControlRegistry();
        
        public Theme Theme { get; set; }
    }
}
