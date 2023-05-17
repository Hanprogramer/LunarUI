using Lunar.Core;
namespace Lunar.Native
{
    public interface IControlRegistry
    {
        public void Register<T>() where T : Control;
        public void RegisterDefaults();
        public Type GetControl(string name);
    }
}
