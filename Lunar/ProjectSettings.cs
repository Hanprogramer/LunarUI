using System.Text.Json;

namespace Lunar
{
    public class ProjectSettings
    {
        // Settings values
        public int FormatVersion { get; set; } = 1;
        public string Title { get; set; } = "Unknown";
        public string Icon { get; set; } = "";
        public string Author { get; set; } = "";
        public string Main { get; set; } = "";
        public string ScriptingLanguage { get; set; } = "C#";
        public int[] Version { get; set; } = new[]
        {
            0, 0, 0
        };

        public bool AllowInternetImages { get; set; } = false;

        public static ProjectSettings? Load(string path)
        {
            var content = File.ReadAllText(path);
            var obj = JsonSerializer.Deserialize<ProjectSettings>(content);
            return obj;
        }

        public void SaveToFile(string path)
        {
            var content = JsonSerializer.Serialize(this);
            File.WriteAllText(path, content);
        }
    }
}
