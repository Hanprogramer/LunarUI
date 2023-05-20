using Lunar.Core;
using Lunar.Native;
using SkiaSharp;
namespace Lunar.Controls
{
    public enum ImageFit
    {
        /// <summary>
        /// Tile the image in the x and y axis
        /// </summary>
        Tile,
        /// <summary>
        /// Stretch image to fill entire control
        /// </summary>
        Stretch,
        /// <summary>
        /// Keep the original image size
        /// </summary>
        Keep,
        /// <summary>
        /// Cover the control but keep aspect ratio
        /// </summary>
        Cover,
        /// <summary>
        /// Fit the image in the entire control but keep aspect ratio
        /// While keeping every parts visible
        /// </summary>
        Fit
    }
    public class Image : Control
    {
        private LunarURI? source = null;
        private SKBitmap? bitmap;
        private ImageFit _fit = ImageFit.Stretch;
        public ImageFit ImageFit
        {
            get => _fit;
            set
            {
                _fit = value;
                RecalculateRect();
            }
        }
        private Rect _rect;
        public LunarURI? Source { get => source; set => SetSource(value); }
        public Image(Window window) : base(window)
        {
        }

        public async void SetSource(LunarURI? val)
        {
            source = val;
            if (val != null)
            {
                // Load the image
                try
                {
                    var path = val.ActualPath;
                    await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using MemoryStream memStream = new MemoryStream();
                    await stream.CopyToAsync(memStream);
                    memStream.Seek(0, SeekOrigin.Begin);
                    bitmap = SKBitmap.Decode(memStream);
                }
                catch
                {
                    Console.WriteLine("Error loading image from: " + Source.Path);
                }
            }
            else
            {
                bitmap = null;
            }
            RecalculateRect();
        }

        public override void OnRender(SKCanvas canvas)
        {
            base.OnRender(canvas);
            int save = canvas.Save();
            if (bitmap != null)
            {
                canvas.ClipRect(new SKRect(Position.X, Position.Y, Size.X, Size.Y));
                canvas.DrawBitmap(bitmap, new SKRect(_rect.X, _rect.Y, _rect.X + _rect.Width, _rect.Y + _rect.Height));
            }
            canvas.RestoreToCount(save);
        }

        public void RecalculateRect()
        {
            if (bitmap != null)
            {
                switch (ImageFit)
                {
                    case ImageFit.Stretch:
                        _rect = new Rect(Position, Size);
                        break;
                    case ImageFit.Keep:
                        _rect = new Rect(
                            Position.X + Size.X / 2.0f - bitmap.Width / 2.0f,
                            Position.Y + Size.Y / 2.0f - bitmap.Height / 2.0f,
                            bitmap.Width,
                            bitmap.Height);
                        break;
                    case ImageFit.Cover:
                    {
                        // Calculate the aspect ratio of the image
                        float imageAspectRatio = (float)bitmap.Width / bitmap.Height;

                        // Calculate the aspect ratio of the rectangle
                        float rectangleAspectRatio = (float)Size.X / Size.Y;


                        // Determine the new dimensions for the image
                        float newWidth, newHeight;
                        if (imageAspectRatio < rectangleAspectRatio)
                        {
                            // Fit the image width to the rectangle width
                            newWidth = Size.X;
                            newHeight = (int)(Size.X / imageAspectRatio);
                        }
                        else
                        {
                            // Fit the image height to the rectangle height
                            newWidth = (int)(Size.Y * imageAspectRatio);
                            newHeight = Size.Y;
                        }
                        _rect = new Rect(Position.X + Size.X / 2.0f - newWidth / 2.0f,
                            Position.Y + Size.Y / 2.0f - newHeight / 2.0f,
                            newWidth, newHeight);
                    }
                        break;
                    case ImageFit.Fit:
                    {
                        // Calculate the aspect ratio of the image
                        float imageAspectRatio = (float)bitmap.Width / bitmap.Height;

                        // Calculate the aspect ratio of the rectangle
                        float rectangleAspectRatio = (float)Size.X / Size.Y;


                        // Determine the new dimensions for the image
                        float newWidth, newHeight;
                        if (imageAspectRatio > rectangleAspectRatio)
                        {
                            // Fit the image width to the rectangle width
                            newWidth = Size.X;
                            newHeight = (int)(Size.X / imageAspectRatio);
                        }
                        else
                        {
                            // Fit the image height to the rectangle height
                            newWidth = (int)(Size.Y * imageAspectRatio);
                            newHeight = Size.Y;
                        }
                        _rect = new Rect(Position.X + Size.X / 2.0f - newWidth / 2.0f,
                            Position.Y + Size.Y / 2.0f - newHeight / 2.0f,
                            newWidth, newHeight);
                    }
                        break;
                    default:
                        throw new NotImplementedException($"This ImageFit is not implemented yet");
                        break;
                }
            }
        }

        public override void OnResized(Vector2 newSize)
        {
            base.OnResized(newSize);
            RecalculateRect();
        }
    }
}
