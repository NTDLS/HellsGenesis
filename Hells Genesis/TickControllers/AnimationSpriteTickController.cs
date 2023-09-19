using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Managers;
using HG.Sprites;
using HG.TickControllers;
using System.Drawing;

namespace HG.Controller
{
    internal class AnimationSpriteTickController : _SpriteTickControllerBase<SpriteAnimation>
    {
        public AnimationSpriteTickController(EngineCore core, EngineSpriteManager manager)
            : base(core, manager)
        {
        }

        public override void ExecuteWorldClockTick(HgPoint displacementVector)
        {
            foreach (var animation in Visible())
            {
                animation.ApplyMotion(displacementVector);
                animation.AdvanceImage();
            }
        }

        /// <summary>
        /// Creates an animation on top of another sprite.
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="defaultPosition"></param>
        public void InsertAt(SpriteAnimation animation, _SpriteBase defaultPosition)
        {
            lock (SpriteManager.Collection)
            {
                animation.X = defaultPosition.X;
                animation.Y = defaultPosition.Y;
                animation.RotationMode = HgRotationMode.Rotate;
                SpriteManager.Collection.Add(animation);
            }
        }

        public SpriteAnimation Create(string imageFrames, Size frameSize, int _frameDelayMilliseconds = 10, SpriteAnimation.PlayMode playMode = null)
        {
            lock (SpriteManager.Collection)
            {
                SpriteAnimation obj = new SpriteAnimation(Core, imageFrames, frameSize, _frameDelayMilliseconds, playMode);
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }
    }
}
