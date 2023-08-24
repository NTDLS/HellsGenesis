using HG.Actors;
using HG.Engine;
using HG.Engine.Managers;
using HG.TickManagers.Interfaces;
using HG.Types;
using System.Collections.Generic;
using System.Drawing;

namespace HG.TickManagers
{
    internal class ActorAnimationManager : IVectoredTickManager
    {
        private readonly Core _core;
        private readonly EngineActorManager _manager;

        public List<subType> VisibleOfType<subType>() where subType : ActorAnimation => _manager.VisibleOfType<subType>();
        public List<ActorAnimation> VisibleOfType() => _manager.VisibleOfType<ActorAnimation>();
        public List<subType> OfType<subType>() where subType : ActorAnimation => _manager.OfType<subType>();

        public ActorAnimationManager(Core core, EngineActorManager manager)
        {
            _core = core;
            _manager = manager;
        }

        public void ExecuteWorldClockTick(HgPoint<double> displacementVector)
        {
            foreach (var animation in _manager.VisibleOfType<ActorAnimation>())
            {
                animation.ApplyMotion(displacementVector);
                animation.AdvanceImage();
            }
        }

        public void DeleteAll()
        {
            lock (_manager.Collection)
            {
                _manager.OfType<ActorAnimation>().ForEach(c => c.QueueForDelete());
            }
        }

        #region Factories.

        /// <summary>
        /// Creates an animation on top of another actor.
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="defaultPosition"></param>
        public void CreateAt(ActorAnimation animation, ActorBase defaultPosition)
        {
            lock (_manager.Collection)
            {
                animation.X = defaultPosition.X;
                animation.Y = defaultPosition.Y;
                animation.RotationMode = HgRotationMode.Clip; //Much less expensive. Use this or NONE if you can.
                _manager.Collection.Add(animation);
            }
        }

        public ActorAnimation Create(string imageFrames, Size frameSize, int _frameDelayMilliseconds = 10, ActorAnimation.PlayMode playMode = null)
        {
            lock (_manager.Collection)
            {
                ActorAnimation obj = new ActorAnimation(_core, imageFrames, frameSize, _frameDelayMilliseconds, playMode);
                _manager.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(ActorAnimation obj)
        {
            lock (_manager.Collection)
            {
                obj.Cleanup();
                _manager.Collection.Remove(obj);
            }
        }

        #endregion
    }
}
