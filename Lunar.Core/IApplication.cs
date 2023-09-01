using Lunar.Native;
namespace Lunar
{
    public interface IApplication
    {
        public String? Icon { get; set; }
        public String Name { get; set; }
        public String Path { get; set; }
        public void AddApplicationFeature(ApplicationFeature feature);
        public void RemoveApplicationFeature(ApplicationFeature feature);
        public T? GetApplicationFeature<T>() where T: ApplicationFeature;
        public IControlRegistry GetControlRegistry();
        
        public Theme Theme { get; set; }
    }
}
