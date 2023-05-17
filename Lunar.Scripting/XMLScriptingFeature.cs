using Lunar.Controls;
using Lunar.Core;
using Lunar.Native;
using Microsoft.ClearScript.Util.Web;
using System.Xml;
using System.Xml.Linq;
namespace Lunar.Scripting
{
    public class XmlScriptingFeature : WindowFeature
    {
        private XmlDocument doc;
        private ControlRegistry _registry;
        private FileSystemWatcher _watcher;
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
        }

        /// <summary>
        /// Generate the Control tree from our document
        /// </summary>
        private void GenerateTree()
        {
            var cont = Window.Control as StackContainer;
            cont.ClearChildren();
            foreach (XmlNode node in doc.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Element || node.Name != "Template")
                    continue;
                foreach (XmlNode n in node.ChildNodes)
                {
                    if (n is { NodeType: XmlNodeType.Element, Name: "Content" })
                    {
                        ParseNode(n.FirstChild, Window.Control);
                    }
                }
            }
            //TODO: Parse Styles and Scripts as well
        }

        public void ParseNode(XmlNode node, in Control parent)
        {
            switch (node.NodeType)
            {
                case XmlNodeType.None:
                    break;
                case XmlNodeType.Element:
                    if (_registry.HasControl(node.Name))
                    {
                        if (parent is MultiChildContainer m)
                        {
                            var control = _registry.GetControl(node.Name);
                            var constructor = control.GetConstructor(new Type[]
                            {
                            });
                            var instance = (Control)constructor.Invoke(new object[]
                            {
                            });
                            m.AddChild(instance);

                            // Set the attributes
                            if (node.Attributes != null)
                            {
                                foreach (XmlAttribute attr in node.Attributes)
                                {
                                    var prop = control.GetProperty(attr.Name);
                                    if (prop == null || attr.Value == "")
                                        continue;
                                    if (prop.PropertyType.IsEnum)
                                    {
                                        if (Enum.TryParse(prop.PropertyType, attr.Value, out var result))
                                            prop.SetValue(instance, result);
                                    }
                                    else
                                        prop.SetValue(instance, attr.Value);
                                    // TODO: Better error handling
                                }
                            }

                            if (instance is MultiChildContainer mc)
                            {
                                foreach (XmlNode c in node.ChildNodes)
                                {
                                    ParseNode(c, instance);
                                }
                            }
                        }
                    }
                    break;
                case XmlNodeType.Attribute:
                    break;
                case XmlNodeType.Text:
                    break;
                case XmlNodeType.CDATA:
                    break;
                case XmlNodeType.EntityReference:
                    break;
                case XmlNodeType.Entity:
                    break;
                case XmlNodeType.ProcessingInstruction:
                    break;
                case XmlNodeType.Comment:
                    break;
                case XmlNodeType.Document:
                    break;
                case XmlNodeType.DocumentType:
                    break;
                case XmlNodeType.DocumentFragment:
                    break;
                case XmlNodeType.Notation:
                    break;
                case XmlNodeType.Whitespace:
                    break;
                case XmlNodeType.SignificantWhitespace:
                    break;
                case XmlNodeType.EndElement:
                    break;
                case XmlNodeType.EndEntity:
                    break;
                case XmlNodeType.XmlDeclaration:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
