using HG.Engine;

namespace HG.Sprites.Enemies.BaseClasses
{
    internal class EnemyBossBase : EnemyBase
    {
        public EnemyBossBase(EngineCore core, int hullHealth, int bountyMultiplier)
            : base(core, hullHealth, bountyMultiplier)
        {
            Velocity.ThrottlePercentage = 1;
            Initialize();
        }

        public override void Explode()
        {
            _explodeSound?.Play();
            _explosionAnimation?.Reset();
            _core.Actors.Animations.InsertAt(_explosionAnimation, this);

            CreateParticlesExplosion();

            base.Explode();
        }
    }
}
