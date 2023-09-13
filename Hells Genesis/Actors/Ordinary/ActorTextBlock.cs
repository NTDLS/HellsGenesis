using HG.Actors.BaseClasses;
using HG.Engine;
using HG.Types.Geometry;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System.Drawing;

namespace HG.Actors.Ordinary
{
    internal class ActorTextBlock : ActorBase
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
                var size = _core.DirectX.GetTextSize(_text, Format);
                _size = new Size((int)size.Width, (int)size.Height);
            }
        }

        #endregion

        public ActorTextBlock(Core core, TextFormat format, SolidColorBrush color, HgPoint location, bool isFixedPosition)
            : base(core)
        {
            IsFixedPosition = isFixedPosition;
            Location = new HgPoint(location);
            Color = color;

            Format = format;
        }

        public override void Render(RenderTarget renderTarget)
        {
            if (Visable)
            {
                _core.DirectX.DrawTextAt(renderTarget, (float)X, (float)Y, 0, _text ?? string.Empty, Format, Color);
            }
        }
    }
}