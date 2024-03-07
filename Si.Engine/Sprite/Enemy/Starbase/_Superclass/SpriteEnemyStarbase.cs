using Si.Engine.Sprite.Enemy._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.Enemy.Starbase._Superclass
{
    /// <summary>
    /// Base class for "Peon" enemies. These guys are basically all the same in theit functionality and animations.
    /// </summary>
    internal class SpriteEnemyStarbase : SpriteEnemyBase
    {
        public SpriteEnemyStarbase(EngineCore engine)
            : base(engine)
        {
            Velocity.ForwardMomentium = 1;
            Initialize();
        }

        /// <summary>
        /// Moves the sprite based on its thrust/boost (velocity) taking into account the background scroll.
        /// </summary>
        /// <param name="displacementVector"></param>
        public override void ApplyMotion(float epoch, SiPoint displacementVector)
        {
            base.ApplyMotion(epoch, displacementVector);
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }
    }
}
