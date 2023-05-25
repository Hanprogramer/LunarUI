using Lunar.Controls;
using Lunar.Core;
using Lunar.Native;
using SkiaSharp;
using System.Xml;
namespace Lunar.Scripting
{
    public class StyleParser
    {
        private ControlRegistry ControlRegistry;
        private Dictionary<string, Type> TypeRegistry;
        public StyleParser(ControlRegistry controlRegistry)
        {
            ControlRegistry = controlRegistry;
            TypeRegistry = new Dictionary<string, Type>();
            // Register types for the xml parser
            RegisterType<SKColor>("Color");
            RegisterType<LinearGradientFill>();
            RegisterType<LinearGradientColor>();

        }

        private void RegisterType<T>()
        {
            TypeRegistry.Add(typeof(T).Name, typeof(T));
        }
        private void RegisterType<T>(string name)
        {
            TypeRegistry.Add(name, typeof(T));
        }

        public object? ParseValueNode(XmlNode node)
        {
            var props = typeof(Control).GetProperties();
            foreach (var prop in props)
            {
                if (prop.Name == node.Name)
                {
                    return ParseNodeAsValue(node, prop.PropertyType);
                }
            }
            return null;
        }

        private object? ParseNodeAsValue(XmlNode node, Type propType)
        {
            if (node.HasChildNodes)
            {
                if (node.ChildNodes.Count == 1)
                {
                    var child = node.FirstChild!;
                    switch (child.NodeType)
                    {
                        case XmlNodeType.Element:
                        {
                            var type = (from pair in TypeRegistry where pair.Key == node.FirstChild!.Name select pair.Value).FirstOrDefault();
                            if (type == null)
                                throw new Exception($"Can't resolve style type: {node.FirstChild!.Name}");
                            var val = ParseNodeAsValue(node.FirstChild!, type);
                            if (val != null)
                                ParseArgumentAndSet(node, val);
                            return val;
                        }
                        case XmlNodeType.Text:
                        {
                            var result = ParseValueText(child.InnerText, propType);
                            if (result != null)
                                ParseArgumentAndSet(node, result);
                            return result;
                        }
                    }
                }
                else
                {
                    // Pass in a list of objects as the constructor
                    var val = new List<object?>();
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        if (child.NodeType != XmlNodeType.Element)
                            continue;
                        var type = (from pair in TypeRegistry where pair.Key == child.Name select pair.Value).FirstOrDefault();
                        if (type == null)
                            throw new Exception($"Can't resolve style type: {node.FirstChild!.Name}");
                        val.Add(ParseNodeAsValue(child, type));
                    }
                    var types = new Type[val.Count];
                    for (int i = 0; i < val.Count; i++)
                    {
                        types[i] = val[i].GetType();
                    }
                    var constructor = propType.GetConstructor(new[]
                    {
                        typeof(object?[])
                    });
                    if (constructor == null)
                        throw new Exception($"Can't find constructor of {propType} with value array of ({string.Join(',', types.Select(x => $"{x}"))})");
                    var result = constructor.Invoke(new object[]
                    {
                        val.ToArray()
                    });
                    ParseArgumentAndSet(node, result);
                    return result;
                }
            }
            {
                var result = ParseValueText(node.InnerText, propType);
                ParseArgumentAndSet(node, result);
                return result;
            }
        }

        public void ParseArgumentAndSet(in XmlNode node, in object instance)
        {
            if (node.Attributes == null)
                return;

            var name = node.Name;
            var type = (from pair in TypeRegistry where pair.Key == name select pair.Value).FirstOrDefault();
            if (type == null)
                // throw new Exception($"Can't resolve style attribute type: {node.Name}");
                return;
            var props = type.GetProperties();
            foreach (XmlAttribute attribute in node.Attributes)
            {
                foreach (var prop in props)
                {
                    if (prop.Name != attribute.Name)
                        continue;
                    // if (prop.PropertyType != type)
                    //     throw new Exception($"Style attribute mismatch, expected {type.Name} got {prop.PropertyType.Name} instead");
                    var value = ParseValueText(attribute.Value, prop.PropertyType);
                    prop.SetValue(instance, value);
                    break;
                }
            }
        }

