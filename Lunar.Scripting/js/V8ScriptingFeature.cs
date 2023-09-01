using Lunar.Scripting.js;
namespace Lunar.Scripting;
using Microsoft.ClearScript.V8;
public class V8ScriptingFeature : ApplicationFeature
{
    public V8ScriptEngine Engine;
    public V8ScriptingFeature()
    {
        Engine = new V8ScriptEngine();
    }
    public override void OnApplicationReady(IApplication application)
    {
        base.OnApplicationReady(application);
        Engine.AddHostType("console", typeof(V8Console));
        Engine.AddHostObject("lunar", new V8Lunar(this));
    }
    public override void OnWindowReady(Window window)
    {
        base.OnWindowReady(window);
    }
}
