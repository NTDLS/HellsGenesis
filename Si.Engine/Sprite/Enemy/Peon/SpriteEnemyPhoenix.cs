using Si.Engine;
using Si.Engine.AI.Logistics;
using Si.GameEngine.Loudout;
using Si.GameEngine.Sprite.Enemy.Peon._Superclass;
using Si.GameEngine.Sprite.Weapon;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System;
using System.Linq;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Sprite.Enemy.Peon
{
    internal class SpriteEnemyPhoenix : SpriteEnemyPeonBase
    {
        public const int hullHealth = 10;
        public const int bountyMultiplier = 15;

        public SpriteEnemyPhoenix(EngineCore engine)
            : base(engine)
        {
            ShipClass = SiEnemyClass.Phoenix;
            SetImage(@$"Graphics\Enemy\Peon\{ShipClass}\Hull.png");

            //Load the loadout from file or create a new one if it does not exist.
            EnemyShipLoadout loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new EnemyShipLoadout(ShipClass)
                {
                    Description = "→ Phoenix ←\n"
                       + "TODO: Add a description\n",
                    Speed = 3.5f,
                    Boost = 1.5f,
                    HullHealth = 20,
                    ShieldHealth = 10,
                    Bounty = 10
                };

                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 5000));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponDualVulcanCannon), 2500));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 42));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponThunderstrikeMissile), 16));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);

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
