using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using Si.GameEngine.Sprites._Superclass;
using Si.Library.Mathematics;
using Si.Library.Mathematics.Geometry;
using System;
using System.Drawing;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Sprites
{
    public class SpriteAnimation : SpriteBase
    {
        private readonly SharpDX.Direct2D1.Bitmap _sheetImage;
        private readonly int _frameCount;
        private int _currentFrame = 0;
        private int _currentRow = 0;
        private int _currentColumn = 0;
        private Size _frameSize;
        private readonly int _rows;
        private readonly int _columns;
        private readonly int _frameDelayMilliseconds = 10;
        private DateTime _lastFrameChange = DateTime.Now.AddSeconds(-60);
        private readonly PlayMode _playMode;

        public class PlayMode
        {
            private SiAnimationReplayMode _replay;

            public SiAnimationReplayMode Replay
            {
                get
                {
                    return _replay;
                }
                set
                {
                    if (value == SiAnimationReplayMode.LoopedPlay)
                    {
                        DeleteSpriteAfterPlay = false;
                    }
                    _replay = value;
                }
            }

            public TimeSpan ReplayDelay;
            public bool DeleteSpriteAfterPlay;
        }

        public SpriteAnimation(GameEngineCore gameEngine, string spriteSheetFileName, Size? frameSize, int frameDelayMilliseconds = 10, PlayMode playMode = null)
            : base(gameEngine)
        {
            _playMode = playMode;

            if (_playMode == null)
            {
                _playMode = new PlayMode()
                {
                    DeleteSpriteAfterPlay = true,
                    Replay = SiAnimationReplayMode.SinglePlay,
                    ReplayDelay = new TimeSpan(0, 0, 0, 0, frameDelayMilliseconds)
                };
            }

            _frameDelayMilliseconds = frameDelayMilliseconds;
            _sheetImage = _gameEngine.Assets.GetBitmap(spriteSheetFileName);

            _frameSize = (Size)frameSize;
            _rows = (int)(_sheetImage.Size.Height / ((Size)frameSize).Height);
            _columns = (int)(_sheetImage.Size.Width / ((Size)frameSize).Width);
            _frameCount = _rows * _columns;

            Location = new SiPoint(0, 0);
            Velocity = new SiVelocity();

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

        public override void Render(RenderTarget renderTarget)
        {
            var sourceRect = new RawRectangleF(
                _currentColumn * _frameSize.Width,
                _currentRow * _frameSize.Height,
                _currentColumn * _frameSize.Width + _frameSize.Width,
                _currentRow * _frameSize.Height + _frameSize.Height);

            _gameEngine.Rendering.DrawBitmapAt(
                renderTarget,
                _sheetImage,
                (RenderLocation.X) - _frameSize.Width / 2.0f,
                (RenderLocation.Y) - _frameSize.Height / 2.0f,
                Velocity.Angle.Radians,
                sourceRect,
                new Size2F(_frameSize.Width, _frameSize.Height)
            );
        }

        public void AdvanceImage()
        {
            if ((DateTime.Now - _lastFrameChange).TotalMilliseconds > _frameDelayMilliseconds)
            {
                _lastFrameChange = DateTime.Now;

                if (++_currentColumn == _columns)
                {
                    _currentColumn = 0;
                    _currentRow++;
                }

                _currentFrame++;

                if (_currentFrame == _frameCount)
                {
                    if (_playMode.DeleteSpriteAfterPlay)
                    {
                        QueueForDelete();
                        return;
                    }

                    if (_playMode.Replay == SiAnimationReplayMode.LoopedPlay)
                    {
                        _currentFrame = 0;
                        _currentColumn = 0;
                        _currentRow = 0;
                    }
                    return;
                }
            }
        }
    }
}