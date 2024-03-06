using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using Si.Engine.Sprites._Superclass;
using Si.Library.Mathematics.Geometry;
using System.Drawing;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprites
{
    public class SpriteTextBlock : SpriteBase
    {
        private Size _size = Size.Empty;
        private string _text;

        #region Properties.

        //Non-sprites (e.g. only text) bounds are simple, unlike sprites the text bounds start at X,Y and go to Width/Height.
        public override RectangleF Bounds => new((float)Location.X, (float)Location.Y, Size.Width, Size.Height);
        public override RawRectangleF RawBounds => new((float)Location.X, (float)Location.Y, (float)Location.X + Size.Width, (float)Location.Y + Size.Height);
        public override RectangleF RenderBounds => new((float)(RenderLocation.X), (float)(RenderLocation.Y), Size.Width, Size.Height);
        public override RawRectangleF RawRenderBounds => new((float)RenderLocation.X, (float)RenderLocation.Y, (float)RenderLocation.X + Size.Width, (float)RenderLocation.Y + Size.Height);

        public TextFormat Format { get; set; }
        public SolidColorBrush Color { get; private set; }
        public float Height => _size.Height;
        public override Size Size => _size;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                var size = _engine.Rendering.GetTextSize(_text, Format);
                _size = new Size((int)size.Width, (int)size.Height);
            }
        }

        #endregion

        public SpriteTextBlock(EngineCore engine, TextFormat format, SolidColorBrush color, SiPoint location, bool isFixedPosition)
            : base(engine)
        {
            RenderScaleOrder = SiRenderScaleOrder.PostScale;
            IsFixedPosition = isFixedPosition;
            Location = new SiPoint(location);
            Color = color;

            Format = format;
        }

        public void SetTextAndCenterXY(string text)
        {
            Text = text;
            X = _engine.Display.NatrualScreenSize.Width / 2 - Size.Width / 2;
            Y = _engine.Display.NatrualScreenSize.Height / 2 - Size.Height / 2;
        }

        public void SetTextAndCenterY(string text)
        {
            Text = text;
            Y = _engine.Display.NatrualScreenSize.Height / 2 - Size.Height / 2;
        }

        public void SetTextAndCenterX(string text)
        {
            Text = text;
            X = _engine.Display.NatrualScreenSize.Width / 2 - Size.Width / 2;
        }

        public override void Render(RenderTarget renderTarget)
        {
            if (Visable)
            {
                _engine.Rendering.DrawTextAt(renderTarget,
                    (float)RenderLocation.X,
                    (float)RenderLocation.Y,
                    0, _text ?? string.Empty, Format, Color);
            }
        }
    }
}