﻿using Lunar.Core;
using Lunar.Native;
using SkiaSharp;
namespace Lunar.Controls
{
    public class Label : Control
    {
        public TextAlign TextAlign { get; set; } = TextAlign.Center;
        public ParagraphAlign ParagraphAlign { get; set; } = ParagraphAlign.Center;

        private string text = "";
        public string Text
        {
            get => text;
            set
            {
                text = value;
                RecalculateTextBound();
            }
        }

        private SKFont? font;
        private SKPaint? paint;

        // TODO: Recalc bounds when changed
        public SKFont Font { get => font ?? Window.Application.Theme.Font; set => font = value; }
        public SKPaint Paint { get => paint ?? DefaultPaint; set => paint = value; }
        public static SKPaint DefaultPaint = new SKPaint();

        private SKColor? color = null;
        public SKColor Color { get => color ?? Window.Application.Theme.Foreground; set => color = value; }
        // private float fontSize = 16;
        // public override float FontSize
        // {
        //     get => fontSize;
        //     set
        //     {
        //         fontSize = value;
        //         RecalculateTextBound();
        //     }
        // }
        /// <summary>
        /// The text position and size
        /// </summary>
        private Rect TextBound = new();

        public override void OnRender(SKCanvas canvas)
        {
            base.OnRender(canvas);
            Paint.Color = Foreground ?? Window.Application.Theme.Foreground;
            Font.Size = FontSize ?? 16;
            canvas.DrawText(Text, TextBound.X, TextBound.Y, Font, Paint);
        }

        public void RecalculateTextBound()
        {
            //TODO: Implement text alignment
            SKRect size = new();
            float fontSize = FontSize ?? 16;
            Paint.TextSize = fontSize;
            Paint.MeasureText(Text, ref size);
            TextBound.Width = size.Width;
            TextBound.Height = fontSize;
            TextBound.X = Position.X + (Size.X / 2.0f) - (size.Width / 2.0f);
            TextBound.Y = Position.Y + (Size.Y / 2.0f) + (fontSize / 2.0f);

            MinSize = TextBound.Size;
            MinSize = new Vector2(MinSize.X, MinSize.Y + 8);
        }

        public override void OnResized(Vector2 newSize)
        {
            base.OnResized(newSize);
            RecalculateTextBound();
        }
        public Label(Window window) : base(window)
        {
        }

        public override void SetFontSize(float? value)
        {
            base.SetFontSize(value);
            RecalculateTextBound();
        }
    }
}
