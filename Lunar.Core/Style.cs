using Lunar.Core;
using SkiaSharp;
namespace Lunar.Native
{
    public struct Style
    {
        private readonly Dictionary<string, string> _properties = new Dictionary<string, string>();
        private readonly Dictionary<string, Style> _states = new Dictionary<string, Style>();
        public string? ClassName;
        public string? Target;
        public Style()
        {
        }

        public void Set(string prop, string value)
        {
            _properties[prop] = value;
        }

        public object? Get(string prop)
        {
            _properties.TryGetValue(prop, out var result);
            return result;
        }

        public void AddState(string state, Style style)
        {
            _states[state] = style;
        }
        public void Apply(in Control control)
        {
            if (Target != null)
                if (control.GetType().Name != Target)
                    return;
            if (ClassName != null)
                if (!control.ClassList.Contains(ClassName))
                    return;
            apply(control, control.GetType());
        }

        private void apply(in Control control, Type type)
        {
            foreach (var pair in _properties)
            {
                var properties = type.GetProperties();
                foreach (var prop in properties)
                {
                    if (prop.Name != pair.Key)
                        continue;
                    // Found a match. now parse it
                    var val = ParseValue(pair.Value, prop.PropertyType);
                    prop.SetValue(control, val);
                }
            }
        }
        public static object? ParseValue(string val, Type type)
        {
            if (type == typeof(SKColor?))
                return ParseColor(val);
            if (type == typeof(int?))
                return int.Parse(val);
            if (type == typeof(float?))
                return float.Parse(val);
            if (type == typeof(string))
                return val;
            
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
