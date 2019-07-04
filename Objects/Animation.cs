using AI2D.Engine;
using System;
using System.Drawing;

namespace AI2D.Objects
{
    public class Animation : BaseObject
    {
        private Bitmap _explodeFrame;
        private Game _game;
        private int _frameCount;
        private int _currentFrame = 0;
        private int _currentRow = 0;
        private int _currentColumn = 0;
        private Size _frameSize;
        private int _rows;
        private int _columns;
        private DateTime _lastFrameChange = DateTime.Now.AddSeconds(-60);
        private bool _deleteWhenDonePlaying = false;

        public Animation(Game game, string imageFrames, Size frameSize, bool deleteWhenDonePlaying = true)
            : base(game)
        {
            _game = game;

            _deleteWhenDonePlaying = deleteWhenDonePlaying;
            _explodeFrame = _game.Actors.GetBitmapCached(imageFrames);
            _frameSize = frameSize;
            _rows = (_explodeFrame.Height / frameSize.Height);
            _columns = (_explodeFrame.Width / frameSize.Width);
            _frameCount = _rows * _columns;

            AdvanceImage();
        }

        public void Reset()
        {
            _currentFrame = 0;
            _currentRow = 0;
            _currentColumn = 0;
            _lastFrameChange = DateTime.Now.AddSeconds(-60);
            Visable = true;
            ReadyForDeletion = false;
        }

        public void AdvanceImage()
        {
            if ((DateTime.Now - _lastFrameChange).TotalMilliseconds > 10)
            {
                _lastFrameChange = DateTime.Now;

                if (_currentFrame == _frameCount)
                {
                    if (_deleteWhenDonePlaying)
                    {
                        this.ReadyForDeletion = true;
                    }
                    this.Visable = false;
                    return;
                }

                Rectangle cloneRect = new Rectangle(_currentColumn * _frameSize.Width, _currentRow * _frameSize.Height, _frameSize.Width, _frameSize.Height);
                System.Drawing.Imaging.PixelFormat format = _explodeFrame.PixelFormat;
                var image = _explodeFrame.Clone(cloneRect, format);
                SetImage(image);

                if (++_currentColumn == _columns)
                {
                    _currentColumn = 0;
                    _currentRow++;
                }

                _currentFrame++;
            }
        }
    }
}