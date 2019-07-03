using AI2D.Engine;
using System.Drawing;
using System.Windows.Forms;

namespace AI2D.Objects
{
    public class TextBlock
    {
        private Rectangle _prevRegion;
        private Font _font;
        private Display _display;
        private int _x;
        private int _y;
        private Graphics _genericDC; //Not used for drawing, only measuring.

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
                _prevRegion = new Rectangle(_x, _y, (int)stringSize.Width, (int)stringSize.Height);
                _display.DrawingSurface.Invalidate(_prevRegion);
            }
        }

        public TextBlock(Display display, string font, float size, int x, int y)
        {
            _x = x;
            _y = y;
            _display = display;
            _font = new Font(font, size);
            _genericDC = _display.DrawingSurface.CreateGraphics();
        }

        public void Render(Graphics dc)
        {
            using (Font font = new Font("ComicSans", 15F))
            {
                dc.DrawString(_text, font, Brushes.Aqua, _x, _y);
            }
        }
    }
}
