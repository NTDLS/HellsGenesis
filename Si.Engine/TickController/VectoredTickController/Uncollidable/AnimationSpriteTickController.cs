using Si.Engine;
using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.Sprite._Superclass._SpriteBase;
using Si.Engine.TickController._Superclass;
using Si.Library;
using Si.Library.Mathematics;
using System.IO;

namespace Si.GameEngine.TickController.VectoredTickController.Uncollidable
{
    public class AnimationSpriteTickController : VectoredTickControllerBase<SpriteAnimation>
    {
        public AnimationSpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
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
        public void Insert(SpriteAnimation animation, SpriteBase defaultPosition)
        {
            animation.Location = defaultPosition.Location.Clone();
            SpriteManager.Add(animation);
        }

        /*
        public SpriteAnimation Add(string imageFrames)
        {
            SpriteAnimation obj = new SpriteAnimation(Engine, imageFrames);
            SpriteManager.Add(obj);
            return obj;
        }
        */

        /// <summary>
        /// Small explosion for a objecting hitting another.
        /// </summary>
        /// <param name="positionOf"></param>
        public void AddRandomHitExplosionAt(SpriteBase positionOf)
        {
            if (SiRandom.ChanceIn(1, 5))
            {
                const string assetPath = @"Sprites\Animation\Explode\Hit Explosion 66x66";
                int assetCount = 2;
                int selectedAssetIndex = SiRandom.Between(0, assetCount - 1);

                var animation = Add(Path.Combine(assetPath, $"{selectedAssetIndex}.png"));
                animation.Location = positionOf.Location.Clone();
            }
            else
            {
                const string assetPath = @"Sprites\Animation\Explode\Hit Explosion 22x22";
                int assetCount = 4;
                int selectedAssetIndex = SiRandom.Between(0, assetCount - 1);

                var animation = Add(Path.Combine(assetPath, $"{selectedAssetIndex}.png"));
                animation.Location = positionOf.Location.Clone();
            }
        }

        /// <summary>
        /// Fairly large firey explosion.
        /// </summary>
        /// <param name="PositionOf"></param>
        public void AddRandomExplosionAt(SpriteBase PositionOf)
        {
            const string assetPath = @"Sprites\Animation\Explode\Explosion 256x256\";
            int assetCount = 6;
            int selectedAssetIndex = SiRandom.Between(0, assetCount - 1);

            var animation = Add(Path.Combine(assetPath, $"{selectedAssetIndex}.png"));
            animation.Location = PositionOf.Location.Clone();
        }
    }
}
