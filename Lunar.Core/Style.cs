using Lunar.Core;
using SkiaSharp;
using System.Reflection;
namespace Lunar.Native
{
    public struct Style
    {
        private readonly Dictionary<string, object?> _properties = new Dictionary<string, object?>();
        private readonly Dictionary<string, Style> _states = new Dictionary<string, Style>();
        public string? ClassName = null;
        public string? Target = null;
        public Style()
        {
        }

        public void Set(string prop, object? value)
        {
            _properties[prop] = value;
        }

        public object? Get(string prop)
        {
            if(_properties.TryGetValue(prop, out var result))
                return result;
            return null;
        }

        public void AddState(string state, Style style)
        {
            var val = style;
            val.Target = Target;
            val.ClassName = ClassName;
            _states[state] = val;
        }
        public Style? GetStateOrNull(string state)
        {
            if (_states.TryGetValue(state, out var result))
                return result;
            return null;
        }
        public bool HasState(string state)
        {
            return _states.ContainsKey(state);
        }
        
        public void Apply(in Control control)
        {
            if (Target != null)
                if (control.GetType().Name != Target)
                    return;
            if (ClassName != null)
                if (!control.ClassList.Contains(ClassName))
                    return;
            
            if(control.State != "" && HasState(control.State))
                GetStateOrNull(control.State)?.Apply(control);
            
            else
            {
                apply(control, control.GetType());
            }
        }

        private void apply(in Control control, Type type)
        {
            foreach (var pair in _properties)
            {
                var properties = type.GetProperties();
                foreach (var prop in properties)
                {
                    if (prop.Name != pair.Key)
                        continue;
                    // Found a match. Set it's value
                    prop.SetValue(control, pair.Value);
                }
            }
        }
    }
}
