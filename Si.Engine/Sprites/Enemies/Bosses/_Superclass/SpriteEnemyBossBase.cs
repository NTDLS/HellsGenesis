using Si.Engine.Sprites.Enemies._Superclass;

namespace Si.Engine.Sprites.Enemies.Bosses._Superclass
{
    /// <summary>
    /// Boss enemies are specialized enemy types, typically have destructible/moving components.
    /// </summary>
    internal class SpriteEnemyBossBase : SpriteEnemyBase
    {
        public SpriteEnemyBossBase(EngineCore engine, int hullHealth, int bountyMultiplier)
            : base(engine, hullHealth, bountyMultiplier)
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
