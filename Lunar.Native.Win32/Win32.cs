using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Lunar.Native.Win32
{
    public static class Win32
    {
        public enum DwmWindowAttribute : uint
        {
            NCRenderingEnabled = 1,
            NCRenderingPolicy,
            TransitionsForceDisabled,
            AllowNCPaint,
            CaptionButtonBounds,
            NonClientRtlLayout,
            ForceIconicRepresentation,
            Flip3DPolicy,
            ExtendedFrameBounds,
            HasIconicBitmap,
            DisallowPeek,
            ExcludedFromPeek,
            Cloak,
            Cloaked,
            FreezeRepresentation,
            PassiveUpdateMode,
            UseHostBackdropBrush,
            UseImmersiveDarkMode = 20,
            WindowCornerPreference = 33,
            BorderColor,
            CaptionColor,
            TextColor,
            VisibleFrameBorderThickness,
            SystemBackdropType,
            Last
        }

        [DllImport("dwmapi.dll", PreserveSig = true)]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, DwmWindowAttribute attr, ref int attrValue, int attrSize);
        [DllImport("user32.dll")]
        static extern bool UpdateWindow(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        
        
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();
        
        public static bool IsUsingDarkTheme()
        {
            try
            {
                int res = (int)Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", -1);
                return res == 0;
            }
            catch
            {
                return false;
            }
        }

        public static void ApplyDarkTitleBar(Win32Window window)
        {
            var val = 1;
            DwmSetWindowAttribute(window.handle, DwmWindowAttribute.UseImmersiveDarkMode, ref val, 4);
            UpdateWindow(window.handle);
            MoveWindow(window.handle, window.X, window.Y, window.Width, window.Height, true);
        }
    }
}
