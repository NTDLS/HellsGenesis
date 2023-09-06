using HG.Engine;

namespace HG.Actors.Enemies.BaseClasses
{
    internal class EnemyBossBase : EnemyBase
    {
        public EnemyBossBase(Core core, int hitPoints, int scoreMultiplier)
            : base(core, hitPoints, scoreMultiplier)
        {
            Velocity.ThrottlePercentage = 1;
            Initialize();
        }
    }
}
