using Lunar.Controls;
using Lunar.Core;
using Lunar.Native;
using Microsoft.ClearScript.Util.Web;
using System.Xml;
namespace Lunar.Scripting
{
    /// <summary>
    /// A Xml Component is a "template" for a control that can be instantiated
    /// either as a whole window content or a subcomponent
    /// </summary>
    public class XmlComponent
    {
        public Dictionary<String, XmlComponent> Includes;
        public Dictionary<String, String> Properties;
        public List<Style> Styles;
        public Window Window;
        public XmlScriptingFeature Scripting;
        public string Name = "";

        /// Reference to XML nodes directly copied
        private readonly XmlNode? _contentNode, _styleNode, _scriptNode, _includesNode, _propertiesNode;

        /// <summary>
        /// Creates a new Xml component from a bunch of XML nodes
        /// </summary>
        /// <param name="scripting"></param>
        /// <param name="contentNode"></param>
        /// <param name="styleNode"></param>
        /// <param name="scriptNode"></param>
        public XmlComponent(XmlScriptingFeature scripting, XmlNode? contentNode, XmlNode? styleNode, XmlNode? scriptNode, XmlNode? includesNode, XmlNode? propertiesNode)
        {
            Scripting = scripting;
            Window = scripting.Window;

            this._contentNode = contentNode?.CloneNode(true);
            this._styleNode = styleNode?.CloneNode(true);
            this._scriptNode = scriptNode?.CloneNode(true);
            this._includesNode = includesNode?.CloneNode(true);
            this._propertiesNode = propertiesNode?.CloneNode(true);

            Includes = new Dictionary<string, XmlComponent>();
            Properties = new Dictionary<string, string>();
            Styles = new List<Style>();
        }

