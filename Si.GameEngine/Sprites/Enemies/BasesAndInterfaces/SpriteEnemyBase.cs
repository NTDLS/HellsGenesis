using Newtonsoft.Json;
using Si.GameEngine.AI;
using Si.GameEngine.Engine;
using Si.GameEngine.Loudouts;
using Si.GameEngine.Managers;
using Si.GameEngine.Sprites.Powerup;
using Si.GameEngine.Sprites.Powerup.BasesAndInterfaces;
using Si.GameEngine.Weapons.BasesAndInterfaces;
using Si.GameEngine.Weapons.Munitions;
using Si.Shared;
using Si.Shared.Payload.DroneActions;
using Si.Shared.Types.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using static Si.Shared.SiConstants;

namespace Si.GameEngine.Sprites.Enemies.BasesAndInterfaces
{
    /// <summary>
    /// The enemy base is a sub-class of the ship base. It is used by Peon and Boss enemies.
    /// </summary>
    public class SpriteEnemyBase : SpriteShipBase
    {
        private DateTime _lastMultiplaySpriteVectorUpdate = DateTime.MinValue;

        public SiEnemyClass ShipClass { get; set; }
        public EnemyShipLoadout Loadout { get; set; }
        public IAIController CurrentAIController { get; set; }
        public Dictionary<Type, IAIController> AIControllers { get; private set; } = new();
        public int CollisionDamage { get; set; } = 25;
        public int BountyWorth { get; private set; } = 25;
        public bool IsHostile { get; set; } = true;
        public List<WeaponBase> Weapons { get; private set; } = new();

        public SpriteEnemyBase(EngineCore gameCore, int hullHealth, int bountyMultiplier)
                : base(gameCore)
        {
            Velocity.ThrottlePercentage = 1;
            Initialize();

            SetHullHealth(hullHealth);
            BountyWorth = HullHealth * bountyMultiplier;

            RadarPositionIndicator = _gameCore.Sprites.RadarPositions.Create();
            RadarPositionIndicator.Visable = false;
            RadarPositionText = _gameCore.Sprites.TextBlocks.CreateRadarPosition(
                gameCore.Rendering.TextFormats.RadarPositionIndicator,
                gameCore.Rendering.Materials.Brushes.Red, new SiPoint());
        }

        public virtual void BeforeCreate() { }

        public virtual void AfterCreate() { }

        public override void RotationChanged() => PositionChanged();

        public override void Explode()
        {
            _gameCore.Player.Sprite.Bounty += BountyWorth;

            if (SiRandom.PercentChance(10))
            {
                var powerup = SiRandom.Between(0, 4) switch
                {
                    0 => new SpritePowerupAmmo(_gameCore),
                    1 => new SpritePowerupBoost(_gameCore),
                    2 => new SpritePowerupBounty(_gameCore),
                    3 => new SpritePowerupRepair(_gameCore),
                    4 => new SpritePowerupSheild(_gameCore),
                    _ => null as SpritePowerupBase
                };

                if (powerup != null)
                {
                    powerup.LocalLocation = Location;
                    _gameCore.Sprites.Powerups.Insert(powerup);
                    _gameCore.Sprites.MultiplayNotifyOfSpriteCreation(powerup);
                }
            }
            base.Explode();
        }

        public override SiDroneActionVector GetMultiplayVector()
        {
            if (_gameCore.Multiplay.State.PlayMode == SiPlayMode.MutiPlayerHost)
            {
                if ((DateTime.UtcNow - _lastMultiplaySpriteVectorUpdate).TotalMilliseconds >= _gameCore.Multiplay.State.PlayerAbsoluteStateDelayMs)
                {
                    _lastMultiplaySpriteVectorUpdate = DateTime.UtcNow;

                    var bgOffset = _gameCore.Display.BackgroundOffset;

                    return new SiDroneActionVector(MultiplayUID)
                    {
                        MultiplayUID = MultiplayUID,
                        X = LocalX + bgOffset.X,
                        Y = LocalY + bgOffset.Y,
                        AngleDegrees = Velocity.Angle.Degrees,
                        BoostPercentage = Velocity.BoostPercentage,
                        ThrottlePercentage = Velocity.ThrottlePercentage,
                        MaxBoost = Velocity.MaxBoost,
                        MaxSpeed = Velocity.MaxSpeed
                    };
                }
            }

            return null;
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
            result += $"       Sheilds : {Loadout.ShieldHealth:n0}\n";
            result += $" Hull Strength : {Loadout.HullHealth:n0}\n";
            result += $"     Max Speed : {Loadout.MaxSpeed:n1}\n";
            result += $"    Warp Drive : {Loadout.MaxBoost:n1}\n";
            result += $"\n{Loadout.Description}";

            return result;
        }

        public EnemyShipLoadout LoadLoadoutFromFile(SiEnemyClass shipClass)
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

