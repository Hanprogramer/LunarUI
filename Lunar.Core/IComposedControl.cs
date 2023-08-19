using Lunar.Core;
namespace Lunar.Native
{
    public interface IComposedControl
    {
        public object? GetProperty(string name);
        public void SetProperty(string name, object? value);
        
        public void ApplyStyles(Control control);
    }
}
