using Lunar.Controls;
using Lunar.Native;
using System.Xml;
namespace Lunar.Scripting
{
    public class XmlScriptingFeature : WindowFeature
    {
        private XmlDocument doc;
        private XmlComponent root;
        public ControlRegistry ControlRegistry;
        private readonly FileSystemWatcher _watcher;
        private bool isParsing = false;
        public string WorkingDirectory = "";

        public StyleParser StyleParser;

        public XmlScriptingFeature(Window window) : base(window)
        {
            _watcher = new FileSystemWatcher();
        }
        public override void OnWindowReady()
        {
            base.OnWindowReady();

            ControlRegistry = (ControlRegistry)Window.Application.GetControlRegistry();
            StyleParser = new StyleParser(ControlRegistry);
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
            WorkingDirectory = Path.GetFullPath(path);
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
            var cont = (Window.Control as StackContainer)!;
            cont.ClearChildren();
            Window.Styles.Clear();
            root = XmlComponent.FromDocument(doc,this);
            cont.AddChild(root.Instantiate(null));
        }

        public static XmlDocument LoadDocument(string path)
        {
            var doc = new XmlDocument();
            string content;
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(stream))
            {
                content = sr.ReadToEnd();
            }
            if (content == "")
                throw new Exception("File is empty");
            
            doc.LoadXml(content);
            return doc;
        }
    }
}
