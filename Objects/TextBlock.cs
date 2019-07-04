using AI2D.Engine;
using AI2D.Types;
using System;
using System.Drawing;

namespace AI2D.Objects
{
    public class TextBlock
    {
        private Rectangle _prevRegion;
        private Font _font;
        private Display _display;
        private double _x;
        private double _y;
        private Graphics _genericDC; //Not used for drawing, only measuring.

        double _height = 0;
        public Double Height
        {
            get
            {
                if (_height == 0)
                {
                    _height = _genericDC.MeasureString("void", _font).Height;
                }
                return _height;
            }
        }

        private string _lastTextSizeCheck;
        SizeD _size = null;
        public SizeD Size
        {
            get
            {
                if (_size == null || _text != _lastTextSizeCheck)
                {
                    var fSize = _genericDC.MeasureString(_text, _font);

                    _size = new SizeD(fSize.Width, fSize.Height);
                    _lastTextSizeCheck = _text;
                }
                return _size;
            }
        }

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

                //If we have previously drawn text, then we need to invalidate the entire region which it occupied.
                if (_prevRegion != null)
                {
                    _display.DrawingSurface.Invalidate(_prevRegion);
                }

                //Now that we have used _prevRegion to invaldate the previous region, set it to the new region coords.
                //And invalidate them for the new text.
                var stringSize = _genericDC.MeasureString(_text, _font);
                _prevRegion = new Rectangle((int)_x, (int)_y, (int)stringSize.Width, (int)stringSize.Height);
                _display.DrawingSurface.Invalidate(_prevRegion);
            }
        }

        public TextBlock(Display display, string font, double size, double x, double y)
        {
            _x = x;
            _y = y;
            _display = display;
            _font = new Font(font, (float)size);
            _genericDC = _display.DrawingSurface.CreateGraphics();
        }

        public void Render(Graphics dc)
        {
            using (Font font = new Font("ComicSans", 15F))
            {
                dc.DrawString(_text, font, Brushes.Aqua, (float)_x, (float)_y);
            }
        }
    }
}
