using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Sprites.Enemies.BaseClasses;

namespace NebulaSiege.Game.Sprites.Enemies.Bosses.BaseClasses
{
    /// <summary>
    /// Boss enemies are specialized enemy types, typically have destructible/moving components.
    /// </summary>
    internal class SpriteEnemyBossBase : SpriteEnemyBase
    {
        public SpriteEnemyBossBase(EngineCore core, int hullHealth, int bountyMultiplier)
            : base(core, hullHealth, bountyMultiplier)
        {
            Velocity.ThrottlePercentage = 1;
            Initialize();
        }

        public override void Explode()
        {
            _explodeSound?.Play();
            _explosionAnimation?.Reset();
            _core.Sprites.Animations.InsertAt(_explosionAnimation, this);

            CreateParticlesExplosion();

            base.Explode();
        }
    }
}
