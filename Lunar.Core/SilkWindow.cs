using Lunar.Native;
using Silk.NET.Core.Contexts;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using SkiaSharp;
using IWindow = Silk.NET.Windowing.IWindow;
namespace Lunar
{
    public class SilkWindow : Window
    {
        private string title = "Window.Title";
        public string Title
        {
            get => title;
            set
            {
                title = value;
                if(_window != null)
                    _window.Title = value;
            }
        }
        public IApplication Application { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public int MouseX { get; set; } = 0;
        public int MouseY { get; set; } = 0;
        public bool IsInitialized { get => _window?.IsInitialized ?? false; }
        public bool IsClosing
        {
            get => !IsInitialized || !IsRunning || (_window?.IsClosing ?? true); 
        }
        public bool IsRunning { get; set; } = false;
        private bool IsReady = false;

        // public event Window.WindowEvent OnReady;
        // public event Window.WindowEvent OnClosing;

        // Silk.NET stuffs
        protected Silk.NET.Windowing.IWindow? _window;
        private IGLContext _context;
        private Thread RenderThread;
        private GL gl;
        private object locker = new();

        // Skia stuffs
        private GRContext skCtx;
        private SKSurface skSurface;

        public bool IsMultiThreaded { get; private set; }
        
        

        public SilkWindow(string path, string title, int w = 800, int h = 600, bool isMultiThreaded = false)
        {
            Title = title;
            Path = new LunarURI(path);
            Width = w;
            Height = h;
            IsMultiThreaded = isMultiThreaded;
            //Silk.NET.Windowing.Sdl.SdlWindowing.Use();
            _window = Silk.NET.Windowing.Window.Create(WindowOptions.Default with
            {
                Title = title,
                Size = new Vector2D<int>(w, h),
                ShouldSwapAutomatically = !isMultiThreaded,
                IsContextControlDisabled = isMultiThreaded,
                VSync = !isMultiThreaded
            });
            _window.Load += Ready;
            _window.Update += Update;
            _window.Render += Render; 
            _window.Closing += Dispose;
            _window.Resize += vector2D => Resized(vector2D.X, vector2D.Y);
            // Control = new StackContainer();
        }

        public override void Initialize()
        {
            _window.Initialize();
        }

        public virtual void Ready()
        {
            // Creating GL Context
            _context = _window.GLContext;
            _context.Clear();
            gl = _window.CreateOpenGL();
            _context.MakeCurrent();
            IsReady = true;
            OnReady();

            var input = _window.CreateInput();
            foreach(var mouse in input.Mice)
            {
                mouse.MouseMove += (mouse1, vector2) =>
                {
                    MouseX = (int)vector2.X;
                    MouseY = (int)vector2.Y;
                };
            }
            
            // Set running state
            IsRunning = true;
            
            // Set control size
            Control.Size = new Vector2(Width, Height);
            Control.Position = Vector2.Zero;
            
            // Run window features
            foreach (var f in Features)
                f.OnWindowReady();
            
            // Start rendering thread
            if (IsMultiThreaded)
                DoRenderThread();
            else
                _window.Run();
        }

        private void InitSkia(bool autoResize = true)
        {
            if (skCtx != null) return;
            //_context.MakeCurrent();
            //var glInt = GRGlInterface.CreateOpenGl((name) => _context.GetProcAddress(name));
            skCtx = GRContext.CreateGl(new GRContextOptions()
            {
                AvoidStencilBuffers = true
            });
            if (skCtx == null)
                throw new Exception("Error creating Skia Context for GL");
            if (autoResize)
                ResizeSkia();
        }

        public void Update(double dt)
        {
            Control.OnUpdate(dt);
            // Run window features
            foreach (var f in Features)
                f.OnWindowUpdate(dt);
        }

        public void Render(double dt)
        {
            if (IsReady)
            {
                InitSkia();
                gl.Clear(ClearBufferMask.ColorBufferBit);
                skSurface.Canvas.DrawColor(SKColors.White);
                Control.OnRender(skSurface.Canvas);
                // Run window features
                foreach (var f in Features)
                    f.OnWindowRender(skSurface.Canvas);
                skCtx.Flush();
            }
        }

        /// <summary>
        /// Internal method for calling the renderer
        /// </summary>
        private void DoRenderThread()
        {
            RenderThread = new Thread(() =>
            {
                _context.MakeCurrent();
                _context.SwapInterval(1);
                // Initialize skia
                InitSkia();
                while (IsRunning && !IsClosing)
                {
                    lock (locker)
                    {
                        _window.DoRender();
                    }
                    _context.SwapBuffers();
                }
            });
            RenderThread.Start();
        }

        public void Dispose()
        {
            OnClosing();
            IsReady = false;
            IsRunning = false;
            // Run window features
            foreach (var f in Features)
                f.OnWindowClosing();
        }

        public override void Close()
        {
            IsRunning = false;
        }

        public void Resized(int w, int h)
        {
            Width = w;
            Height = h;
            if (IsReady)
            {
                gl?.Viewport(0, 0, (uint)Width, (uint)Height);
                ResizeSkia();
                if (!IsMultiThreaded)
                    _window.DoRender();

                Control.Size = new Vector2(Width, Height);
                Control.Position = Vector2.Zero;
            }
        }


        private static readonly GRGlFramebufferInfo _fbi = new GRGlFramebufferInfo(0, (uint)InternalFormat.Rgba8);
        private void ResizeSkia()
        {
            if (skCtx == null)
            {
                InitSkia(false);
            }
            var old = skSurface;

            var beRenderTarget = new GRBackendRenderTarget(Width, Height, 0, 0, _fbi);
            lock (locker)
            {
                // Recreate the window surface
                skSurface = SKSurface.Create(skCtx, beRenderTarget, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888, null, null);
            }
            if (skSurface == null)
                throw new Exception("Error creating window surface");
            old?.Dispose();
        }

        public override void DoUpdate()
        {
            _window.DoUpdate();
            _window.DoEvents();
        }
    }
}
