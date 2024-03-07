using Si.Engine;
using Si.GameEngine.Sprite.Enemy._Superclass;

namespace Si.GameEngine.Sprite.Enemy.Boss._Superclass
{
    /// <summary>
    /// Boss enemies are specialized enemy types, typically have destructible/moving components.
    /// </summary>
    internal class SpriteEnemyBossBase : SpriteEnemyBase
    {
        public SpriteEnemyBossBase(EngineCore engine)
            : base(engine)
        {
            Velocity.ForwardMomentium = 1;
            Initialize();
        }

        public override void Explode()
        {
            _engine.Audio.PlayRandomExplosion();
            _engine.Sprites.Animations.AddRandomExplosionAt(this);

            CreateParticlesExplosion();

            base.Explode();
        }
    }
}
