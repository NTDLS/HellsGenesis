using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using Si.Engine.Sprite.SupportingClasses.Metadata;
using Si.Library.Mathematics;
using System;
using System.Drawing;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite
{
    public class SpriteAnimation : SpriteMinimalBitmap
    {
        private SharpDX.Direct2D1.Bitmap _sheetImage;
        private int _frameCount;
        private int _currentFrame = 0;
        private int _currentRow = 0;
        private int _currentColumn = 0;
        private int _rows;
        private int _columns;
        private int _frameDelayMilliseconds;
        private DateTime _lastFrameChange = DateTime.Now.AddSeconds(-60);
        private PlayMode _playMode;

        private struct PlayMode
        {
            private SiAnimationReplayMode _replay;

            public SiAnimationReplayMode ReplyMode
            {
                get
                {
                    return _replay;
                }
                set
                {
                    if (value == SiAnimationReplayMode.Infinite)
                    {
                        DeleteSpriteAfterPlay = false;
                    }
                    _replay = value;
                }
            }

            public TimeSpan ReplayDelay;
            public bool DeleteSpriteAfterPlay;
        }

        public SpriteAnimation(EngineCore engine, string spriteSheetFileName)
            : base(engine)
        {
            Location = new SiVector();

            var metadata = _engine.Assets.GetMetaData<SpriteAnimationMetadata>(spriteSheetFileName);

            Speed = metadata.Speed;
            Throttle = metadata.Throttle;
            MaxThrottle = metadata.MaxThrottle;

            SetImage(spriteSheetFileName);
            FramesPerSecond = metadata.FramesPerSecond;
            SetSize(new Size(metadata.FrameWidth, metadata.FrameHeight));

            _playMode = new PlayMode
            {
                DeleteSpriteAfterPlay = metadata.DeleteAfterPlay,
                ReplyMode = metadata.ReplyMode,
                ReplayDelay = new TimeSpan(0, 0, 0, 0, metadata.ReplayDelayMilliseconds)
            };

            AdvanceImage();
        }

        public float FramesPerSecond
        {
            set => _frameDelayMilliseconds = (int)((1.0f / value) * 1000.0f);
            get => (int)(1.0f / (_frameDelayMilliseconds / 1000.0f));
        }

        /// <summary>
        /// We want to get the entire animation sheet and reserve the base.image for the individual slices set by AdvanceImage().
        /// </summary>
        /// <param name="imagePath"></param>
        public new void SetImage(string imagePath)
        {
            _sheetImage = _engine.Assets.GetBitmap(imagePath);
        }

        public new void SetSize(Size frameSize)
        {
            base.SetSize(frameSize);

            _rows = (int)(_sheetImage.Size.Height / ((Size)frameSize).Height);
            _columns = (int)(_sheetImage.Size.Width / ((Size)frameSize).Width);
            _frameCount = _rows * _columns;
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
                _currentColumn * Size.Width,
                _currentRow * Size.Height,
                _currentColumn * Size.Width + Size.Width,
                _currentRow * Size.Height + Size.Height);

            _engine.Rendering.DrawBitmap(
                renderTarget,
                _sheetImage,
                RenderLocation.X - Size.Width / 2.0f,
                RenderLocation.Y - Size.Height / 2.0f,
                Orientation.RadiansSigned,
                sourceRect,
                new Size2F(Size.Width, Size.Height)
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

                    if (_playMode.ReplyMode == SiAnimationReplayMode.Infinite)
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