        public override bool TryMunitionHit(SiPoint displacementVector, MunitionBase munition, SiPoint hitTestPosition)
        {
            if (munition.FiredFromType == SiFiredFromType.Player)
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

        /// <summary>
        /// Moves the sprite to the exact location as dictated by the remote connection.
        /// Also sets the current vector of the remote sprite so that we can move the sprite along that vector between updates.
        /// </summary>
        /// <param name="vector"></param>
        public override void ApplyAbsoluteMultiplayVector(SiDroneActionVector vector)
        {
            Velocity.ThrottlePercentage = vector.ThrottlePercentage;
            Velocity.BoostPercentage = vector.BoostPercentage;
            Velocity.MaxSpeed = vector.MaxSpeed;
            Velocity.MaxBoost = vector.MaxBoost;
            Velocity.Angle.Degrees = vector.AngleDegrees;
            Velocity.AvailableBoost = 10000; //Just a high number so the drone does not run out of boost.

            MultiplayX = vector.X;
            MultiplayY = vector.Y;
        }

        /// <summary>
        /// Moves the sprite based on its thrust/boost (velocity) taking into account the background scroll.
        /// </summary>
        /// <param name="displacementVector"></param>
        public override void ApplyMotion(SiPoint displacementVector)
        {
            if (IsDrone)
            {
                var thrust = (Velocity.MaxSpeed * Velocity.ThrottlePercentage) + (Velocity.MaxBoost * Velocity.BoostPercentage);

                //Move sprite based on Multiplay vector. Linear interpolation?
                MultiplayX += Velocity.Angle.X * thrust;
                MultiplayY += Velocity.Angle.Y * thrust;

                //Move sprite based on local offset.
                LocalX -= displacementVector.X;
                LocalY -= displacementVector.Y;

                FixRadarPositionIndicator();

                return;
            }

            if (LocalX < -_gameCore.Settings.EnemySceneDistanceLimit
                || LocalX >= _gameCore.Display.NatrualScreenSize.Width + _gameCore.Settings.EnemySceneDistanceLimit
                || LocalY < -_gameCore.Settings.EnemySceneDistanceLimit
                || LocalY >= _gameCore.Display.NatrualScreenSize.Height + _gameCore.Settings.EnemySceneDistanceLimit)
            {
                QueueForDelete();
                return;
            }

            //When an enemy had boost available, it will use it.
            if (Velocity.AvailableBoost > 0)
            {
                if (Velocity.BoostPercentage < 1.0) //Ramp up the boost until it is at 100%
                {
                    Velocity.BoostPercentage += _gameCore.Settings.EnemyThrustRampUp;
                }
                Velocity.AvailableBoost -= Velocity.MaxBoost * Velocity.BoostPercentage; //Consume boost.

                if (Velocity.AvailableBoost < 0) //Sanity check available boost.
                {
                    Velocity.AvailableBoost = 0;
                }
            }
            else if (Velocity.BoostPercentage > 0) //Ramp down the boost.
            {
                Velocity.BoostPercentage -= _gameCore.Settings.EnemyThrustRampDown;
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

            LocalX += Velocity.Angle.X * thrustVector - displacementVector.X;
            LocalY += Velocity.Angle.Y * thrustVector - displacementVector.Y;

            //base.ApplyMotion(displacementVector);

            FixRadarPositionIndicator();

            if (Velocity.RecoilPercentage > 0)
            {
                Velocity.RecoilPercentage -= Velocity.RecoilPercentage * 0.01;
                if (Velocity.RecoilPercentage < 0.01)
                {
                    Velocity.RecoilPercentage = 0;
                }
            }
        }

        public virtual void ApplyIntelligence(SiPoint displacementVector)
        {
            if (Weapons != null && _gameCore.Player.Sprite != null)
            {
                foreach (var weapon in Weapons)
                {
                    if (weapon.ApplyWeaponsLock(_gameCore.Player.Sprite)) //Enemy lock-on to Player. :O
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
            var weaponType = SiReflection.GetTypeByName(weaponTypeName);

            var weapon = Weapons.Where(o => o.GetType() == weaponType).SingleOrDefault();

            if (weapon == null)
            {
                weapon = SiReflection.CreateInstanceFromType<WeaponBase>(weaponType, new object[] { _gameCore, this });
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
                weapon = SiReflection.CreateInstanceOf<T>(new object[] { _gameCore, this });
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

            if (weapon != null && _gameCore.Multiplay.State.PlayMode != SiPlayMode.SinglePlayer && this.IsDrone == false)
            {
                _gameCore.Multiplay.RecordSpriteWeaponFire(new SiDroneActionFireWeapon(MultiplayUID)
                {
                    WeaponTypeName = weapon.GetType().Name,
                });
            }

            return weapon?.Fire() == true;
        }

        public WeaponBase GetWeaponOfType<T>() where T : WeaponBase
        {
            return (from o in Weapons where o.GetType() == typeof(T) select o).FirstOrDefault();
        }

        #endregion
    }
}
