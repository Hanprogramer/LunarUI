namespace Lunar
{
    /// <summary>
    /// A custom URI path with parser
    /// </summary>
    public class LunarURI
    {
        private static string _rootPath = "";
        /// <summary>
        /// The current URI's root path. Usually points to the current project root
        /// </summary>
        public static string RootPath { get => GetRootPath(); set => SetRootPath(value); }
        /// <summary>
        /// The path after being processed. Leads to the actual path
        /// set by the runner
        /// </summary>
        public string ActualPath { get; set; }
        /// <summary>
        /// The one provided in the constructor
        /// </summary>
        private string path = "";
        public string Path { get => path; set => SetPath(value); }
        public LunarURI(string uri)
        {
            Path = uri;
        }

        /// <summary>
        /// Setter for the Path property, also parses it
        /// </summary>
        public void SetPath(string value)
        {
            path = value;
            ActualPath = ParseToPath(value);
        }

        public static string ParseToPath(string value)
        {
            if (value.Equals("/") || value.Equals("\\"))
                return _rootPath;
            throw new Exception("Can't parse Lunar URI: " + value);
        }

        /// <summary>
        /// Set the root path for all application
        /// </summary>
        /// <param name="value"></param>
        public static void SetRootPath(string value)
        {
            _rootPath = value;
        }

        public static string GetRootPath()
        {
            if (_rootPath.Equals(""))
                throw new Exception("Lunar's RootPath is not initialized yet! Make sure to create an Application object first!");
            return _rootPath;
        }

        public static implicit operator LunarURI(string path)
        {
            return new LunarURI(path);
        }
    }
}