        public static XmlComponent FromDocument(XmlDocument doc, XmlScriptingFeature scripting)
        {
            XmlNode? contentNode = null, scriptNode = null, styleNode = null, includesNode = null, propertiesNode=null;
            foreach (XmlNode node in doc.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Element || node.Name != "Template")
                    continue;
                List<string> includes = new();
                foreach (XmlNode n in node.ChildNodes)
                {
                    switch (n)
                    {
                        case { NodeType: XmlNodeType.Element, Name: "Content" }:
                        {
                            contentNode = n;
                            break;
                        }
                        case { NodeType: XmlNodeType.Element, Name: "Styles" }:
                        {
                            styleNode = n;
                            break;
                        }
                        case { NodeType: XmlNodeType.Element, Name: "Script" }:
                        {
                            scriptNode = n;
                            break;
                        }
                        case { NodeType: XmlNodeType.Element, Name: "Properties" }:
                        {
                            propertiesNode = n;
                            break;
                        }
                        case { NodeType: XmlNodeType.Element, Name: "Includes" }:
                        {
                            includesNode = n;
                            foreach (XmlNode child in n.ChildNodes)
                            {
                                if (child is { NodeType: XmlNodeType.Element, Name: "Xml" })
                                {
                                    string? source = null;
                                    if (child.Attributes != null)
                                        foreach (XmlAttribute attr in child.Attributes)
                                        {
                                            if (attr.Name == "Source")
                                            {
                                                source = attr.Value;
                                            }
                                        }
                                    if (source != null)
                                    {
                                        // Add the included xml
                                        includes.Add(source);
                                    }
                                    else
                                    {
                                        throw new Exception("The included Xml must have a 'Source' argument");
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
                var obj = new XmlComponent(scripting, contentNode, styleNode, scriptNode, includesNode, propertiesNode);
                // Add the included components
                foreach (var include in includes)
                {
                    var compDoc = XmlScriptingFeature.LoadDocument(Path.Join(scripting.WorkingDirectory, include));
                    var comp = XmlComponent.FromDocument(compDoc, scripting);
                    var name = Path.GetFileNameWithoutExtension(include);
                    obj.Includes[name] = comp;
                }
                return obj;
            }
            throw new Exception("Can't find <Template> element in the Xml file!");
        }

        private Control CreateControl(string name, XmlNode node)
        {
            // look in the current component includes
            if (Includes.TryGetValue(name, out XmlComponent value))
            {
                var comp = value.Instantiate(node);
                return comp;
            }
            // look in the component registry
            var control = Scripting.ControlRegistry.GetControlOrNull(name);
            if (control != null)
            {
                var constructor = control.GetConstructor(new Type[]
                {
                    typeof(Window)
                });
                var instance = (Control)constructor.Invoke(new object[]
                {
                    Window
                });
                return instance;
            }
            throw new Exception($"Control not found: {name}");
        }

        /// <summary>
        /// Parse the node tree to a tree filled with controls
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        private void ParseContentNode(XmlNode node, in Control parent, ComposedControl? composedParent = null)
        {
            if (node.NodeType != XmlNodeType.Element)
                return;
            if (parent is not MultiChildContainer m)
                return;
            
            var instance = CreateControl(node.Name, node);
            instance.ComposableParent = composedParent;
            
            var control = instance.GetType();
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
                    else if (prop.PropertyType == typeof(Int32))
                    {
                        prop.SetValue(instance, Int32.Parse(attr.Value));
                    }
                    else if (prop.PropertyType == typeof(Single?))
                    {
                        prop.SetValue(instance, Single.Parse(attr.Value));
                    }
                    else if (prop.PropertyType == typeof(Fill))
                    {
                        Fill val = StyleParser.ParseColor(attr.Value);
                        prop.SetValue(instance, val);
                    }
                    else if (prop.PropertyType == typeof(Spacing))
                    {
                        prop.SetValue(instance, Spacing.Parse(attr.Value));
                    }
                    else if (prop.PropertyType == typeof(string))
                    {
                        var val = attr.Value;
                        if (val.StartsWith("@"))
                        {
                            // Parse in a IPropertySource property
                            var propName = val.Substring(1);
                            var valu = composedParent?.GetProperty(propName);
                            prop.SetValue(instance, valu ?? "");
                        }
                        else
                            prop.SetValue(instance, (attr.Value));
                    }
                    else
                        prop.SetValue(instance, attr.Value);
                    // TODO: Better error handling
                }
            }

            if (!node.HasChildNodes)
                return;

            ComposedControl? cp = null;
            if (instance is ComposedControl sc)
            {
                cp = sc;
            }
            else
            {
                cp = composedParent;
            }
            
            if (instance is MultiChildContainer)
            {
                foreach (XmlNode c in node.ChildNodes)
                {
                    ParseContentNode(c, instance, cp);
                }
            }
            else if (instance is SingleChildContainer)
            {
                if (node.ChildNodes.Count == 1)
                    ParseContentNode(node.FirstChild, instance, cp);
                else
                    throw new Exception($"{node.Name} is a SingleChildContainer and can only has one child or no child at all");
            }
        }

        public ComposedControl Instantiate(XmlNode? node)
        {
            var baseControl = new ComposedControl(Window);
            
            if (_propertiesNode != null)
            {
                ParsePropertiesNode(_propertiesNode, in baseControl.Properties);
            }
            
            if (node != null && node.Attributes != null)
            {
                foreach (XmlAttribute attr in node.Attributes)
                {
                    baseControl.SetProperty(attr.Name, attr.Value);
                }
            }
            
            if (_styleNode != null)
            {
                foreach (XmlNode nn in _styleNode.ChildNodes)
                {
                    ParseStyleNodes(nn, in baseControl.Styles);
                }
            }
            if (_contentNode != null)
            {
                foreach (XmlNode child in _contentNode.ChildNodes)
                {
                    ParseContentNode(child, baseControl, baseControl);
                }
            }

            // TODO: Implement script node
            return baseControl;
        }

        private void ParsePropertiesNode(XmlNode node, in Dictionary<string, ComposedControlProperty> props)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child is { NodeType: XmlNodeType.Element })
                {
                    if (child.Name != "Property")
                        throw new Exception("Properties can only contain Property objects!");

                    // Parse the property node
                    ComposedControlPropertyType? propType = null;
                    string name = "", defaultValue = "";
                    bool required = false;
                    if (child.Attributes == null) throw new Exception("A property node must have attributes!");
                    foreach (XmlAttribute attr in child.Attributes)
                    {
                        if (attr.Name == "Type")
                        {
                            if (Enum.TryParse(attr.Value, out ComposedControlPropertyType _propType))
                            {
                                propType = _propType;
                            }
                            else
                            {
                                throw new Exception($"Unknown property type: " + attr.Value);
                            }
                        }
                        else if (attr.Name == "Name")
                        {
                            name = attr.Value;
                        }
                        else if (attr.Name == "Default")
                        {
                            defaultValue = attr.Value;
                        }
                        else if (attr.Name == "Required")
                        {
                            required = attr.Value == "true";
                        }
                    }

                    if (propType == null)
                    {
                        throw new Exception($"A property type is required!");
                    }
                    
                    // TODO: check if other attributes empty as well
                    
                    // Add the property
                    props.Add(name, new ComposedControlProperty(name, (ComposedControlPropertyType)propType, defaultValue, required));
                }
            }
        }

        private void ParseStyleNodes(XmlNode node, in List<Style> styles)
        {
            var style = new Style();
            foreach (XmlAttribute attr in node.Attributes)
            {
                if (attr.Name == "Class")
                    style.ClassName = attr.Value;
                if (attr.Name == "Target")
                    style.Target = attr.Value;
            }
            if (node.HasChildNodes)
            {
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    ParseStyleProperties(childNode, ref style);
                }
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
                style.Set(node.Name, Scripting.StyleParser.ParseValueNode(node));
            }
        }
    }
}
