namespace Lunar.Native.Win32;

public class Win32ApplicationFeature : ApplicationFeature
{
    public override void OnWindowReady(Window window)
    {
        base.OnWindowReady(window);
        if (window is Win32Window win)
        {
            // Apply Dark Title Bar
            if (Win32.IsUsingDarkTheme())
            {
                Win32.ApplyDarkTitleBar(win);
            }
            // Set scaling factor
            window.DpiScaling = Win32.GetDisplayScaleFactor(win.handle);
        }
    }
    public override void OnApplicationReady(IApplication application)
    {
        base.OnApplicationReady(application);
        Win32.SetProcessDPIAware();
    }

}
