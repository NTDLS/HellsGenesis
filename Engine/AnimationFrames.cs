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

        public AnimationFrames(Game game, string image, Size frameSize)
        {
            _game = game;
            _explodeFrame = new Bitmap(Image.FromFile(image));
            _frameSize = frameSize;
            _frameCount = _explodeFrame.Width / frameSize.Width;
        }

        public Bitmap GetReplacmentImage()
        {
            if (_currentFrame == _frameCount)
            {
                return null;
            }
            Rectangle cloneRect = new Rectangle(_currentFrame++ * _frameSize.Width, 0, _frameSize.Width, _frameSize.Height);
            System.Drawing.Imaging.PixelFormat format = _explodeFrame.PixelFormat;
            return _explodeFrame.Clone(cloneRect, format);
        }
    }
}
