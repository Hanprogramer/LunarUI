using Lunar.Controls;
using Lunar.Core;
using Lunar.Native;
using Lunar.Native.Win32;
using System.Collections.ObjectModel;
namespace Lunar;

public class Application : IApplication
{
    public bool IsRunning = false;
    public NativeContext NativeContext;

    public List<ApplicationFeature> Features;
    public List<Window> Windows;
    public List<Type> WindowFeatures = new();

    public ControlRegistry ControlRegistry = new ControlRegistry();
    public ProjectSettings Settings;
    public Application(string path, string name, string? icon = null)
    {
        Name = name;
        Icon = icon;
        Features = new List<ApplicationFeature>();
        Windows = new List<Window>();
        Path = path;

        var settingsPath = System.IO.Path.Join(path, "lunarproject.json");
        if (File.Exists(settingsPath))
        {
            Settings = ProjectSettings.Load(settingsPath) ?? new ProjectSettings();
            Icon = Settings.Icon;
            Name = Settings.Title;
        }
        else
            throw new Exception("Can't find lunarproject.json on " + path);
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
        
        foreach (var feature in Features)
        {
            feature.OnBeforeApplicationReady(this);
        }
    }

    public string? Icon { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public void AddApplicationFeature(ApplicationFeature feature)
    {
        Features.Add(feature);
    }

    public void RemoveApplicationFeature(ApplicationFeature feature)
    {
        Features.Remove(feature);
    }
    public T? GetApplicationFeature<T>() where T : ApplicationFeature
    {
        return Features.OfType<T>().FirstOrDefault();
    }
    
    public IControlRegistry GetControlRegistry()
    {
        return ControlRegistry;
    }
    public Theme Theme { get; set; } = Themes.Dark;

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
    public void CreateWindow(string path)
    {
        var win = NativeContext.CreateWindow(this, path, Name);
        win.Application = this;
        foreach (var windowFeature in WindowFeatures)
        {
            var constructor = windowFeature.GetConstructor(types: new Type[]
            {
                typeof(Window)
            });
            var obj = constructor.Invoke(new object[]
            {
                win
            });
            win.AddFeature((WindowFeature)obj);
        }
        var sc = new StackContainer(win);
        win.Control = sc;
        
        
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
        foreach (var feature in Features)
        {
            feature.OnApplicationReady(this);
        }
        IsRunning = true;
        while (IsRunning)
        {
            // Handles the update
            for (var i = 0; i < Windows.Count; i++)
            {
                if (Windows[i].IsInitialized)
                {
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
    public void AddWindowFeature<T>() where T: WindowFeature
    {
        WindowFeatures.Add(typeof(T));
    }
}
