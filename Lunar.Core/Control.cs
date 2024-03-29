﻿using Lunar.Controls;
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
        public Cursor Cursor { get; set; } = Cursor.Arrow;

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

        public int Width { get => (int)Size.X; set => Size = Size.WithX(value); }
        public int Height { get => (int)Size.Y; set => Size = Size.WithY(value); }
        public int MinWidth { get => (int)MinSize.X; set => MinSize = MinSize.WithX(value); }
        public int MinHeight { get => (int)MinSize.Y; set => MinSize = MinSize.WithY(value); }

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

        public Fill? Background
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
        /// Size of the control including padding and margins.
        /// </summary>
        public Vector2 MeasuredMinSize
        {
            get => new Vector2(
                MinSize.X + Padding.Width + Margin.Width,
                MinSize.Y + Padding.Height + Margin.Height);
            set => MinSize = new Vector2(
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

        private string state = "";
        public string State
        {
            get => state;
            set
            {
                if (state == value)
                    return;
                state = value;
                ApplyStyles();
            }
        }
        

        
        public TextAlign TextAlign { get; set; }
        
        public IComposedControl? ComposableParent = null;
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
        public void ResetStyles()
        {
            Background = null;
            Foreground = null;
            FontSize = null;
            BorderRadius = null;
        }
        public virtual void ApplyStyles()
        {
            ResetStyles();
            foreach (var style in Window.Styles)
            {
                style.Apply(this);
                if (state != "")
                {
                    style.GetStateOrNull(State)?.Apply(this);
                }
            }
            ComposableParent?.ApplyStyles(this);
            
            Style?.Apply(this);
            if (Style != null && state != "")
            {
                // Apply state
                Style?.GetStateOrNull(State)?.Apply(this);
            }
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
            Background?.OnDraw(canvas,
                Position.X - Padding.Left,
                Position.Y - Padding.Top,
                Size.X + Padding.Width,
                Size.Y + Padding.Height,
                BorderRadius ?? 0);
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
        public virtual void OnResized(Vector2 newSize)
        {
            Background?.OnResize(Position, newSize);
        }

        /// <summary>
        /// Called when mouse is moved
        /// </summary>
        /// <param name="e"></param>
        /// <param name="position"></param>
        public virtual void OnMouseMove(ref MouseEvent e, Vector2 position)
        {
            if (e.Handled)
            {
                return;
            }
            if (position.X > Position.X - Padding.Left && position.X < Position.X + Size.X + Padding.Right &&
                position.Y > Position.Y - Padding.Top && position.Y < Position.Y + Size.Y + Padding.Bottom)
            {
                if (State == "")
                    State = "Hover";
                Window.SetCursor(Cursor);
                e.Handled = true;
            }
            else
            {
                if (State == "Hover")
                    State = "";
            }
        }

        /// <summary>
        /// Called when mouse button is pressed and released
        /// </summary>
        /// <param name="ev"></param>
        /// <param name="button"></param>
        /// <param name="pressed"></param>
        /// <param name="position"></param>
        public virtual void OnMouseButton(ref MouseEvent ev, MouseButton button, bool pressed, Vector2 position)
        {
            if (ev.Handled)
                return;

            if (position.X > Position.X - Padding.Left && position.X < Position.X + Size.X + Padding.Right &&
                position.Y > Position.Y - Padding.Top && position.Y < Position.Y + Size.Y + Padding.Bottom)
            {
                State = pressed ? "Clicked" : "Hover";
                ev.Handled = true;
            }
            else
            {
                if (!pressed)
                {
                    State = "";
                }
            }
        }

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
        
        /// <summary>
        /// Gets a property of a control.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual object? GetProperty(string name)
        {
            return ComposableParent?.GetProperty(name);
        }
    }
}
