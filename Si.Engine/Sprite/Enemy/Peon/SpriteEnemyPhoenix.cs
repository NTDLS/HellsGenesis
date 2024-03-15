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
        public SpriteEnemyPhoenix(EngineCore engine)
            : base(engine)
        {
            InitializeSpriteFromMetadata(@"Sprites\Enemy\Peon\Phoenix\Hull.png");

            AddAIController(new HostileEngagement(_engine, this, _engine.Player.Sprite));
            AddAIController(new Taunt(_engine, this, _engine.Player.Sprite));
            //AddAIController(new Meander(_engine, this, _engine.Player.Sprite));

            SetCurrentAIController(AIControllers[typeof(Taunt)]);

            _behaviorChangeThresholdMilliseconds = SiRandom.Between(2000, 10000);
        }

        #region Artificial Intelligence.

        private DateTime _lastBehaviorChangeTime = DateTime.UtcNow;
        private float _behaviorChangeThresholdMilliseconds = 0;

        public override void ApplyIntelligence(float epoch, SiPoint displacementVector)
        {
            base.ApplyIntelligence(epoch, displacementVector);

            if ((DateTime.UtcNow - _lastBehaviorChangeTime).TotalMilliseconds > _behaviorChangeThresholdMilliseconds)
            {
                _lastBehaviorChangeTime = DateTime.UtcNow;
                _behaviorChangeThresholdMilliseconds = SiRandom.Between(2000, 10000);

                /*
                if (SiRandom.PercentChance(1))
                {
                    SetCurrentAIController(AIControllers[typeof(Taunt)]);
                }
                */

                if (SiRandom.PercentChance(5))
                {
                    SetCurrentAIController(AIControllers[typeof(HostileEngagement)]);
                }
            }

            if (IsHostile)
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

            CurrentAIController?.ApplyIntelligence(epoch, displacementVector);
        }

        #endregion
    }
}
