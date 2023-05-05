﻿namespace Lunar.Native.Win32
{
    public class Win32NativeContext : NativeContext
    {

        public override void OnAddFeatures(IApplication application)
        {
            application.AddFeature(new Win32ApplicationFeature());
        }
        public override Window CreateWindow(string title, int w = 800, int h = 600, bool isMultiThreaded = false)
        {
            return new Win32Window(title,w,h,isMultiThreaded);
        }
    }
}
