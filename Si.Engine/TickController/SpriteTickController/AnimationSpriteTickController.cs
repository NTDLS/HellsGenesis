using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.Sprite._Superclass;
using Si.Engine.TickController._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System.Drawing;
using System.IO;

namespace Si.Engine.TickController.SpriteTickController
{
    public class AnimationSpriteTickController : SpriteTickControllerBase<SpriteAnimation>
    {
        public AnimationSpriteTickController(EngineCore engine, EngineSpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiPoint displacementVector)
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
            SpriteAnimation obj = new SpriteAnimation(Engine, imageFrames, frameSize, _frameDelayMilliseconds, playMode);
            SpriteManager.Add(obj);
            return obj;
        }

        /// <summary>
        /// Small explosion for a objecting hitting another.
        /// </summary>
        /// <param name="defaultPosition"></param>
        public void AddRandomHitExplosionAt(SpriteBase defaultPosition)
        {
            const string assetPath = @"Graphics\Animation\Explode\Hit Explosion 22x22";
            int assetCount = 2;
            int selectedAssetIndex = SiRandom.Between(0, assetCount - 1);

            var animation = Create(Path.Combine(assetPath, $"{selectedAssetIndex}.png"), new Size(22, 22));
            animation.Location = defaultPosition.Location.Clone();
        }

        /// <summary>
        /// Fairly large firey explosion.
        /// </summary>
        /// <param name="defaultPosition"></param>
        public void AddRandomExplosionAt(SpriteBase defaultPosition)
        {
            const string assetPath = @"Graphics\Animation\Explode\Explosion 256x256\";
            int assetCount = 6;
            int selectedAssetIndex = SiRandom.Between(0, assetCount - 1);

            var animation = Create(Path.Combine(assetPath, $"{selectedAssetIndex}.png"), new Size(256, 256));
            animation.Location = defaultPosition.Location.Clone();
        }
    }
}
