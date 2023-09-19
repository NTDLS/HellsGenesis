using HG.Engine;

namespace HG.Sprites.Enemies.Bosses
{
    internal class _SpriteEnemyBossBase : _SpriteEnemyBase
    {
        public _SpriteEnemyBossBase(EngineCore core, int hullHealth, int bountyMultiplier)
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
