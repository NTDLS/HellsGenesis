using HG.Controllers;
using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Sprites;
using HG.Sprites.BaseClasses;
using HG.TickHandlers.Interfaces;
using System.Collections.Generic;
using System.Drawing;

namespace HG.TickHandlers
{
    internal class AnimationSpriteTickHandler : IVectoredTickManager
    {
        private readonly EngineCore _core;
        private readonly EngineSpriteController _controller;

        public List<subType> VisibleOfType<subType>() where subType : SpriteAnimation => _controller.VisibleOfType<subType>();
        public List<SpriteAnimation> Visible() => _controller.VisibleOfType<SpriteAnimation>();
        public List<subType> OfType<subType>() where subType : SpriteAnimation => _controller.OfType<subType>();

        public AnimationSpriteTickHandler(EngineCore core, EngineSpriteController manager)
        {
            _core = core;
            _controller = manager;
        }

        public void ExecuteWorldClockTick(HgPoint displacementVector)
        {
            foreach (var animation in Visible())
            {
                animation.ApplyMotion(displacementVector);
                animation.AdvanceImage();
            }
        }

        public void DeleteAll()
        {
            lock (_controller.Collection)
            {
                _controller.OfType<SpriteAnimation>().ForEach(c => c.QueueForDelete());
            }
        }

        #region Factories.

        /// <summary>
        /// Creates an animation on top of another sprite.
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="defaultPosition"></param>
        public void InsertAt(SpriteAnimation animation, SpriteBase defaultPosition)
        {
            lock (_controller.Collection)
            {
                animation.X = defaultPosition.X;
                animation.Y = defaultPosition.Y;
                animation.RotationMode = HgRotationMode.Rotate;
                _controller.Collection.Add(animation);
            }
        }

        public SpriteAnimation Create(string imageFrames, Size frameSize, int _frameDelayMilliseconds = 10, SpriteAnimation.PlayMode playMode = null)
        {
            lock (_controller.Collection)
            {
                SpriteAnimation obj = new SpriteAnimation(_core, imageFrames, frameSize, _frameDelayMilliseconds, playMode);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(SpriteAnimation obj)
        {
            lock (_controller.Collection)
            {
                obj.Cleanup();
                _controller.Collection.Remove(obj);
            }
        }

        #endregion
    }
}
