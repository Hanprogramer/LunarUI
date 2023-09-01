using Lunar.Core;
namespace Lunar.Scripting
{
    /// <summary>
    /// Base class for scripting scripts
    /// </summary>
    public abstract class ComposedControlScript
    {
        public Control Control;
        public string Source { get; set; }
        public string ClassName { get; set; }
        
        public abstract void Load(string path, string content);
        public abstract void Dispose();
        
        public abstract void Call(string funcName, object[]? args=null);
        
        public abstract void GetValue(string name);
        public abstract void SetValue(string name, object value);
        
        protected ComposedControlScript(Control control, string source, string className)
        {
            Control = control;
            Source = source;
            ClassName = className;
        }
    }
}
