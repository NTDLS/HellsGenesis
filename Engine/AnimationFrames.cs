using System.Drawing;

namespace AI2D.Engine
{
    public class AnimationFrames
    {
        private Bitmap _explodeFrame;
        private Game _game;
        private int _frameCount;
        private int _currentFrame;
        private Size _frameSize;
        private int _rows;
        private int _columns;

        public AnimationFrames(Game game, string image, Size frameSize)
        {
            _game = game;
            _explodeFrame = _game.Actors.GetBitmap(image);
            _frameSize = frameSize;
            _rows = (_explodeFrame.Height / frameSize.Height);
            _columns = (_explodeFrame.Width / frameSize.Width);
            _frameCount = _rows * _columns;
        }

        private int _currentRow = 0;
        private int _currentColumn = 0;
 
        private Bitmap _previousFrame; //Begin super judicious!

        public Bitmap GetReplacmentImage()
        {
            if (_previousFrame != null)
            {
                _previousFrame.Dispose();
                _previousFrame = null;
            }

            if (_currentFrame == _frameCount)
            {
                return null;
            }

            Rectangle cloneRect = new Rectangle(_currentColumn * _frameSize.Width, _currentRow * _frameSize.Height, _frameSize.Width, _frameSize.Height);
            System.Drawing.Imaging.PixelFormat format = _explodeFrame.PixelFormat;

            _previousFrame = _explodeFrame.Clone(cloneRect, format);

            if (++_currentColumn == _columns)
            {
                _currentColumn = 0;
                _currentRow++;
            }

            _currentFrame++;

            return _previousFrame;
        }
    }
}
