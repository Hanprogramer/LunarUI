using Lunar.Native.Win32;
namespace Lunar;

public class Application : IApplication
{
    public String Name { get; set; }
    public String? Icon { get; set; }
    public String? Path { get; private set; }
    public bool IsRunning = false;
    public NativeContext NativeContext;


    public List<ApplicationFeature> Features;
    public List<Window> Windows;

    public Application(String path, String name, String? icon = null)
    {
        Name = name;
        Icon = icon;
        Features = new();
        Windows = new();
        Path = path;

        LunarURI.SetRootPath(path);

        if (OperatingSystem.IsWindows())
        {
            NativeContext = new Win32NativeContext();
            NativeContext.OnAddFeatures(this);
        }
        else
        {
            throw new Exception("Unimplemented OS Platform");
        }
    }

    public void AddFeature(ApplicationFeature feature)
    {
        Features.Add(feature);
    }

    public void RemoveFeature(ApplicationFeature feature)
    {
        Features.Remove(feature);
    }

    public void AddWindow(Window window)
    {
        Windows.Add(window);
        foreach (var feature in Features)
        {
            feature.OnAddWindow(window);
        }
    }

    public void RemoveWindow(Window window)
    {
        Windows.Remove(window);
    }

    /// <summary>
    /// Create a window from a specific path
    /// Can be relative or absolute
    /// </summary>
    /// <param name="path"></param>
    public void CreateWindow(String path)
    {
        var win = NativeContext.CreateWindow(Name);
        win.Ready += () =>
        {
            foreach (var feature in Features)
            {
                feature.OnWindowReady(win);
            }
        };
        win.Closing += () => RemoveWindow(win);
        //TODO: load the metadata from xml
        AddWindow(win);
    }

    /// <summary>
    /// Create a window at "/"
    /// </summary>
    public void CreateRootWindow()
    {
        CreateWindow("/");
    }

    public void Run()
    {
        IsRunning = true;
        while (IsRunning)
        {
            // Handles the update
            for (var i = 0; i < Windows.Count; i++)
            {
                if (Windows[i].IsInitialized)
                {
                    // if (Windows[i].IsClosing)
                    // {
                    //     RemoveWindow(Windows[i]);
                    //     break;
                    // }
                    if(Windows[i].IsRunning)
                        Windows[i].DoUpdate();
                }
                else
                    Windows[i].Initialize();
                

            }

            if (Windows.Count == 0)
            {
                IsRunning = false;
            }
        }
    }
}
