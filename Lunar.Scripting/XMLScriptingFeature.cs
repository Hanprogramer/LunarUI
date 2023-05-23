using Lunar.Controls;
using Lunar.Core;
using Lunar.Native;
using SkiaSharp;
using System.Xml;
namespace Lunar.Scripting
{
    public class XmlScriptingFeature : WindowFeature
    {
        private XmlDocument doc;
        private ControlRegistry _registry;
        private readonly FileSystemWatcher _watcher;
        private bool isParsing = false;

        public XmlScriptingFeature(Window window) : base(window)
        {
            _watcher = new FileSystemWatcher();
        }
        public override void OnWindowReady()
        {
            base.OnWindowReady();

            _registry = (ControlRegistry)Window.Application.GetControlRegistry();
            var path = Window.Path.ActualPath;
            var lastRead = DateTime.MinValue;
            _watcher.Filter = "*.xml";
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.Path = Path.GetDirectoryName(path);
            _watcher.Changed += async (sender, args) =>
            {
                var now = DateTime.Now;
                var lastWriteTime = File.GetLastWriteTime(args.FullPath);
                if (now == lastWriteTime)
                    return;
                if (lastWriteTime != lastRead)
                {
                    _watcher.EnableRaisingEvents = false;
                    Console.WriteLine("changed " + args.FullPath);
                    if (!isParsing)
                        await ParseFile(path);
                    await Task.Delay(100);
                    _watcher.EnableRaisingEvents = true;
                }
            };
            _watcher.IncludeSubdirectories = true;
            _watcher.EnableRaisingEvents = true;
            ParseFile(path);
        }

        /// <summary>
        /// Parse a source XML file
        /// </summary>
        /// <param name="path">Absolute path to the file</param>
        private async Task ParseFile(string path)
        {
            isParsing = true;
            if (!path.EndsWith(".xml"))
            {
                // Try to see if it's a folder
                if (Directory.Exists(path))
                {
                    // Check if there's an index.xml
                    var p = Path.Join(path, "index.xml");
                    if (File.Exists(p))
                    {
                        path = p;
                    }
                    else return;
                }
                else return;
            }
            doc = new XmlDocument();
            string content;
            await using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(stream))
            {
                content = await sr.ReadToEndAsync();
            }
            if (content != "")
            {
                try
                {
                    doc.LoadXml(content);
                    GenerateTree();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"content: {content}");
                    Console.WriteLine(e.ToString());
                }
                Console.WriteLine("Succesful");
            }
            else
            {
                Console.WriteLine("Failed to read file, retrying");
                await Task.Delay(100); // Add a slight delay to ensure 100% it will work
                await ParseFile(path);
                return;
            }
            isParsing = false;
            await Task.Delay(100);
            Window.Control.Refresh();
            Window.Control.Refresh();
        }

        /// <summary>
        /// Generate the Control tree from our document
        /// </summary>
        private void GenerateTree()
        {
            var cont = Window.Control as StackContainer;
            cont.ClearChildren();
            Window.Styles.Clear();
            foreach (XmlNode node in doc.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Element || node.Name != "Template")
                    continue;
                foreach (XmlNode n in node.ChildNodes)
                {
                    switch (n)
                    {
                        case { NodeType: XmlNodeType.Element, Name: "Content" }:
                        {
                            foreach (XmlNode nn in n.ChildNodes)
                            {
                                ParseContentNode(nn, Window.Control);
                            }
                            break;
                        }
                        case { NodeType: XmlNodeType.Element, Name: "Styles" }:
                        {
                            foreach (XmlNode nn in n.ChildNodes)
                            {
                                ParseStyleNodes(nn, in Window.Styles);
                            }
                            break;
                        }
                    }
                }
            }
            //TODO: Parse Styles and Scripts as well
        }
        /// <summary>
        /// Parse the node tree to a tree filled with controls
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        private void ParseContentNode(XmlNode node, in Control parent)
        {
            if (node.NodeType != XmlNodeType.Element)
                return;
            if (!_registry.HasControl(node.Name))
                return;
            if (parent is not MultiChildContainer m)
                return;
            var control = _registry.GetControl(node.Name);
            var constructor = control.GetConstructor(new Type[]
            {
                typeof(Window)
            });
            var instance = (Control)constructor.Invoke(new object[]
            {
                Window
            });
            m.AddChild(instance);

            // Set the attributes
            if (node.Attributes != null)
            {
                foreach (XmlAttribute attr in node.Attributes)
                {
                    if (attr.Name == "Class")
                        foreach (var cls in attr.Value.Split(" "))
                        {
                            instance.ClassList.Add(cls);
                        }
                    var prop = control.GetProperty(attr.Name);
                    if (prop == null || attr.Value == "")
                        continue;
                    if (prop.PropertyType.IsEnum)
                    {
                        if (Enum.TryParse(prop.PropertyType, attr.Value, out var result))
                            prop.SetValue(instance, result);
                    }
                    else if (prop.PropertyType == typeof(float))
                    {
                        if (float.TryParse(attr.Value, out var f))
                            prop.SetValue(instance, f);
                    }
                    else if (prop.PropertyType == typeof(LunarURI))
                    {
                        prop.SetValue(instance, new LunarURI(attr.Value));
                    }
                    else if (prop.PropertyType == typeof(Spacing?))
                    {
                        prop.SetValue(instance, Spacing.Parse(attr.Value));
                    }
                    else
                        prop.SetValue(instance, attr.Value);
                    // TODO: Better error handling
                }
            }

            if (!node.HasChildNodes)
                return;
            if (instance is MultiChildContainer)
            {
                foreach (XmlNode c in node.ChildNodes)
                {
                    ParseContentNode(c, instance);
                }
            }
            else if (instance is SingleChildContainer)
            {
                if (node.ChildNodes.Count == 1)
                    ParseContentNode(node.FirstChild, instance);
                else
                    throw new Exception($"{node.Name} is a SingleChildContainer and can only has one child or no child at all");
            }
        }

        private void ParseStyleNodes(XmlNode node, in List<Style> styles)
        {
            var style = new Style();
            if (node.HasChildNodes)
            {
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    ParseStyleProperties(childNode, ref style);
                }
            }
            foreach (XmlAttribute attr in node.Attributes)
            {
                if (attr.Name == "Class")
                    style.ClassName = attr.Value;
                if (attr.Name == "Target")
                    style.Target = attr.Value;
            }
            styles.Add(style);
        }

        private void ParseStyleProperties(XmlNode node, ref Style style)
        {
            if (node.Name == "States")
            {
                // Parse States
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    string? name = null;
                    if (childNode.Attributes == null)
                        throw new Exception("Node states must have at least a 'Name' attribute");
                    foreach (XmlAttribute attr in childNode.Attributes)
                    {
                        if (attr.Name == "Name")
                            name = attr.Value;
                    }
                    if (name == null)
                        throw new Exception("Node states must have at least a 'Name' attribute");
                    var state = new Style();
                    foreach (XmlNode childNodeChildNode in childNode.ChildNodes)
                    {
                        ParseStyleProperties(childNodeChildNode, ref state);
                    }
                    style.AddState(name, state);
                }
            }
            else
            {
                // Parse Regular Properties
                style.Set(node.Name, node.InnerText);
            }
        }

    }
}
