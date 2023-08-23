using HG.Actors.Objects;
using HG.Engine;
using HG.Engine.Managers;
using HG.Types;
using System.Drawing;

namespace HG.Actors.Factories
{
    internal class EngineActorAnimationFactory
    {
        private readonly Core _core;
        private readonly EngineActorManager _manager;

        public EngineActorAnimationFactory(Core core, EngineActorManager manager)
        {
            _core = core;
            _manager = manager;
        }

        public void DeleteAll()
        {
            lock (_manager.Collection)
            {
                _manager.OfType<ActorAnimation>().ForEach(c => c.QueueForDelete());
            }
        }

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
                animation.RotationMode = HGRotationMode.Clip; //Much less expensive. Use this or NONE if you can.
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
    }
}
