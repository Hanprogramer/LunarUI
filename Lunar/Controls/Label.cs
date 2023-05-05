﻿using Lunar.Core;
using SkiaSharp;
using System.Numerics;
namespace Lunar.Controls
{
    public class Label : Control
    {
        public TextAlign TextAlign { get; set; } = TextAlign.Center;
        public ParagraphAlign ParagraphAlign { get; set; } = ParagraphAlign.Center;

        private string text;
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
        public SKFont Font { get => font ?? DefaultFont; set => font = value; }
        public SKPaint Paint { get => paint ?? DefaultPaint; set => paint = value; }

        public static SKFont DefaultFont = new SKFont();
        public static SKPaint DefaultPaint = new SKPaint();

        /// <summary>
        /// The text position and size
        /// </summary>
        private Vector4 TextBound = new();

        public override void OnRender(SKCanvas canvas)
        {
            base.OnRender(canvas);
            canvas.DrawText(Text, TextBound.X, TextBound.Y, DefaultFont, DefaultPaint);
        }

        public void RecalculateTextBound()
        {
            //TODO: Implement text alignment
            SKRect size = new();
            Paint.MeasureText(Text, ref size);
            TextBound.Z = size.Width;
            TextBound.W = size.Height;
            TextBound.X = Size.X / 2.0f - size.Width / 2.0f;
            TextBound.Y = Size.Y / 2.0f - size.Height / 2.0f;
        }

        public override void OnResized(Vector2 newSize)
        {
            base.OnResized(newSize);
            RecalculateTextBound();
        }
    }
}