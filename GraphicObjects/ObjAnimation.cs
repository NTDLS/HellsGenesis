using AI2D.Engine;
using System;
using System.Drawing;

namespace AI2D.GraphicObjects
{
    public class ObjAnimation : BaseGraphicObject
    {
        private Bitmap _explodeFrame;
        private int _frameCount;
        private int _currentFrame = 0;
        private int _currentRow = 0;
        private int _currentColumn = 0;
        private Size _frameSize;
        private int _rows;
        private int _columns;
        private int _frameDelayMiliseconds = 10;
        private DateTime _lastFrameChange = DateTime.Now.AddSeconds(-60);
        private string _imageName; //Debugging.
        private PlayMode _playMode;

        public enum ReplayMode
        {
             SinglePlay,
             LoopedPlay
        };

        public class PlayMode
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

        public ObjAnimation(Core core, string imageFrames, Size frameSize,  int frameDelayMiliseconds = 10, PlayMode playMode = null)
            : base(core)
        {
            _playMode = playMode;

            if (_playMode == null)
            {
                _playMode = new PlayMode()
                {
                    DeleteActorAfterPlay = true,
                    Replay = ReplayMode.SinglePlay,
                    ReplayDelay = TimeSpan.Zero
                };
            }

            _imageName = imageFrames;
            _frameDelayMiliseconds = frameDelayMiliseconds;
            _explodeFrame = _core.Actors.GetBitmapCached(imageFrames);
            _frameSize = frameSize;
            _rows = (_explodeFrame.Height / frameSize.Height);
            _columns = (_explodeFrame.Width / frameSize.Width);
            _frameCount = _rows * _columns;

            Location = new Types.PointD(0, 0);
            Velocity = new Types.VelocityD();

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
            if ((DateTime.Now - _lastFrameChange).TotalMilliseconds > _frameDelayMiliseconds)
            {
                _lastFrameChange = DateTime.Now;

                if (_currentFrame == _frameCount)
                {
                    if (_playMode.DeleteActorAfterPlay)
                    {
                        this.ReadyForDeletion = true;
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