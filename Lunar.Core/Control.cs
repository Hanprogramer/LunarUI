using Lunar.Native;
using SkiaSharp;
using System.Collections.ObjectModel;
namespace Lunar.Core
{
    /// <summary>
    /// Base class for all controls
    /// </summary>
    public class Control
    {
        protected Window Window { set; get; }

        #region Properties

        /// <summary>
        /// Unique name for this control in this window or control child
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Parent control
        /// </summary>
        public Control Parent { get; set; }

        private Vector2 size = new Vector2();
        /// <summary>
        /// Control's Size
        /// </summary>
        public Vector2 Size
        {
            get => size;
            set
            {
                size = value;
                if (MinSize != Vector2.Zero)
                {
                    size.X = Math.Max(size.X, MinSize.X);
                    size.Y = Math.Max(size.Y, MinSize.Y);
                }
                OnResized(value);
            }
        }

        /// <summary>
        /// Control's Minimum Size
        /// </summary>
        public Vector2 MinSize
        {
            get;
            set;
        }

        /// <summary>
        /// Control's Position relative to window
        /// </summary>
        public Vector2 Position
        {
            get;
            set;
        } = new();

        /// <summary>
        /// Control's Position relative to parent
        /// </summary>
        public Vector2 RelativePosition
        {
            get => Position - Parent.Position;
        }

        /// <summary>
        /// Control's spacing outside
        /// </summary>
        public Spacing Margin
        {
            get;
            set;
        }

        /// <summary>
        /// Control's spacing inside
        /// </summary>
        public Spacing Padding
        {
            get;
            set;
        }

        public SKColor? Background
        {
            get;
            set;
        }
        public SKColor? Foreground
        {
            get;
            set;
        }

        private float? fontSize = null;
        public float? FontSize
        {
            get => fontSize;
            set => SetFontSize(value);
        }
        public float? BorderRadius
        {
            get;
            set;
        }
        private Style? style;
        public Style? Style
        {
            get => style;
            set
            {
                style = value;
                ApplyStyles();
            }
        }

        public ObservableCollection<string> ClassList = new ObservableCollection<string>();

        /// <summary>
        /// Size of the control including padding and margins.
        /// </summary>
        public Vector2 MeasuredSize
        {
            get => new Vector2(
                Size.X + Padding.Width + Margin.Width,
                Size.Y + Padding.Height + Margin.Height);
            set => Size = new Vector2(
                value.X - Padding.Width - Margin.Width,
                value.Y - Padding.Height - Margin.Height);
        }

        /// <summary>
        /// Control's Weight for sizing inside of containers
        /// </summary>
        public float Weight
        {
            get;
            set;
        } = 0;

        #endregion

        public Control(Window window)
        {
            Window = window;
            ClassList.CollectionChanged += (sender, args) =>
            {
                // Class list changed, apply new styles
                ApplyStyles();
            };
        }

        public virtual void ApplyStyles()
        {
            foreach (var style in Window.Styles)
            {
                style.Apply(this);
            }
            Style?.Apply(this);
        }

        /// <summary>
        /// Called every update frame
        /// </summary>
        /// <param name="dt">Delta time since last frame</param>
        public virtual void OnUpdate(double dt) { }

        /// <summary>
        /// Called when rendered to screen
        /// </summary>
        /// <param name="canvas">What to draw to screen</param>
        public virtual void OnRender(SKCanvas canvas)
        {
            if (Background != null)
            {
                canvas.DrawRoundRect(
                    Position.X - Padding.Left,
                    Position.Y - Padding.Top,
                    Size.X + Padding.Width,
                    Size.Y + Padding.Height,
                    BorderRadius ?? 0, BorderRadius ?? 0,
                    new SKPaint()
                    {
                        Color = (SKColor)Background
                    });
            }
            // if(HasClass("Debug") || HasClass("MainButton"))
            // Debugging rect
            // canvas.DrawRect(
            //     Position.X - Padding.Left - Margin.Left,
            //     Position.Y - Padding.Left - Margin.Left,
            //     MeasuredSize.X,
            //     MeasuredSize.Y,
            //     new SKPaint()
            //     {
            //         Color = SKColors.Red,
            //         IsStroke = true
            //     });
        }

        /// <summary>
        /// When the control's size changed. Called before changed
        /// </summary>
        /// <param name="newSize"></param>
        public virtual void OnResized(Vector2 newSize) { }

        public void Refresh()
        {
            OnResized(Size);
            ApplyStyles();
        }

        public virtual void SetFontSize(float? value)
        {
            fontSize = value;
        }

        public bool HasClass(string val)
        {
            return ClassList.Contains(val);
        }
    }
}
