using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AI2D.Engine
{
    public class Display
    {
        private Size _visibleSize;
        public Size VisibleSize
        {
            get
            {
                return _visibleSize;
            }
        }

        private Control _drawingSurface;
        public Control DrawingSurface
        {
            get
            {
                return _drawingSurface;
            }
        }

        public Display(Control drawingSurface, Size visibleSize)
        {
            _drawingSurface = drawingSurface;
            _visibleSize = visibleSize;
        }
    }
}
