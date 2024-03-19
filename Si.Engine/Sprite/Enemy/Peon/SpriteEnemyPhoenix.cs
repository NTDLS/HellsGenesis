using Si.Engine.AI.Logistics;
using Si.Engine.Sprite.Enemy.Peon._Superclass;
using Si.Engine.Sprite.Weapon;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System;
using System.Linq;

namespace Si.Engine.Sprite.Enemy.Peon
{
    internal class SpriteEnemyPhoenix : SpriteEnemyPeonBase
    {
        private DateTime _behaviorChangeTimestamp = DateTime.UtcNow;
        private float _behaviorChangeDelayMilliseconds = 0;

        public SpriteEnemyPhoenix(EngineCore engine)
            : base(engine)
        {
            SetImageAndLoadMetadata(@"Sprites\Enemy\Peon\Phoenix\Hull.png");

            AddAIController(new AILogisticsHostileEngagement(_engine, this, _engine.Player.Sprite));
            AddAIController(new AILogisticsTaunt(_engine, this, _engine.Player.Sprite));
            AddAIController(new AILogisticsMeander(_engine, this, _engine.Player.Sprite));

            SetCurrentAIController<AILogisticsTaunt>();

            _behaviorChangeDelayMilliseconds = SiRandom.Between(2000, 10000);
        }

        public override void ApplyIntelligence(float epoch, SiPoint displacementVector)
        {
            base.ApplyIntelligence(epoch, displacementVector);

            ChangeAIModes();
            FireAtPlayer();
        }

        private void ChangeAIModes()
        {
            if ((DateTime.UtcNow - _behaviorChangeTimestamp).TotalMilliseconds > _behaviorChangeDelayMilliseconds)
            {
                _behaviorChangeTimestamp = DateTime.UtcNow;
                _behaviorChangeDelayMilliseconds = SiRandom.Between(2000, 10000);

                if (SiRandom.PercentChance(10))
                {
                    SetCurrentAIController<AILogisticsTaunt>();
                }
                else if (SiRandom.PercentChance(1))
                {
                    SetCurrentAIController<AILogisticsHostileEngagement>();
                }
            }
        }

        private void FireAtPlayer()
        {
            var playersIAmPointingAt = GetPointingAtOf(_engine.Sprites.AllVisiblePlayers, 2.0f);
            if (playersIAmPointingAt.Any())
            {
                var closestDistance = ClosestDistanceOf(playersIAmPointingAt);

                if (closestDistance < 1000)
                {
                    if (closestDistance > 500 && HasWeaponAndAmmo<WeaponVulcanCannon>())
                    {
                        FireWeapon<WeaponVulcanCannon>();
                    }
                    else if (closestDistance > 0 && HasWeaponAndAmmo<WeaponDualVulcanCannon>())
                    {
                        FireWeapon<WeaponDualVulcanCannon>();
                    }
                }
            }


        }
    }
}
