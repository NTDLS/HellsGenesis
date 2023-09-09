using HG.Actors.BaseClasses;
using HG.Engine;
using HG.Types;
using System.Drawing;

namespace HG.Actors.Ordinary
{
    internal class ActorTextBlock : ActorBase
    {
        private Rectangle? _prevRegion;
        private readonly Font _font;
        private readonly Graphics _genericDC; //Not used for drawing, only measuring.
        private readonly Brush _color;

        public bool IsPositionStatic { get; set; }

        #region Properties.

        double _height = 0;
        public double Height
        {
            get
            {
                if (_height == 0)
                {
                    lock (_genericDC)
                    {
                        _height = _genericDC.MeasureString("void", _font).Height;
                    }
                }
                return _height;
            }
        }

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

                var size = _core.DirectX.GetTextSize(_text, _core.DirectX.LargeTextFormat);
                _size = new Size((int)size.Width, (int)size.Height);
            }
        }

        #endregion

        public ActorTextBlock(Core core, string font, Brush color, double size, HgPoint<double> location, bool isPositionStatic)
            : base(core)
        {
            IsPositionStatic = isPositionStatic;
            Location = new HgPoint<double>(location);
            _color = color;
            _font = new Font(font, (float)size);

            _genericDC = _core.Display.DrawingSurface.CreateGraphics();
        }

        public override void Render()
        {
            if (Visable)
            {
                _core.DirectX.DrawTextAt((float)X, (float)Y, 0, _text ?? string.Empty, _core.DirectX.LargeTextFormat, _core.DirectX.SolidColorBrushGreen);
            }
        }
    }
}