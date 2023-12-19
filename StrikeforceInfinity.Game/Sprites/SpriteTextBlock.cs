using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using System.Drawing;

namespace StrikeforceInfinity.Game.Sprites
{
    internal class SpriteTextBlock : SpriteBase
    {
        private Size _size = Size.Empty;
        private string _text;

        #region Properties.

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
                var size = _core.Rendering.GetTextSize(_text, Format);
                _size = new Size((int)size.Width, (int)size.Height);
            }
        }

        #endregion

        public SpriteTextBlock(EngineCore core, TextFormat format, SolidColorBrush color, SiPoint location, bool isFixedPosition)
            : base(core)
        {
            IsFixedPosition = isFixedPosition;
            Location = new SiPoint(location);
            Color = color;

            Format = format;
        }

        public override void Render(RenderTarget renderTarget)
        {
            if (Visable)
            {
                _core.Rendering.DrawTextAt(renderTarget, (float)X, (float)Y, 0, _text ?? string.Empty, Format, Color);
            }
        }
    }
}