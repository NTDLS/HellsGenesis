using Si.GameEngine.Core;
using Si.GameEngine.ResourceManagers;
using Si.GameEngine.Sprites;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.TickControllers._Superclass;
using Si.Library.Mathematics.Geometry;
using System.Drawing;

namespace Si.GameEngine.TickControllers
{
    public class AnimationSpriteTickController : SpriteTickControllerBase<SpriteAnimation>
    {
        public AnimationSpriteTickController(GameEngineCore gameEngine, EngineSpriteManager manager)
            : base(gameEngine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector displacementVector)
        {
            foreach (var animation in Visible())
            {
                animation.ApplyMotion(epoch, displacementVector);
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
            animation.Location = defaultPosition.Location.Clone();
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