        public static object? ParseValueText(string val, Type type)
        {
            if (type == typeof(SKColor?))
                return ParseColor(val);
            if (type == typeof(Fill))
            {
                return new SolidFill(ParseColor(val));
            }
            if (type == typeof(LinearGradientColor))
            {
                return new LinearGradientColor(ParseColor(val));
            }
            if (type == typeof(int?) || type == typeof(int))
                return int.Parse(val);
            if (type == typeof(float?) || type == typeof(Single))
                return float.Parse(val);
            if (type == typeof(string))
                return val;
            if (type == typeof(Spacing))
                return Spacing.Parse(val);
            return null;
        }

        public static SKColor ParseColor(string val)
        {
            if (SKColor.TryParse(val, out var col))
                return col;
            return val switch
            {
                "AliceBlue" => SKColors.AliceBlue,
                "AntiqueWhite" => SKColors.AntiqueWhite,
                "Aqua" => SKColors.Aqua,
                "Aquamarine" => SKColors.Aquamarine,
                "Azure" => SKColors.Azure,
                "Beige" => SKColors.Beige,
                "Bisque" => SKColors.Bisque,
                "Black" => SKColors.Black,
                "BlanchedAlmond" => SKColors.BlanchedAlmond,
                "Blue" => SKColors.Blue,
                "BlueViolet" => SKColors.BlueViolet,
                "Brown" => SKColors.Brown,
                "BurlyWood" => SKColors.BurlyWood,
                "CadetBlue" => SKColors.CadetBlue,
                "Chartreuse" => SKColors.Chartreuse,
                "Chocolate" => SKColors.Chocolate,
                "Coral" => SKColors.Coral,
                "CornflowerBlue" => SKColors.CornflowerBlue,
                "Cornsilk" => SKColors.Cornsilk,
                "Crimson" => SKColors.Crimson,
                "Cyan" => SKColors.Cyan,
                "DarkBlue" => SKColors.DarkBlue,
                "DarkCyan" => SKColors.DarkCyan,
                "DarkGoldenrod" => SKColors.DarkGoldenrod,
                "DarkGray" => SKColors.DarkGray,
                "DarkGreen" => SKColors.DarkGreen,
                "DarkKhaki" => SKColors.DarkKhaki,
                "DarkMagenta" => SKColors.DarkMagenta,
                "DarkOliveGreen" => SKColors.DarkOliveGreen,
                "DarkOrange" => SKColors.DarkOrange,
                "DarkOrchid" => SKColors.DarkOrchid,
                "DarkRed" => SKColors.DarkRed,
                "DarkSalmon" => SKColors.DarkSalmon,
                "DarkSeaGreen" => SKColors.DarkSeaGreen,
                "DarkSlateBlue" => SKColors.DarkSlateBlue,
                "DarkSlateGray" => SKColors.DarkSlateGray,
                "DarkTurquoise" => SKColors.DarkTurquoise,
                "DarkViolet" => SKColors.DarkViolet,
                "DeepPink" => SKColors.DeepPink,
                "DeepSkyBlue" => SKColors.DeepSkyBlue,
                "DimGray" => SKColors.DimGray,
                "DodgerBlue" => SKColors.DodgerBlue,
                "Firebrick" => SKColors.Firebrick,
                "FloralWhite" => SKColors.FloralWhite,
                "ForestGreen" => SKColors.ForestGreen,
                "Fuchsia" => SKColors.Fuchsia,
                "Gainsboro" => SKColors.Gainsboro,
                "GhostWhite" => SKColors.GhostWhite,
                "Gold" => SKColors.Gold,
                "Goldenrod" => SKColors.Goldenrod,
                "Gray" => SKColors.Gray,
                "Green" => SKColors.Green,
                "GreenYellow" => SKColors.GreenYellow,
                "Honeydew" => SKColors.Honeydew,
                "HotPink" => SKColors.HotPink,
                "IndianRed" => SKColors.IndianRed,
                "Indigo" => SKColors.Indigo,
                "Ivory" => SKColors.Ivory,
                "Khaki" => SKColors.Khaki,
                "Lavender" => SKColors.Lavender,
                "LavenderBlush" => SKColors.LavenderBlush,
                "LawnGreen" => SKColors.LawnGreen,
                "LemonChiffon" => SKColors.LemonChiffon,
                "LightBlue" => SKColors.LightBlue,
                "LightCoral" => SKColors.LightCoral,
                "LightCyan" => SKColors.LightCyan,
                "LightGoldenrodYellow" => SKColors.LightGoldenrodYellow,
                "LightGray" => SKColors.LightGray,
                "LightGreen" => SKColors.LightGreen,
                "LightPink" => SKColors.LightPink,
                "LightSalmon" => SKColors.LightSalmon,
                "LightSeaGreen" => SKColors.LightSeaGreen,
                "LightSkyBlue" => SKColors.LightSkyBlue,
                "LightSlateGray" => SKColors.LightSlateGray,
                "LightSteelBlue" => SKColors.LightSteelBlue,
                "LightYellow" => SKColors.LightYellow,
                "Lime" => SKColors.Lime,
                "LimeGreen" => SKColors.LimeGreen,
                "Linen" => SKColors.Linen,
                "Magenta" => SKColors.Magenta,
                "Maroon" => SKColors.Maroon,
                "MediumAquamarine" => SKColors.MediumAquamarine,
                "MediumBlue" => SKColors.MediumBlue,
                "MediumOrchid" => SKColors.MediumOrchid,
                "MediumPurple" => SKColors.MediumPurple,
                "MediumSeaGreen" => SKColors.MediumSeaGreen,
                "MediumSlateBlue" => SKColors.MediumSlateBlue,
                "MediumSpringGreen" => SKColors.MediumSpringGreen,
                "MediumTurquoise" => SKColors.MediumTurquoise,
                "MediumVioletRed" => SKColors.MediumVioletRed,
                "MidnightBlue" => SKColors.MidnightBlue,
                "MintCream" => SKColors.MintCream,
                "MistyRose" => SKColors.MistyRose,
                "Moccasin" => SKColors.Moccasin,
                "NavajoWhite" => SKColors.NavajoWhite,
                "Navy" => SKColors.Navy,
                "OldLace" => SKColors.OldLace,
                "Olive" => SKColors.Olive,
                "OliveDrab" => SKColors.OliveDrab,
                "Orange" => SKColors.Orange,
                "OrangeRed" => SKColors.OrangeRed,
                "Orchid" => SKColors.Orchid,
                "PaleGoldenrod" => SKColors.PaleGoldenrod,
                "PaleGreen" => SKColors.PaleGreen,
                "PaleTurquoise" => SKColors.PaleTurquoise,
                "PaleVioletRed" => SKColors.PaleVioletRed,
                "PapayaWhip" => SKColors.PapayaWhip,
                "PeachPuff" => SKColors.PeachPuff,
                "Peru" => SKColors.Peru,
                "Pink" => SKColors.Pink,
                "Plum" => SKColors.Plum,
                "PowderBlue" => SKColors.PowderBlue,
                "Purple" => SKColors.Purple,
                "Red" => SKColors.Red,
                "RosyBrown" => SKColors.RosyBrown,
                "RoyalBlue" => SKColors.RoyalBlue,
                "SaddleBrown" => SKColors.SaddleBrown,
                "Salmon" => SKColors.Salmon,
                "SandyBrown" => SKColors.SandyBrown,
                "SeaGreen" => SKColors.SeaGreen,
                "SeaShell" => SKColors.SeaShell,
                "Sienna" => SKColors.Sienna,
                "Silver" => SKColors.Silver,
                "SkyBlue" => SKColors.SkyBlue,
                "SlateBlue" => SKColors.SlateBlue,
                "SlateGray" => SKColors.SlateGray,
                "Snow" => SKColors.Snow,
                "SpringGreen" => SKColors.SpringGreen,
                "SteelBlue" => SKColors.SteelBlue,
                "Tan" => SKColors.Tan,
                "Teal" => SKColors.Teal,
                "Thistle" => SKColors.Thistle,
                "Tomato" => SKColors.Tomato,
                "Turquoise" => SKColors.Turquoise,
                "Violet" => SKColors.Violet,
                "Wheat" => SKColors.Wheat,
                "White" => SKColors.White,
                "WhiteSmoke" => SKColors.WhiteSmoke,
                "Yellow" => SKColors.Yellow,
                "YellowGreen" => SKColors.YellowGreen,
                "Transparent" => SKColors.Transparent,
                "Empty" => SKColors.Empty,
                _ => SKColor.Empty
            };
        }
    }
}
