using Newtonsoft.Json;
using Si.Engine.AI.Logistics._Superclass;
using Si.Engine.Loudout;
using Si.Engine.Manager;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.PowerUp;
using Si.Engine.Sprite.PowerUp._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite.Enemy._Superclass
{
    /// <summary>
    /// The enemy base is a sub-class of the ship base. It is used by Peon and Boss enemies.
    /// </summary>
    public class SpriteEnemyBase : SpriteShipBase
    {
        public SiEnemyClass ShipClass { get; set; }
        public EnemyShipLoadout Loadout { get; set; }
        public IIAController CurrentAIController { get; set; }
        public Dictionary<Type, IIAController> AIControllers { get; private set; } = new();
        public int Bounty { get; private set; } = 0;
        public bool IsHostile { get; set; } = true;
        public List<WeaponBase> Weapons { get; private set; } = new();

        public SpriteEnemyBase(EngineCore engine)
                : base(engine)
        {
            Velocity.ForwardVelocity = 1;
            Initialize();

            //SetHullHealth(hullHealth);
            //BountyWorth = HullHealth * bountyMultiplier;

            RadarPositionIndicator = _engine.Sprites.RadarPositions.Create();
            RadarPositionIndicator.Visable = false;
            RadarPositionText = _engine.Sprites.TextBlocks.CreateRadarPosition(
                engine.Rendering.TextFormats.RadarPositionIndicator,
                engine.Rendering.Materials.Brushes.Red, new SiPoint());
        }

        public virtual void BeforeCreate() { }

        public virtual void AfterCreate() { }

        public override void RotationChanged() => LocationChanged();

        public override void Explode()
        {
            _engine.Player.Sprite.Bounty += Bounty;

            if (SiRandom.PercentChance(10))
            {
                var powerup = SiRandom.Between(0, 4) switch
                {
                    0 => new SpritePowerupAmmo(_engine),
                    1 => new SpritePowerupBoost(_engine),
                    2 => new SpritePowerupBounty(_engine),
                    3 => new SpritePowerupRepair(_engine),
                    4 => new SpritePowerupShield(_engine),
                    _ => null as SpritePowerupBase
                };

                if (powerup != null)
                {
                    powerup.Location = Location;
                    _engine.Sprites.Powerups.Add(powerup);
                }
            }
            base.Explode();
        }

        public string GetLoadoutHelpText()
        {
            string weapons = string.Empty;
            foreach (var weapon in Loadout.Weapons)
            {
                var weaponName = SiReflection.GetStaticPropertyValue(weapon.Type, "Name");
                weapons += $"{weaponName} x{weapon.MunitionCount}\n{new string(' ', 20)}";
            }

            string result = $"          Name : {Loadout.Name}\n";
            result += $"       Weapons : {weapons.Trim()}\n";
            result += $"       Shields : {Loadout.ShieldHealth:n0}\n";
            result += $" Hull Strength : {Loadout.HullHealth:n0}\n";
            result += $"     Max Speed : {Loadout.Speed:n1}\n";
            result += $"   Surge Drive : {Loadout.Boost:n1}\n";
            result += $"\n{Loadout.Description}";

            return result;
        }

        public EnemyShipLoadout LoadLoadoutFromFile(SiEnemyClass shipClass)
        {
            EnemyShipLoadout loadout = null;

            var loadoutText = EngineAssetManager.GetUserText($"Enemy.{shipClass}.loadout.json");

            try
            {
                if (string.IsNullOrWhiteSpace(loadoutText) == false)
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
            Velocity.MaximumSpeed = Loadout.Speed;
            Velocity.MaximumBoostSpeed = Loadout.Boost;
            Bounty = Loadout.Bounty;

            SetHullHealth(Loadout.HullHealth);
            SetShieldHealth(Loadout.ShieldHealth);

            Weapons.Clear();
            foreach (var weapon in Loadout.Weapons)
            {
                AddWeapon(weapon.Type, weapon.MunitionCount);
            }
        }

        public override bool TryMunitionHit(MunitionBase munition, SiPoint hitTestPosition)
        {
            if (munition.FiredFromType == SiFiredFromType.Player)
            {
                if (Intersects(hitTestPosition))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Moves the sprite based on its velocity/boost (velocity) taking into account the background scroll.
        /// </summary>
        /// <param name="displacementVector"></param>
        public override void ApplyMotion(float epoch, SiPoint displacementVector)
        {

            if (!_engine.Display.TotalCanvasBounds.Balloon(_engine.Settings.EnemySceneDistanceLimit).IntersectsWith(RenderBounds))
            {
                QueueForDelete();
                return;
            }

            //When an enemy had boost available, it will use it.
            if (Velocity.AvailableBoost > 0)
            {
                if (Velocity.ForwardBoostVelocity < 1.0) //Ramp up the boost until it is at 100%
                {
                    Velocity.ForwardBoostVelocity += _engine.Settings.EnemyVelocityRampUp;
                }
                Velocity.AvailableBoost -= Velocity.MaximumBoostSpeed * Velocity.ForwardBoostVelocity; //Consume boost.

                if (Velocity.AvailableBoost < 0) //Sanity check available boost.
                {
                    Velocity.AvailableBoost = 0;
                }
            }
            else if (Velocity.ForwardBoostVelocity > 0) //Ramp down the boost.
            {
                Velocity.ForwardBoostVelocity -= _engine.Settings.EnemyVelocityRampDown;
                if (Velocity.ForwardBoostVelocity < 0)
                {
                    Velocity.ForwardBoostVelocity = 0;
                }
            }

            var velocityAmount = Velocity.MaximumSpeed * Velocity.ForwardVelocity;

            if (Velocity.ForwardBoostVelocity > 0)
            {
                velocityAmount += Velocity.MaximumBoostSpeed * Velocity.ForwardBoostVelocity;
            }

            //Location += (Velocity.Angle * velocityAmount * epoch);

            Location += Velocity.ForwardAngle * velocityAmount * epoch;

            //System.Diagnostics.Debug.Print($"Dbg: {newLocation}, Actual: {Location}");

            //base.ApplyMotion(epoch, displacementVector);

            FixRadarPositionIndicator();
        }

        public virtual void ApplyIntelligence(float epoch, SiPoint displacementVector)
        {
            if (Weapons != null)
            {
                foreach (var weapon in Weapons)
                {
                    weapon.ApplyIntelligence(epoch);
                }
            }
        }

        internal void AddAIController(IIAController controller)
            => AIControllers.Add(controller.GetType(), controller);

        internal void SetCurrentAIController(IIAController value)
        {
            CurrentAIController = value;
        }

        #region Weapons selection and evaluation.

        public void ClearWeapons() => Weapons.Clear();

        public void AddWeapon(string weaponTypeName, int munitionCount)
        {
            var weaponType = SiReflection.GetTypeByName(weaponTypeName);

            var weapon = Weapons.Where(o => o.GetType() == weaponType).SingleOrDefault();

            if (weapon == null)
            {
                weapon = SiReflection.CreateInstanceFromType<WeaponBase>(weaponType, new object[] { _engine, this });
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
                weapon = SiReflection.CreateInstanceOf<T>(new object[] { _engine, this });
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

        public bool FireWeapon<T>(SiPoint location, float? angle = null) where T : WeaponBase
        {
            var weapon = GetWeaponOfType<T>();
            return weapon?.Fire(location, angle) == true;
        }

        public WeaponBase GetWeaponOfType<T>() where T : WeaponBase
        {
            return (from o in Weapons where o.GetType() == typeof(T) select o).FirstOrDefault();
        }

        #endregion
    }
}
