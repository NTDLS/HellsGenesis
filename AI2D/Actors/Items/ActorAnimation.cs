using AI2D.Engine;
using AI2D.Types;
using System;
using System.Drawing;

namespace AI2D.Actors.Items
{
    internal class ActorAnimation : ActorBase
    {
        private readonly Bitmap _frameImage;
        private readonly int _frameCount;
        private int _currentFrame = 0;
        private int _currentRow = 0;
        private int _currentColumn = 0;
        private Size _frameSize;
        private readonly int _rows;
        private readonly int _columns;
        private readonly int _frameDelayMilliseconds = 10;
        private DateTime _lastFrameChange = DateTime.Now.AddSeconds(-60);
        private readonly string _imageName; //Debugging.
        private readonly PlayMode _playMode;

        public enum ReplayMode
        {
            SinglePlay,
            LoopedPlay,
            StillFrame
        };

        internal class PlayMode
        {
            private ReplayMode _replay;

            public ReplayMode Replay
            {
                get
                {
                    return _replay;
                }
                set
                {
                    if (value == ReplayMode.LoopedPlay)
                    {
                        DeleteActorAfterPlay = false;
                    }
                    _replay = value;
                }
            }

            public TimeSpan ReplayDelay;
            public bool DeleteActorAfterPlay;
        }

        public ActorAnimation(Core core, string imageFrames, Size? frameSize, int frameDelayMilliseconds = 10, PlayMode playMode = null)
            : base(core)
        {
            _playMode = playMode;

            if (_playMode == null)
            {
                _playMode = new PlayMode()
                {
                    DeleteActorAfterPlay = true,
                    Replay = ReplayMode.SinglePlay,
                    ReplayDelay = new TimeSpan(0, 0, 0, 0, frameDelayMilliseconds)
                };
            }

            _imageName = imageFrames;
            _frameDelayMilliseconds = frameDelayMilliseconds;
            _frameImage = _core.Imaging.Get(imageFrames);

            if (frameSize == null)
            {
                if (playMode.Replay == ReplayMode.StillFrame)
                {
                    frameSize = _frameImage.Size;
                }
                else
                {
                    throw new Exception("The anamation frame size must be set unless it is a still shot.");
                }
            }

            _frameSize = (Size)frameSize;
            _rows = _frameImage.Height / ((Size)frameSize).Height;
            _columns = _frameImage.Width / ((Size)frameSize).Width;
            _frameCount = _rows * _columns;

            Location = new Point<double>(0, 0);
            Velocity = new Velocity<double>();

            AdvanceImage();
        }

        public void Reset()
        {
            _currentFrame = 0;
            _currentRow = 0;
            _currentColumn = 0;
            _lastFrameChange = DateTime.Now.AddSeconds(-60);
            Visable = true;
        }

        public void AdvanceImage()
        {
            if (_playMode.Replay == ReplayMode.StillFrame)
            {
                if (GetImage() != null)
                {
                    return;
                }

                Rectangle cloneRect = new Rectangle(_currentColumn * _frameSize.Width, _currentRow * _frameSize.Height, _frameSize.Width, _frameSize.Height);
                System.Drawing.Imaging.PixelFormat format = _frameImage.PixelFormat;
                SetImage(_frameImage.Clone(cloneRect, format));
                return;
            }

            if ((DateTime.Now - _lastFrameChange).TotalMilliseconds > _frameDelayMilliseconds)
            {
                _lastFrameChange = DateTime.Now;

                if (_currentFrame == _frameCount)
                {
                    if (_playMode.DeleteActorAfterPlay)
                    {
                        QueueForDelete(); ;
                        return;
                    }

                    if (_playMode.Replay == ReplayMode.LoopedPlay)
                    {
                        Reset();
                        _lastFrameChange = DateTime.Now.AddMilliseconds(_playMode.ReplayDelay.TotalMilliseconds);
                    }
                    return;
                }

                Rectangle cloneRect = new Rectangle(_currentColumn * _frameSize.Width, _currentRow * _frameSize.Height, _frameSize.Width, _frameSize.Height);
                System.Drawing.Imaging.PixelFormat format = _frameImage.PixelFormat;
                SetImage(_frameImage.Clone(cloneRect, format));

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