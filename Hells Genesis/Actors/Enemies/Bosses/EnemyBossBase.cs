using HG.Engine;
using HG.Types;
using System.Drawing;

namespace HG.Actors.Enemies.Bosses
{
    internal class EnemyBossBase : EnemyBase
    {
        public EnemyBossBase(Core core, int hitPoints, int scoreMultiplier)
            : base(core, hitPoints, scoreMultiplier)
        {
            Velocity.ThrottlePercentage = 1;
            Initialize();

            RadarDotSize = new HgPoint<int>(4, 4);
            RadarDotColor = Color.FromArgb(200, 100, 100);

            RadarPositionIndicator = _core.Actors.RadarPositions.Create();
            RadarPositionIndicator.Visable = false;
            RadarPositionText = _core.Actors.TextBlocks.CreateRadarPosition("Consolas", Brushes.Red, 8, new HgPoint<double>());
        }

        public override void Cleanup()
        {
        }
    }
}
