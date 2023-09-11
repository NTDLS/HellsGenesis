using HG.Actors.BaseClasses;
using HG.Engine;
using HG.Types;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System.Drawing;

namespace HG.Actors.Ordinary
{
    internal class ActorTextBlock : ActorBase
    {
        public bool IsPositionStatic { get; set; }

        #region Properties.

        public TextFormat Format { get; set; }
        public SolidColorBrush Color { get; private set; }

        public double Height => _size.Height;

        private Size _size = Size.Empty;
        public override Size Size => _size;

        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;

                var size = _core.DirectX.GetTextSize(_text, Format);
                _size = new Size((int)size.Width, (int)size.Height);
            }
        }

        #endregion

        public ActorTextBlock(Core core, TextFormat format, SolidColorBrush color, HgPoint<double> location, bool isPositionStatic)
            : base(core)
        {
            IsPositionStatic = isPositionStatic;
            Location = new HgPoint<double>(location);
            Color = color;

            Format = format;
        }

        public override void Render(SharpDX.Direct2D1.RenderTarget renderTarget)
        {
            if (Visable)
            {
                _core.DirectX.DrawTextAt(renderTarget, (float)X, (float)Y, 0, _text ?? string.Empty, Format, Color);
            }
        }
    }
}