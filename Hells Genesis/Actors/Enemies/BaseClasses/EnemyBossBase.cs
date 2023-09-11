using HG.Engine;
using System.IO;

namespace HG.Actors.Enemies.BaseClasses
{
    internal class EnemyBossBase : EnemyBase
    {
        public EnemyBossBase(Core core, int hullHealth, int bountyMultiplier)
            : base(core, hullHealth, bountyMultiplier)
        {
            Velocity.ThrottlePercentage = 1;
            Initialize();
        }

        public override void Explode()
        {
            _explodeSound?.Play();
            _explosionAnimation?.Reset();
            _core.Actors.Animations.CreateAt(_explosionAnimation, this);

            CreateParticlesExplosion();

            base.Explode();
        }
    }
}
