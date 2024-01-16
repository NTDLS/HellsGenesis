using Si.GameEngine.Core.Managers;
using Si.GameEngine.Core.TickControllers._Superclass;
using Si.GameEngine.Sprites;
using Si.GameEngine.Sprites._Superclass;
using Si.Library.Types.Geometry;
using System.Drawing;

namespace Si.GameEngine.Core.TickControllers
{
    public class AnimationSpriteTickController : SpriteTickControllerBase<SpriteAnimation>
    {
        public AnimationSpriteTickController(GameEngineCore gameEngine, EngineSpriteManager manager)
            : base(gameEngine, manager)
        {
        }

        public override void ExecuteWorldClockTick(SiPoint displacementVector)
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
        public void AddAt(SpriteAnimation animation, SpriteBase defaultPosition)
        {
            animation.X = defaultPosition.Location.X;
            animation.Y = defaultPosition.Location.Y;
            SpriteManager.Add(animation);
        }

        public SpriteAnimation Create(string imageFrames, Size frameSize, int _frameDelayMilliseconds = 10, SpriteAnimation.PlayMode playMode = null)
        {
            SpriteAnimation obj = new SpriteAnimation(GameEngine, imageFrames, frameSize, _frameDelayMilliseconds, playMode);
            SpriteManager.Add(obj);
            return obj;
        }
    }
}
