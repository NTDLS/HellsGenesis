using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Engine.Types.Geometry;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System.Drawing;

namespace NebulaSiege.Game.Sprites
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

        public SpriteTextBlock(EngineCore core, TextFormat format, SolidColorBrush color, NsPoint location, bool isFixedPosition)
            : base(core)
        {
            IsFixedPosition = isFixedPosition;
            Location = new NsPoint(location);
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