namespace Lunar.Native.Win32
{
    public class Win32Window : SilkWindow
    {
        public IntPtr handle;
        public Win32Window(string path, string title, int w = 800, int h = 600, bool isMultiThreaded = false) : base(path, title, w, h, isMultiThreaded)
        {
            
        }

        public override void Ready()
        {
            var pointer = _window.Native?.Win32.Value.Hwnd;
            if (pointer == null)
                throw new Exception("Can't get win32 handle");
            handle = (nint)pointer;
            base.Ready();
        }

    }
}
