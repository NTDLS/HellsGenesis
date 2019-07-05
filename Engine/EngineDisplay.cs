using AI2D.Types;
using System.Drawing;
using System.Windows.Forms;

namespace AI2D.Engine
{
    public class EngineDisplay
    {
        public PointD BackgroundOffset { get; set; } = new PointD(); //Offset of background, all cals must take into account.
        public FrameCounter FrameCounter { get; set; } = new FrameCounter();
        public RectangleF VisibleBounds { get; private set; }

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

        public EngineDisplay(Control drawingSurface, Size visibleSize)
        {
            _drawingSurface = drawingSurface;
            _visibleSize = visibleSize;
            VisibleBounds = new RectangleF(0, 0, visibleSize.Width, visibleSize.Height);
        }
    }
}
