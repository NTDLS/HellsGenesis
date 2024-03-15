using Si.Engine;
using Si.Engine.Sprite.Enemy.Starbase._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;

namespace Si.GameEngine.Sprite.Enemy.Starbase.Garrison
{
    internal class SpriteEnemyStarbaseGarrison : SpriteEnemyStarbase
    {
        // Other Names: Nexus, Forge, Bastion, Citadel, Spire, Stronghold, Enclave, Garrison, Fortress

        public SpriteEnemyStarbaseGarrison(EngineCore engine)
            : base(engine)
        {
            InitializeSpriteFromMetadata(@"Sprites\Enemy\Starbase\Garrison\Hull.png");

            Velocity.ForwardAngle.Degrees = SiRandom.Between(0, 359);
        }

        public override void ApplyMotion(float epoch, SiPoint displacementVector)
        {
            Velocity.ForwardAngle.Degrees += 0.005f;
            base.ApplyMotion(epoch, displacementVector);
        }
    }
}
