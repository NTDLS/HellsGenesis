using Si.Engine.AI.Logistics;
using Si.Engine.Loudouts;
using Si.Engine.Sprites.Enemies.Peons._Superclass;
using Si.Engine.Sprites.Weapons;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprites.Enemies.Debug
{
    /// <summary>
    /// Debugging enemy unit - a scary sight to see.
    /// </summary>
    internal class SpriteEnemyDebug : SpriteEnemyPeonBase
    {
        public const int hullHealth = 10;
        public const int bountyMultiplier = 15;

        public SpriteEnemyDebug(EngineCore engine)
            : base(engine, hullHealth, bountyMultiplier)
        {
            ShipClass = SiEnemyClass.Debug;
            SetImage(@$"Graphics\Enemy\Debug\{ShipClass}\Hull.png");

            //Load the loadout from file or create a new one if it does not exist.
            EnemyShipLoadout loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new EnemyShipLoadout(ShipClass)
                {
                    Description = "→ Debug ←\n"
                       + "Easily the scariest enemy in the universe.\n"
                       + "When this badboy is spotted, s**t has already hit the proverbial fan.\n",
                    Speed = 3.5f,
                    Boost = 1.5f,
                    HullHealth = 20,
                    ShieldHealth = 10,
                };

                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 5000));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponDualVulcanCannon), 5000));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 42));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponThunderstrikeMissile), 16));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);

            AddAIController(new HostileEngagement(_engine, this, _engine.Player.Sprite));
            AddAIController(new Taunt(_engine, this, _engine.Player.Sprite));
            AddAIController(new Meander(_engine, this, _engine.Player.Sprite));

            //if (SiRandom.FlipCoin())
            //{
            SetCurrentAIController(AIControllers[typeof(Taunt)]);
            //}
            //else
            //{
            //    SetDefaultAIController(AIControllers[typeof(Meander)]);
            //}

            _behaviorChangeThresholdMilliseconds = SiRandom.Between(2000, 10000);

            SetCurrentAIController(AIControllers[typeof(Taunt)]);
        }

        #region Artificial Intelligence.

        private DateTime _lastBehaviorChangeTime = DateTime.Now;
        private float _behaviorChangeThresholdMilliseconds = 0;

        public override void ApplyIntelligence(float epoch, SiPoint displacementVector)
        {
            float distanceToPlayer = SiPoint.DistanceTo(this, _engine.Player.Sprite);

            base.ApplyIntelligence(epoch, displacementVector);

            if ((DateTime.Now - _lastBehaviorChangeTime).TotalMilliseconds > _behaviorChangeThresholdMilliseconds)
            {
                _behaviorChangeThresholdMilliseconds = SiRandom.Between(2000, 10000);

                /*
                if (SiRandom.ChanceIn(2))
                {
                    SetDefaultAIController(AIControllers[typeof(HostileEngagement)]);
                }
                if (SiRandom.ChanceIn(2))
                {
                */
                SetCurrentAIController(AIControllers[typeof(Taunt)]);
                /*
                }
                else if (SiRandom.ChanceIn(2))
                {
                    SetDefaultAIController(AIControllers[typeof(Meander)]);
                }
                */
            }

            if (IsHostile)
            {
                if (distanceToPlayer < 1000)
                {
                    if (distanceToPlayer > 500 && HasWeaponAndAmmo<WeaponDualVulcanCannon>())
                    {
                        bool isPointingAtPlayer = IsPointingAt(_engine.Player.Sprite, 2.0f);
                        if (isPointingAtPlayer)
                        {
                            FireWeapon<WeaponDualVulcanCannon>();
                        }
                    }
                    else if (distanceToPlayer > 0 && HasWeaponAndAmmo<WeaponVulcanCannon>())
                    {
                        bool isPointingAtPlayer = IsPointingAt(_engine.Player.Sprite, 2.0f);
                        if (isPointingAtPlayer)
                        {
                            FireWeapon<WeaponVulcanCannon>();
                        }
                    }
                }
            }

            CurrentAIController?.ApplyIntelligence(epoch, displacementVector);
        }

        #endregion
    }
}
