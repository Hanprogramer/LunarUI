using Lunar.Core;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
namespace Lunar.Scripting.js
{
    public class V8ComposedControlScript : ComposedControlScript
    {
        private readonly V8ScriptingFeature _v8;
        private V8Script? _script = null;
        private DocumentInfo _info;
        public V8ComposedControlScript(V8ScriptingFeature v8, Control control, string source, string className) : base(control, source, className)
        {
            this._v8 = v8;
        }
        
        public override void Load(string path, string content)
        {
            _script = _v8.Engine.Compile(content);
            _info = new DocumentInfo(path);
            _v8.Engine.Execute(_script);
        }
        
        public override void Dispose()
        {
            _script?.Dispose();
        }
        
        public override void Call(string funcName, object[]? args=null)
        {
            _v8.Engine.Invoke(funcName, args);
        }
        
        public override void GetValue(string name)
        {
            _v8.Engine.Global.GetProperty(name);
        }
        
        public override void SetValue(string name, object value)
        {
            _v8.Engine.Global.SetProperty(name, value);
        }
    }
}
