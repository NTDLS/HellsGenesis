using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;
using Si.Library.Types.Geometry;
using System.Drawing;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Sprites
{
    public class SpriteTextBlock : SpriteBase
    {
        private Size _size = Size.Empty;
        private string _text;

        #region Properties.

        //Non-sprites (e.g. only text) bounds are simple, unlike sprites the text bounds start at X,Y and go to Width/Height.
        public Rectangle BoundsI => new((int)(Location.X), (int)(Location.Y), Size.Width, Size.Height);
        public override RectangleF Bounds => new((float)(Location.X), (float)(Location.Y), Size.Width, Size.Height);
        public override RectangleF RenderBounds => new((float)(RenderLocation.X), (float)(RenderLocation.Y), Size.Width, Size.Height);

        public TextFormat Format { get; set; }
        public SolidColorBrush Color { get; private set; }
        public double Height => _size.Height;
        public override Size Size => _size;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                var size = _gameEngine.Rendering.GetTextSize(_text, Format);
                _size = new Size((int)size.Width, (int)size.Height);
            }
        }

        #endregion

        public SpriteTextBlock(GameEngineCore gameEngine, TextFormat format, SolidColorBrush color, SiPoint location, bool isFixedPosition)
            : base(gameEngine)
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
            X = _gameEngine.Display.NatrualScreenSize.Width / 2 - Size.Width / 2;
            Y = _gameEngine.Display.NatrualScreenSize.Height / 2 - Size.Height / 2;
        }

        public void SetTextAndCenterY(string text)
        {
            Text = text;
            Y = _gameEngine.Display.NatrualScreenSize.Height / 2 - Size.Height / 2;
        }

        public void SetTextAndCenterX(string text)
        {
            Text = text;
            X = _gameEngine.Display.NatrualScreenSize.Width / 2 - Size.Width / 2;
        }

        public override void Render(RenderTarget renderTarget)
        {
            if (Visable)
            {
                _gameEngine.Rendering.DrawTextAt(renderTarget,
                    (float)RenderLocation.X,
                    (float)RenderLocation.Y,
                    0, _text ?? string.Empty, Format, Color);
            }
        }
    }
}