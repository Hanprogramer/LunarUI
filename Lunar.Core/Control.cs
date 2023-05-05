using SkiaSharp;
using System.Numerics;
namespace Lunar.Core
{
    /// <summary>
    /// Base class for all controls
    /// </summary>
    public class Control
    {

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
        public Vector2 Size { get => size;
            set
            {
                if (size == value)
                    return;
                OnResized(value);
                size = value;
            }
        }

        /// <summary>
        /// Control's Minimum Size
        /// </summary>
        public Vector2 MinSize { get; set; }

        /// <summary>
        /// Control's Position relative to window
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Control's Position relative to parent
        /// </summary>
        public Vector2 RelativePosition { get => Position - Parent.Position; }

        /// <summary>
        /// Control's spacing outside
        /// </summary>
        public Vector4 Margin { get; set; }

        /// <summary>
        /// Control's spacing inside
        /// </summary>
        public Vector4 Padding { get; set; }

        /// <summary>
        /// Size of the control including padding and margins.
        /// </summary>
        public Vector2 MeasuredSize
        {
            get => new Vector2(
                Size.X + Padding.X + Padding.Z + Margin.X + Margin.Z,
                Size.Y + Padding.Y + Padding.W + Margin.Y + Margin.W);
            set => Size = new Vector2(
                value.X - (Padding.X + Padding.Z + Margin.X + Margin.Z), 
                value.Y - (Padding.Y + Padding.W + Margin.Y + Margin.W));
        }

        /// <summary>
        /// Control's Weight for sizing inside of containers
        /// </summary>
        public float Weight = 0;

        #endregion

        /// <summary>
        /// Called every update frame
        /// </summary>
        /// <param name="dt">Delta time since last frame</param>
        public virtual void OnUpdate(double dt) { }

        /// <summary>
        /// Called when rendered to screen
        /// </summary>
        /// <param name="canvas">What to draw to screen</param>
        public virtual void OnRender(SKCanvas canvas) { }
        
        /// <summary>
        /// When the control's size changed. Called before changed
        /// </summary>
        /// <param name="newSize"></param>
        public virtual void OnResized(Vector2 newSize) { }
    }
}
