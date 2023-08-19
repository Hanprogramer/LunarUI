// See https://aka.ms/new-console-template for more information

using Lunar;
using Lunar.Scripting;
var projectDir = @"D:\Programming\C#\Lunar\CoreLauncher";
Console.WriteLine("Running test project...");

var app = new Application(projectDir, "CoreLauncher");
app.AddWindowFeature<XmlScriptingFeature>();
app.AddFeature(new V8ScriptingFeature());
app.CreateRootWindow();
app.Run();