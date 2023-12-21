using Newtonsoft.Json;
using StrikeforceInfinity.Game.AI;
using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Loudouts;
using StrikeforceInfinity.Game.Managers;
using StrikeforceInfinity.Game.Sprites.PowerUp;
using StrikeforceInfinity.Game.Sprites.PowerUp.BasesAndInterfaces;
using StrikeforceInfinity.Game.Utility;
using StrikeforceInfinity.Game.Utility.ExtensionMethods;
using StrikeforceInfinity.Game.Weapons.BasesAndInterfaces;
using StrikeforceInfinity.Game.Weapons.Munitions;
using StrikeforceInfinity.Shared.Messages.Notify;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StrikeforceInfinity.Game.Sprites.Enemies.BasesAndInterfaces
{
    /// <summary>
    /// The enemy base is a sub-class of the ship base. It is used by Peon and Boss enemies.
    /// </summary>
    internal class SpriteEnemyBase : _SpriteShipBase
    {
        private readonly SiSpriteVector _multiplaySpriteVector = new();

        public HgEnemyClass ShipClass { get; set; }
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

            if (HgRandom.PercentChance(5))
            {
                int random = HgRandom.Between(0, 4);

                SpritePowerUpBase powerUp = null;

                switch (random)
                {
                    case 0:
                        powerUp = new SpritePowerUpAmmo(_gameCore);
                        break;
                    case 1:
                        powerUp = new SpritePowerUpBoost(_gameCore);
                        break;
                    case 2:
                        powerUp = new SpritePowerUpBounty(_gameCore);
                        break;
                    case 3:
                        powerUp = new SpritePowerUpRepair(_gameCore);
                        break;
                    case 4:
                        powerUp = new SpritePowerUpSheild(_gameCore);
                        break;
                }

                if (powerUp != null)
                {
                    powerUp.Location = Location;
                    _gameCore.Sprites.Powerups.Insert(powerUp);
                }
            }
            base.Explode();
        }

        public override SiSpriteVector GetMultiplayVector()
        {
            if (_gameCore.Multiplay.PlayMode == HgPlayMode.MutiPlayerHost)
            {
                if ((DateTime.UtcNow - _multiplaySpriteVector.Timestamp).TotalMilliseconds >= _gameCore.Multiplay.State.PlayerAbsoluteStateDelayMs)
                {
                    //System.Diagnostics.Debug.WriteLine($"MultiplayUID: {enemy.MultiplayUID}");

                    _multiplaySpriteVector.MultiplayUID = MultiplayUID;
                    _multiplaySpriteVector.Timestamp = DateTime.UtcNow;
                    _multiplaySpriteVector.X = X;
                    _multiplaySpriteVector.Y = Y;
                    _multiplaySpriteVector.AngleDegrees = Velocity.Angle.Degrees;
                    _multiplaySpriteVector.BoostPercentage = Velocity.BoostPercentage;
                    _multiplaySpriteVector.ThrottlePercentage = Velocity.ThrottlePercentage;

                    return _multiplaySpriteVector;
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

        public override bool TryMunitionHit(SiPoint displacementVector, MunitionBase munition, SiPoint hitTestPosition)
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

        public override void ApplyMotion(SiPoint displacementVector)
        {
            if (X < -_gameCore.Settings.EnemySceneDistanceLimit
                || X >= _gameCore.Display.NatrualScreenSize.Width + _gameCore.Settings.EnemySceneDistanceLimit
                || Y < -_gameCore.Settings.EnemySceneDistanceLimit
                || Y >= _gameCore.Display.NatrualScreenSize.Height + _gameCore.Settings.EnemySceneDistanceLimit)
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

            X += Velocity.Angle.X * thrustVector - displacementVector.X;
            Y += Velocity.Angle.Y * thrustVector - displacementVector.Y;

            //base.ApplyMotion(displacementVector);

            if (RadarPositionIndicator != null)
            {
                if (_gameCore.Display.GetCurrentScaledScreenBounds().IntersectsWith(Bounds, -50) == false)
                {
                    RadarPositionText.DistanceValue = Math.Abs(DistanceTo(_gameCore.Player.Sprite));

                    RadarPositionText.Visable = true;
                    RadarPositionIndicator.Visable = true;

                    double requiredAngle = _gameCore.Player.Sprite.AngleTo360(this);

                    var offset = HgMath.PointFromAngleAtDistance360(new SiAngle(requiredAngle), new SiPoint(200, 200));

                    RadarPositionText.Location = _gameCore.Player.Sprite.Location + offset + new SiPoint(25, 25);
                    RadarPositionIndicator.Velocity.Angle.Degrees = requiredAngle;

                    RadarPositionIndicator.Location = _gameCore.Player.Sprite.Location + offset;
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

        public virtual void ApplyIntelligence(SiPoint displacementVector)
        {
            if (Weapons != null && _gameCore.Player.Sprite != null)
            {
                foreach (var weapon in Weapons)
                {
                    if (weapon.ApplyWeaponsLock(displacementVector, _gameCore.Player.Sprite)) //Enemy lock-on to Player. :O
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
            return weapon?.Fire() == true;
        }

        public WeaponBase GetWeaponOfType<T>() where T : WeaponBase
        {
            return (from o in Weapons where o.GetType() == typeof(T) select o).FirstOrDefault();
        }

        #endregion
    }
}
