using Lunar.Controls;
using Lunar.Core;
using Lunar.Native;
using Lunar.Scripting;
namespace Lunar.Controls
{
    public enum ComposedControlPropertyType
    {
        String, Number, Bool
    }
    
    public class ComposedControlProperty
    {
        public string Name;
        public object? Value, DefaultValue;
        public ComposedControlPropertyType Type;
        public bool Required = false;

        public ComposedControlProperty(string name, ComposedControlPropertyType type, string? defaultValue=null, bool required = false)
        {
            Name = name;
            Type = type;
            DefaultValue = defaultValue;
            if (defaultValue != null)
                Value = defaultValue;
            Required = required;
            // TODO: cast default value automatically to required type
        }
    }
    
    public class ComposedControl : StackContainer, IComposedControl
    {
        public Dictionary<string, ComposedControlProperty> Properties;
        public List<Style> Styles;

        public ComposedControlScript? Script;
        
        public ComposedControl(Window window) : base(window)
        {
            Properties = new();
            Styles = new List<Style>();
        }

        public override object? GetProperty(string name)
        {
            return Properties.TryGetValue(name, out ComposedControlProperty value) ? value.Value : null;
        }
        public void SetProperty(string name, object? value)
        {
            if (Properties.TryGetValue(name, out ComposedControlProperty composedControlProperty))
            {
                composedControlProperty.Value = value;
            }
            else
            {
                throw new Exception($"Property not found: '{name}'");
            }
        }
        public void ApplyStyles(Control control)
        {
            foreach (var style in Styles)
            {
                style.Apply(control);
            }
        }
    }
}
