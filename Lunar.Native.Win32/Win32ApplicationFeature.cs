namespace Lunar.Native.Win32;

public class Win32ApplicationFeature : ApplicationFeature
{
    public override void OnWindowReady(Window window)
    {
        base.OnWindowReady(window);
        if (Win32.IsUsingDarkTheme() && window is Win32Window win)
        {
            Win32.ApplyDarkTitleBar(win);
        }
    }
    public override void OnApplicationReady(IApplication application)
    {
        base.OnApplicationReady(application);
        Win32.SetProcessDPIAware();
    }

}
