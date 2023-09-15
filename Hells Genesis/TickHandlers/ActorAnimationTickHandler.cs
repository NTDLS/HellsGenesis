using HG.Actors.BaseClasses;
using HG.Actors.Ordinary;
using HG.Controllers;
using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.TickHandlers.Interfaces;
using System.Collections.Generic;
using System.Drawing;

namespace HG.TickHandlers
{
    internal class ActorAnimationTickHandler : IVectoredTickManager
    {
        private readonly EngineCore _core;
        private readonly EngineActorController _controller;

        public List<subType> VisibleOfType<subType>() where subType : ActorAnimation => _controller.VisibleOfType<subType>();
        public List<ActorAnimation> Visible() => _controller.VisibleOfType<ActorAnimation>();
        public List<subType> OfType<subType>() where subType : ActorAnimation => _controller.OfType<subType>();

        public ActorAnimationTickHandler(EngineCore core, EngineActorController manager)
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
                _controller.OfType<ActorAnimation>().ForEach(c => c.QueueForDelete());
            }
        }

        #region Factories.

        /// <summary>
        /// Creates an animation on top of another actor.
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="defaultPosition"></param>
        public void InsertAt(ActorAnimation animation, ActorBase defaultPosition)
        {
            lock (_controller.Collection)
            {
                animation.X = defaultPosition.X;
                animation.Y = defaultPosition.Y;
                animation.RotationMode = HgRotationMode.Rotate;
                _controller.Collection.Add(animation);
            }
        }

        public ActorAnimation Create(string imageFrames, Size frameSize, int _frameDelayMilliseconds = 10, ActorAnimation.PlayMode playMode = null)
        {
            lock (_controller.Collection)
            {
                ActorAnimation obj = new ActorAnimation(_core, imageFrames, frameSize, _frameDelayMilliseconds, playMode);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(ActorAnimation obj)
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
