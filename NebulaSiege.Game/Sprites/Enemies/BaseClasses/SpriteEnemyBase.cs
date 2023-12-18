using NebulaSiege.Game.AI;
using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Engine.Types.Geometry;
using NebulaSiege.Game.Loudouts;
using NebulaSiege.Game.Managers;
using NebulaSiege.Game.Sprites.PowerUp;
using NebulaSiege.Game.Sprites.PowerUp.BaseClasses;
using NebulaSiege.Game.Utility;
using NebulaSiege.Game.Utility.ExtensionMethods;
using NebulaSiege.Game.Weapons.BaseClasses;
using NebulaSiege.Game.Weapons.Munitions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NebulaSiege.Game.Sprites.Enemies.BaseClasses
{
    /// <summary>
    /// The enemy base is a sub-class of the ship base. It is used by Peon and Boss enemies.
    /// </summary>
    internal class SpriteEnemyBase : _SpriteShipBase
    {
        public HgEnemyClass ShipClass { get; set; }
        public EnemyShipLoadout Loadout { get; set; }
        public IAIController CurrentAIController { get; set; }
        public Dictionary<Type, IAIController> AIControllers { get; private set; } = new();
        public int CollisionDamage { get; set; } = 25;
        public int BountyWorth { get; private set; } = 25;
        public bool IsHostile { get; set; } = true;
        public List<WeaponBase> Weapons { get; private set; } = new();


        public SpriteEnemyBase(EngineCore core, int hullHealth, int bountyMultiplier)
                : base(core)
        {
            Velocity.ThrottlePercentage = 1;
            Initialize();

            SetHullHealth(hullHealth);
            BountyWorth = HullHealth * bountyMultiplier;

            RadarPositionIndicator = _core.Sprites.RadarPositions.Create();
            RadarPositionIndicator.Visable = false;
            RadarPositionText = _core.Sprites.TextBlocks.CreateRadarPosition(
                core.Rendering.TextFormats.RadarPositionIndicator,
                core.Rendering.Materials.Brushes.Red, new NsPoint());
        }

        public virtual void BeforeCreate() { }

        public virtual void AfterCreate() { }

        public override void RotationChanged() => PositionChanged();

        public override void Explode()
        {
            _core.Player.Sprite.Bounty += BountyWorth;

            if (HgRandom.PercentChance(5))
            {
                int random = HgRandom.Between(0, 4);

                SpritePowerUpBase powerUp = null;

                switch (random)
                {
                    case 0:
                        powerUp = new SpritePowerUpAmmo(_core);
                        break;
                    case 1:
                        powerUp = new SpritePowerUpBoost(_core);
                        break;
                    case 2:
                        powerUp = new SpritePowerUpBounty(_core);
                        break;
                    case 3:
                        powerUp = new SpritePowerUpRepair(_core);
                        break;
                    case 4:
                        powerUp = new SpritePowerUpSheild(_core);
                        break;
                }

                if (powerUp != null)
                {
                    powerUp.Location = Location;
                    _core.Sprites.Powerups.Insert(powerUp);
                }
            }
            base.Explode();
        }

        public string GetLoadoutHelpText()
        {
            string weapons = string.Empty;
            foreach (var weapon in Loadout.Weapons)
            {
                var weaponName = NsReflection.GetStaticPropertyValue(weapon.Type, "Name");
                weapons += $"{weaponName} x{weapon.MunitionCount}\n{new string(' ', 20)}";
            }

            string result = $"          Name : {Loadout.Name}\n";
            result += $"       Weapons : {weapons.Trim()}\n";
            result += $"       Sheilds : {Loadout.ShieldHealth:n0}\n";
            result += $" Hull Strength : {Loadout.HullHealth:n0}\n";
            result += $"     Max Speed : {Loadout.MaxSpeed:n1}\n";
            result += $"    Warp Drive : {Loadout.MaxBoost:n1}\n";
            result += $"\n{Loadout.Description}";

            return result;
        }

        public EnemyShipLoadout LoadLoadoutFromFile(HgEnemyClass shipClass)
        {
            EnemyShipLoadout loadout = null;

            var loadoutText = EngineAssetManager.GetUserText($"Enemy.{shipClass}.loadout.json");

            try
            {
                if (string.IsNullOrWhiteSpace(loadoutText))
                {
                    loadout = JsonConvert.DeserializeObject<EnemyShipLoadout>(loadoutText);
                }
            }
            catch
            {
                loadout = null;
            }

            return loadout;
        }

        public void SaveLoadoutToFile(EnemyShipLoadout loadout)
        {
            var serializedText = JsonConvert.SerializeObject(loadout, Formatting.Indented);
            EngineAssetManager.PutUserText($"Enemy.{loadout.Class}.loadout.json", serializedText);
        }

        public void ResetLoadout(EnemyShipLoadout loadout)
        {
            Loadout = loadout;
            Reset();
        }

        public void Reset()
        {
            Velocity.MaxSpeed = Loadout.MaxSpeed;
            Velocity.MaxBoost = Loadout.MaxBoost;

            SetHullHealth(Loadout.HullHealth);
            SetShieldHealth(Loadout.ShieldHealth);

            Weapons.Clear();
            foreach (var weapon in Loadout.Weapons)
            {
                AddWeapon(weapon.Type, weapon.MunitionCount);
            }
        }

        public override bool TryMunitionHit(NsPoint displacementVector, MunitionBase munition, NsPoint hitTestPosition)
        {
            if (munition.FiredFromType == HgFiredFromType.Player)
            {
                if (Intersects(hitTestPosition))
                {
                    Hit(munition);
                    if (HullHealth <= 0)
                    {
                        Explode();
                    }
                    return true;
                }
            }
            return false;
        }

        public override void ApplyMotion(NsPoint displacementVector)
        {
            if (X < -_core.Settings.EnemySceneDistanceLimit
                || X >= _core.Display.NatrualScreenSize.Width + _core.Settings.EnemySceneDistanceLimit
                || Y < -_core.Settings.EnemySceneDistanceLimit
                || Y >= _core.Display.NatrualScreenSize.Height + _core.Settings.EnemySceneDistanceLimit)
            {
                QueueForDelete();
                return;
            }

            //When an enemy had boost available, it will use it.
            if (Velocity.AvailableBoost > 0)
            {
                if (Velocity.BoostPercentage < 1.0) //Ramp up the boost until it is at 100%
                {
                    Velocity.BoostPercentage += _core.Settings.EnemyThrustRampUp;
                }
                Velocity.AvailableBoost -= Velocity.MaxBoost * Velocity.BoostPercentage; //Consume boost.

                if (Velocity.AvailableBoost < 0) //Sanity check available boost.
                {
                    Velocity.AvailableBoost = 0;
                }
            }
            else if (Velocity.BoostPercentage > 0) //Ramp down the boost.
            {
                Velocity.BoostPercentage -= _core.Settings.EnemyThrustRampDown;
                if (Velocity.BoostPercentage < 0)
                {
                    Velocity.BoostPercentage = 0;
                }
            }

            var thrustVector = Velocity.MaxSpeed * (Velocity.ThrottlePercentage + -Velocity.RecoilPercentage);

            if (Velocity.BoostPercentage > 0)
            {
                thrustVector += Velocity.MaxBoost * Velocity.BoostPercentage;
            }

            X += Velocity.Angle.X * thrustVector - displacementVector.X;
            Y += Velocity.Angle.Y * thrustVector - displacementVector.Y;

            //base.ApplyMotion(displacementVector);

            if (RadarPositionIndicator != null)
            {
                if (_core.Display.GetCurrentScaledScreenBounds().IntersectsWith(Bounds, -50) == false)
                {
                    RadarPositionText.DistanceValue = Math.Abs(DistanceTo(_core.Player.Sprite));

                    RadarPositionText.Visable = true;
                    RadarPositionIndicator.Visable = true;

                    double requiredAngle = _core.Player.Sprite.AngleTo360(this);

                    var offset = HgMath.PointFromAngleAtDistance360(new NsAngle(requiredAngle), new NsPoint(200, 200));

                    RadarPositionText.Location = _core.Player.Sprite.Location + offset + new NsPoint(25, 25);
                    RadarPositionIndicator.Velocity.Angle.Degrees = requiredAngle;

                    RadarPositionIndicator.Location = _core.Player.Sprite.Location + offset;
                    RadarPositionIndicator.Velocity.Angle.Degrees = requiredAngle;
                }
                else
                {
                    RadarPositionText.Visable = false;
                    RadarPositionIndicator.Visable = false;
                }
            }

            if (Velocity.RecoilPercentage > 0)
            {
                Velocity.RecoilPercentage -= Velocity.RecoilPercentage * 0.01;
                if (Velocity.RecoilPercentage < 0.01)
                {
                    Velocity.RecoilPercentage = 0;
                }
            }
        }

        public virtual void ApplyIntelligence(NsPoint displacementVector)
        {
            if (Weapons != null && _core.Player.Sprite != null)
            {
                foreach (var weapon in Weapons)
                {
                    if (weapon.ApplyWeaponsLock(displacementVector, _core.Player.Sprite)) //Enemy lock-on to Player. :O
                    {
                        break;
                    }
                }
            }
        }

        internal void AddAIController(IAIController controller)
            => AIControllers.Add(controller.GetType(), controller);

        internal void SetCurrentAIController(IAIController value)
        {
            CurrentAIController = value;
        }

        #region Weapons selection and evaluation.

        public void ClearWeapons() => Weapons.Clear();

        public void AddWeapon(string weaponTypeName, int munitionCount)
        {
            var weaponType = NsReflection.GetTypeByName(weaponTypeName);

            var weapon = Weapons.Where(o => o.GetType() == weaponType).SingleOrDefault();

            if (weapon == null)
            {
                weapon = NsReflection.CreateInstanceFromType<WeaponBase>(weaponType, new object[] { _core, this });
                weapon.RoundQuantity += munitionCount;
                Weapons.Add(weapon);
            }
            else
            {
                weapon.RoundQuantity += munitionCount;
            }
        }

        public void AddWeapon<T>(int munitionCount) where T : WeaponBase
        {
            var weapon = GetWeaponOfType<T>();
            if (weapon == null)
            {
                weapon = NsReflection.CreateInstanceOf<T>(new object[] { _core, this });
                weapon.RoundQuantity += munitionCount;
                Weapons.Add(weapon);
            }
            else
            {
                weapon.RoundQuantity += munitionCount;
            }
        }

        public int TotalAvailableWeaponRounds() => (from o in Weapons select o.RoundQuantity).Sum();
        public int TotalWeaponFiredRounds() => (from o in Weapons select o.RoundsFired).Sum();

        public bool HasWeapon<T>() where T : WeaponBase
        {
            var existingWeapon = (from o in Weapons where o.GetType() == typeof(T) select o).FirstOrDefault();
            return existingWeapon != null;
        }

        public bool HasWeaponAndAmmo<T>() where T : WeaponBase
        {
            var existingWeapon = (from o in Weapons where o.GetType() == typeof(T) select o).FirstOrDefault();
            return existingWeapon != null && existingWeapon.RoundQuantity > 0;
        }

        public bool FireWeapon<T>() where T : WeaponBase
        {
            var weapon = GetWeaponOfType<T>();
            return weapon?.Fire() == true;
        }

        public WeaponBase GetWeaponOfType<T>() where T : WeaponBase
        {
            return (from o in Weapons where o.GetType() == typeof(T) select o).FirstOrDefault();
        }

        #endregion
    }
}
