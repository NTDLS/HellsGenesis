using Si.GameEngine.Core.Managers;
using Si.GameEngine.Core.TickControllers._Superclass;
using Si.GameEngine.Sprites;
using Si.GameEngine.Sprites._Superclass;
using Si.Library;
using Si.Library.Types;
using Si.Library.Types.Geometry;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Core.TickControllers
{
    /// <summary>
    /// These are just generic bitmap sprites.
    /// </summary>
    public class GenericSpriteTickController : SpriteTickControllerBase<SpriteGeneric>
    {
        public GenericSpriteTickController(GameEngineCore gameEngine, EngineSpriteManager manager)
            : base(gameEngine, manager)
        {
        }

        public override void ExecuteWorldClockTick(double epoch, SiPoint displacementVector)
        {
            foreach (var particle in Visible())
            {
                particle.ApplyMotion(epoch, displacementVector);
            }
        }

        public SpriteGeneric CreateAt(SpriteBase sprite, SharpDX.Direct2D1.Bitmap bitmap)
        {
            var obj = new SpriteGeneric(GameEngine, sprite.X, sprite.Y, bitmap);
            SpriteManager.Add(obj);
            return obj;
        }

        public SpriteGeneric Create(SharpDX.Direct2D1.Bitmap bitmap)
        {
            var obj = new SpriteGeneric(GameEngine, bitmap);
            SpriteManager.Add(obj);
            return obj;
        }

        public void FragmentBlastOf(SpriteBase sprite)
        {
            var image = sprite.GetImage();
            if (image == null)
            {
                return;
            }

            int fragmentCount = SiRandom.Between(2, 10);

            var fragmentImages = GameEngine.Rendering.GenerateIrregularFragments(image, fragmentCount);

            for (int index = 0; index < fragmentCount; index++)
            {
                var fragment = CreateAt(sprite, fragmentImages[index]);
                //TODO: Can we implement this.
                fragment.CleanupMode = ParticleCleanupMode.FadeToBlack;
                fragment.FadeToBlackReductionAmount = SiRandom.Between(0.001, 0.01);

                fragment.Velocity.Angle.Degrees = SiRandom.Between(0.0, 359.0);

                fragment.Velocity.MaxSpeed = SiRandom.Between(1, 3.5);
                fragment.Velocity.ThrottlePercentage = 1;
                fragment.VectorType = ParticleVectorType.Independent;
            }
        }
    }
}
