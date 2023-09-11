using HG.Engine;

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
    }
}
