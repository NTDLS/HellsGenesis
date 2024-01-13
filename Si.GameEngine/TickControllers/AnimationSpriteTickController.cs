using Si.GameEngine.Engine;
using Si.GameEngine.Managers;
using Si.GameEngine.Sprites;
using Si.GameEngine.TickControllers.BasesAndInterfaces;
using Si.Shared.Types.Geometry;
using System.Drawing;
using static Si.Shared.SiConstants;

namespace Si.GameEngine.Controller
{
    public class AnimationSpriteTickController : SpriteTickControllerBase<SpriteAnimation>
    {
        public AnimationSpriteTickController(EngineCore gameCore, EngineSpriteManager manager)
            : base(gameCore, manager)
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
            animation.LocalX = defaultPosition.CombinedLocation.X;
            animation.LocalY = defaultPosition.CombinedLocation.Y;
            animation.RotationMode = SiRotationMode.Rotate;
            SpriteManager.Add(animation);
        }

        public SpriteAnimation Create(string imageFrames, Size frameSize, int _frameDelayMilliseconds = 10, SpriteAnimation.PlayMode playMode = null)
        {
            SpriteAnimation obj = new SpriteAnimation(GameCore, imageFrames, frameSize, _frameDelayMilliseconds, playMode);
            SpriteManager.Add(obj);
            return obj;
        }
    }
}
