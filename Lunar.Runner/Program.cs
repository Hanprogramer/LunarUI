// See https://aka.ms/new-console-template for more information

using Lunar;
using Lunar.Scripting;
var app = new Application(args[0], "CoreLauncher");
app.AddWindowFeature<XmlScriptingFeature>();
app.AddApplicationFeature(new V8ScriptingFeature());
app.CreateRootWindow();
app.Run();