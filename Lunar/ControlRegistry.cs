using Lunar.Core;
using Lunar.Native;
namespace Lunar.Controls
{
    public class ControlRegistry : IControlRegistry
    {
        private Dictionary<string, Type> Controls = new Dictionary<string, Type>();

        public void Register<T>() where T : Control
        {
            Controls.Add(typeof(T).Name, typeof(T));
        }

        public void RegisterDefaults()
        {
            Register<Control>();
            Register<Label>();
            Register<BoxContainer>();
            Register<StackContainer>();
            Register<Image>();
        }
        public Type GetControl(string name)
        {
            if (Controls.TryGetValue(name, out var value))
                return value;
            throw new Exception("Control not found: " + name);
        }

        public bool HasControl(string name)
        {
            return Controls.ContainsKey(name);
        }

        public ControlRegistry()
        {
            RegisterDefaults();
        }
    }
}
