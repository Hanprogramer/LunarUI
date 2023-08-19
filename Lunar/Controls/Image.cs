using Lunar.Core;
using Lunar.Native;
using SkiaSharp;
using Svg.Skia;
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
        private LunarURI? _source = null;
        private SKBitmap? _bitmap;

        private SKSvg? _svg;
        private SKMatrix? _matrix;

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
        public LunarURI? Source { get => _source; set => SetSource(value); }
        public Image(Window window) : base(window)
        {
        }

        public async void SetSource(LunarURI? val)
        {
            _source = val;
            if (val != null)
            {
                // Load the image
                try
                {
                    var path = val.ActualPath;
                    if (path.EndsWith(".svg"))
                    {
                        _svg = new SKSvg();
                        _svg.Load(path);
                    }
                    else
                    {
                        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        using MemoryStream memStream = new MemoryStream();
                        await stream.CopyToAsync(memStream);
                        memStream.Seek(0, SeekOrigin.Begin);
                        _bitmap = SKBitmap.Decode(memStream);
                    }
                }
                catch
                {
                    Console.WriteLine("Error loading image from: " + Source.Path);
                }
            }
            else
            {
                _bitmap = null;
            }
            RecalculateRect();
        }

        public override void OnRender(SKCanvas canvas)
        {
            base.OnRender(canvas);
            if (_rect == null) RecalculateRect();
            int save = canvas.Save();
            canvas.ClipRect(new SKRect(Position.X, Position.Y, Position.X + Size.X, Position.Y + Size.Y));
            if (_bitmap != null)
            {
                canvas.DrawBitmap(_bitmap, new SKRect(_rect.X, _rect.Y, _rect.X + _rect.Width, _rect.Y + _rect.Height));
            }
            if (_svg != null && _svg.Picture != null)
            {
                // Get drawing surface bounds
                var drawBounds = new SKRect(_rect.X, _rect.Y, _rect.X + _rect.Width, _rect.Y + _rect.Height);

                // Get bounding rectangle for SVG image
                var boundingBox = _svg.Picture!.CullRect;

                // Translate and scale drawing canvas to fit SVG image
                canvas.Translate(drawBounds.MidX, drawBounds.MidY);
                canvas.Scale(0.9f *
                             Math.Min(drawBounds.Width / boundingBox.Width,
                                 drawBounds.Height / boundingBox.Height));
                canvas.Translate(-boundingBox.MidX, -boundingBox.MidY);

                // Now finally draw the SVG image
                //canvas.DrawPicture(svg.Picture, paint);
                canvas.DrawPicture(_svg.Picture);

                // Optional -> Reset the matrix before performing more draw operations
                canvas.ResetMatrix();
            }
            canvas.RestoreToCount(save);
        }

        public void RecalculateRect()
        {
            float w, h;
            if (_bitmap != null)
            {
                w = _bitmap.Width;
                h = _bitmap.Height;
            }
            else if (_svg != null && _svg.Picture != null)
            {
                var boundingBox = _svg.Picture!.CullRect;
                w = boundingBox.Width;
                h = boundingBox.Height;
            }
            else
            {
                w = 0;
                h = 0;
            }
            switch (ImageFit)
            {
                case ImageFit.Stretch:
                    _rect = new Rect(Position, Size);
                    break;
                case ImageFit.Keep:
                    _rect = new Rect(
                        Position.X + Size.X / 2.0f - w / 2.0f,
                        Position.Y + Size.Y / 2.0f - h / 2.0f,
                        w,
                        h);
                    break;
                case ImageFit.Cover:
                {
                    // Calculate the aspect ratio of the image
                    float imageAspectRatio = w / h;

                    // Calculate the aspect ratio of the rectangle
                    float rectangleAspectRatio = Size.X / Size.Y;


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
                    float imageAspectRatio = w / h;

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
            }
        }


        public override void OnResized(Vector2 newSize)
        {
            base.OnResized(newSize);
            RecalculateRect();
        }
    }
}
