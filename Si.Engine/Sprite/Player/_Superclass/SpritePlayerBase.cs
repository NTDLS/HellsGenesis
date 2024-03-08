using Newtonsoft.Json;
using Si.Audio;
using Si.Engine.Loudout;
using Si.Engine.Manager;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite.Player._Superclass
{
    /// <summary>
    /// The player base is a sub-class of the ship base. It is only used by the Player and as a model for menu selections.
    /// </summary>
    public class SpritePlayerBase : SpriteShipBase
    {
        public SiPlayerClass ShipClass { get; set; }
        public PlayerShipLoadout Loadout { get; private set; }
        public SiAudioClip AmmoLowSound { get; private set; }
        public SiAudioClip AmmoEmptySound { get; private set; }
        public SiAudioClip ShipEngineRoarSound { get; private set; }
        public SiAudioClip ShipEngineIdleSound { get; private set; }
        public SiAudioClip AllSystemsGoSound { get; private set; }
        public SiAudioClip ShieldFailSound { get; private set; }
        public SiAudioClip ShieldDownSound { get; private set; }
        public SiAudioClip ShieldMaxSound { get; private set; }
        public SiAudioClip ShieldNominalSound { get; private set; }
        public SiAudioClip SystemsFailingSound { get; private set; }
        public SiAudioClip HullBreachedSound { get; private set; }
        public SiAudioClip IntegrityLowSound { get; private set; }
        public SiAudioClip ShipEngineBoostSound { get; private set; }
        public int Bounty { get; set; } //Score points.
        public int MaxHullHealth { get; set; }
        public int MaxShieldPoints { get; set; }
        public SpriteAnimation ThrustAnimation { get; private set; }
        public SpriteAnimation BoostAnimation { get; private set; }
        public WeaponBase PrimaryWeapon { get; private set; }
        private List<WeaponBase> _secondaryWeapons = new();
        public WeaponBase SelectedSecondaryWeapon { get; private set; }
        public bool UseSpeedBoost { get; private set; }

        public SpritePlayerBase(EngineCore engine)
            : base(engine)
        {
            OnHit += SpritePlayer_OnHit;

            AmmoLowSound = _engine.Assets.GetAudio(@"Sounds\Ship\Ammo Low.wav", 0.75f);
            SystemsFailingSound = _engine.Assets.GetAudio(@"Sounds\Ship\Systems Failing.wav", 0.75f);
            HullBreachedSound = _engine.Assets.GetAudio(@"Sounds\Ship\Hull Breached.wav", 0.75f);
            IntegrityLowSound = _engine.Assets.GetAudio(@"Sounds\Ship\Integrity Low.wav", 0.75f);
            ShieldFailSound = _engine.Assets.GetAudio(@"Sounds\Ship\Shield Fail.wav", 0.75f);
            ShieldDownSound = _engine.Assets.GetAudio(@"Sounds\Ship\Shield Down.wav", 0.75f);
            ShieldMaxSound = _engine.Assets.GetAudio(@"Sounds\Ship\Shield Max.wav", 0.75f);
            ShieldNominalSound = _engine.Assets.GetAudio(@"Sounds\Ship\Shield Nominal.wav", 0.75f);
            AllSystemsGoSound = _engine.Assets.GetAudio(@"Sounds\Ship\All Systems Go.wav", 0.75f);
            AmmoLowSound = _engine.Assets.GetAudio(@"Sounds\Ship\Ammo Low.wav", 0.75f);
            AmmoEmptySound = _engine.Assets.GetAudio(@"Sounds\Ship\Ammo Empty.wav", 0.75f);
            ShipEngineRoarSound = _engine.Assets.GetAudio(@"Sounds\Ship\Engine Roar.wav", 0.5f, true);
            ShipEngineIdleSound = _engine.Assets.GetAudio(@"Sounds\Ship\Engine Idle.wav", 0.5f, true);
            ShipEngineBoostSound = _engine.Assets.GetAudio(@"Sounds\Ship\Engine Boost.wav", 0.5f, true);

            CenterInUniverse();
        }

        public void ToggleSpeedBoost()
        {
            UseSpeedBoost = !UseSpeedBoost;
        }

        public override void Cleanup()
        {
            ThrustAnimation?.QueueForDelete();
            BoostAnimation?.QueueForDelete();
            base.Cleanup();
        }

        public override void VisibilityChanged()
        {
            UpdateThrustAnimationPositions();
            if (Visable == false)
            {
                if (ThrustAnimation != null) ThrustAnimation.Visable = false;
                if (BoostAnimation != null) BoostAnimation.Visable = false;
                ShipEngineIdleSound?.Stop();
                ShipEngineRoarSound?.Stop();
            }
        }

        public override void RotationChanged() => UpdateThrustAnimationPositions();

        //The player position does not change, only the background offset changes... hmmmm. :/
        public override void LocationChanged() => UpdateThrustAnimationPositions();

        public string GetLoadoutHelpText()
        {
            string weaponName = SiReflection.GetStaticPropertyValue(Loadout.PrimaryWeapon.Type, "Name");
            string primaryWeapon = $"{weaponName} x{Loadout.PrimaryWeapon.MunitionCount}";

            string secondaryWeapons = string.Empty;
            foreach (var weapon in Loadout.SecondaryWeapons)
            {
                weaponName = SiReflection.GetStaticPropertyValue(weapon.Type, "Name");
                secondaryWeapons += $"{weaponName} x{weapon.MunitionCount}\n{new string(' ', 20)}";
            }

            string result = $"             Name : {Loadout.Name}\n";
            result += $"   Primary weapon : {primaryWeapon.Trim()}\n";
            result += $"Secondary Weapons : {secondaryWeapons.Trim()}\n";
            result += $"          Shields : {Loadout.ShieldHealth:n0}\n";
            result += $"    Hull Strength : {Loadout.HullHealth:n0}\n";
            result += $"        Max Speed : {Loadout.Speed:n1}\n";
            result += $"      Surge Drive : {Loadout.Boost:n1}\n";
            result += $"\n{Loadout.Description}";

            return result;
        }

        public PlayerShipLoadout LoadLoadoutFromFile(SiPlayerClass shipClass)
        {
            PlayerShipLoadout loadout = null;

            var loadoutText = EngineAssetManager.GetUserText($"Player.{shipClass}.loadout.json");

            try
            {
                if (string.IsNullOrWhiteSpace(loadoutText) == false)
                {
                    loadout = JsonConvert.DeserializeObject<PlayerShipLoadout>(loadoutText);
                }
            }
            catch
            {
                loadout = null;
            }

            return loadout;
        }

        public void SaveLoadoutToFile(PlayerShipLoadout loadout)
        {
            var serializedText = JsonConvert.SerializeObject(loadout, Formatting.Indented);
            EngineAssetManager.PutUserText($"Player.{loadout.Class}.loadout.json", serializedText);
        }

        /// <summary>
        /// Resets ship image, state, health etc while allowing you to change the class.
        /// </summary>
        /// <param name="playerClass"></param>
        public void ResetLoadout(PlayerShipLoadout loadout)
        {
            Loadout = loadout;
            SetImage(@$"Graphics\Player\Ships\{loadout.Name}.png");
            Reset();
        }

        /// <summary>
        /// Resets ship state, health etc while keeping the existing class.
        /// </summary>
        public void Reset()
        {
            ClearPrimaryWeapon();
            ClearSecondaryWeapons();

            ReviveDeadOrExploded();

            Velocity.Angle = new SiAngle(SiPoint.DegreesToRadians(45));

            Velocity.ForwardMomentium = 0;
            Velocity.AvailableBoost = _engine.Settings.MaxPlayerBoostAmount;

            #region Reset loadout.

            Velocity.MaximumSpeed = Loadout.Speed;
            Velocity.MaximumBoostSpeed = Loadout.Boost;

            SetHullHealth(Loadout.HullHealth);
            SetShieldHealth(Loadout.ShieldHealth);

            SetPrimaryWeapon(Loadout.PrimaryWeapon.Type, Loadout.PrimaryWeapon.MunitionCount);

            foreach (var secondaryWeapon in Loadout.SecondaryWeapons)
            {
                AddSecondaryWeapon(secondaryWeapon.Type, secondaryWeapon.MunitionCount);
            }

            #endregion

            SelectFirstAvailableUsableSecondaryWeapon();

            if (ThrustAnimation == null || ThrustAnimation.IsQueuedForDeletion == true)
            {
                var playMode = new SpriteAnimation.PlayMode()
                {
                    Replay = SiAnimationReplayMode.LoopedPlay,
                    DeleteSpriteAfterPlay = false,
                    ReplayDelay = new TimeSpan(0)
                };
                ThrustAnimation = new SpriteAnimation(_engine, @"Graphics\Animation\ThrustStandard32x32.png", new Size(32, 32), 10, playMode)
                {
                    SpriteTag = "PlayerForwardThrust",
                    Visable = false,
                    OwnerUID = UID
                };
                //ThrustAnimation.Reset();
                _engine.Sprites.Animations.AddAt(ThrustAnimation, this);
                ThrustAnimation.OnVisibilityChanged += (sender) => UpdateThrustAnimationPositions();
            }

            if (BoostAnimation == null || BoostAnimation.IsQueuedForDeletion == true)
            {
                var playMode = new SpriteAnimation.PlayMode()
                {
                    Replay = SiAnimationReplayMode.LoopedPlay,
                    DeleteSpriteAfterPlay = false,
                    ReplayDelay = new TimeSpan(0)
                };
                BoostAnimation = new SpriteAnimation(_engine, @"Graphics\Animation\ThrustBoost32x32.png", new Size(32, 32), 10, playMode)
                {
                    SpriteTag = "PlayerForwardThrust",
                    Visable = false,
                    OwnerUID = UID
                };
                //BoostAnimation.Reset();
                _engine.Sprites.Animations.AddAt(BoostAnimation, this);
                BoostAnimation.OnVisibilityChanged += (sender) => UpdateThrustAnimationPositions();
            }
        }

        public override void AddShieldHealth(int pointsToAdd)
        {
            if (ShieldHealth < _engine.Settings.MaxShieldHealth && ShieldHealth + pointsToAdd >= _engine.Settings.MaxShieldHealth)
            {
                ShieldMaxSound.Play(); //If we didnt have full shields but now we do, tell the player.
            }

            base.AddShieldHealth(pointsToAdd);
        }

        private void UpdateThrustAnimationPositions()
        {
            if (ThrustAnimation != null)
            {
                if (Visable)
                {
                    var pointBehind = SiPoint.PointFromAngleAtDistance360(Velocity.Angle + SiPoint.DegreesToRadians(180), new SiPoint(20, 20));
                    ThrustAnimation.Velocity.Angle = Velocity.Angle;
                    ThrustAnimation.Location = Location + pointBehind;
                }
            }

            if (BoostAnimation != null)
            {
                if (Visable)
                {
                    var pointBehind = SiPoint.PointFromAngleAtDistance360(Velocity.Angle + SiPoint.DegreesToRadians(180), new SiPoint(20, 20));
                    BoostAnimation.Velocity.Angle = Velocity.Angle;
                    BoostAnimation.Location = Location + pointBehind;
                }
            }
        }

        public override void MunitionHit(MunitionBase munition)
        {
            Hit(munition);
            if (HullHealth <= 0)
            {
                //Explode(); //We don't auto delete the player because the engine always assumes its valid. 
            }
        }

        public override bool TryMunitionHit(MunitionBase munition, SiPoint hitTestPosition)
        {
            if (munition.FiredFromType == SiFiredFromType.Enemy)
            {
                return Intersects(hitTestPosition);
            }
            return false;
        }

        private void SpritePlayer_OnHit(SpriteBase sender, SiDamageType damageType, int damageAmount)
        {
            if (damageType == SiDamageType.Shield)
            {
                if (ShieldHealth == 0)
                {
                    ShieldDownSound.Play();
                }
            }

            //This is the hit that took us under the treshold.
            if (HullHealth < 100 && HullHealth + damageAmount > 100)
            {
                IntegrityLowSound.Play();
            }
            else if (HullHealth < 50 && HullHealth + damageAmount > 50)
            {
                SystemsFailingSound.Play();
            }
            else if (HullHealth < 20 && HullHealth + damageAmount > 20)
            {
                HullBreachedSound.Play();
            }
        }

        #region Weapons selection and evaluation.

        public void ClearPrimaryWeapon()
        {
            PrimaryWeapon = null;
        }

        public void ClearSecondaryWeapons() => _secondaryWeapons.Clear();

        public void SetPrimaryWeapon(string weaponTypeName, int munitionCount)
        {
            var weaponType = SiReflection.GetTypeByName(weaponTypeName);

            if (PrimaryWeapon?.GetType() == weaponType)
            {
                PrimaryWeapon.RoundQuantity += munitionCount;
            }
            else
            {
                var weapon = SiReflection.CreateInstanceFromType<WeaponBase>(weaponType, new object[] { _engine, this });
                weapon.RoundQuantity += munitionCount;
                PrimaryWeapon = weapon;

                if (PrimaryWeapon == null) //If there is no primary weapon selected, then default to the newly added one.
                {
                    PrimaryWeapon = weapon;
                }
            }
        }

        public void AddSecondaryWeapon(string weaponTypeName, int munitionCount)
        {
            var weaponType = SiReflection.GetTypeByName(weaponTypeName);

            var weapon = _secondaryWeapons.Where(o => o.GetType() == weaponType).SingleOrDefault();

            if (weapon == null)
            {
                weapon = SiReflection.CreateInstanceFromType<WeaponBase>(weaponType, new object[] { _engine, this });
                weapon.RoundQuantity += munitionCount;
                _secondaryWeapons.Add(weapon);
            }
            else
            {
                weapon.RoundQuantity += munitionCount;
            }

            if (SelectedSecondaryWeapon == null) //If there is no secondary weapon selected, then default to the newly added one.
            {
                SelectedSecondaryWeapon = weapon;
            }
        }

        /// <summary>
        /// Adds a new primary weapon or adds its ammo to the current of its type.
        /// </summary>
        /// <param name="weapon"></param>
        public void SetPrimaryWeapon<T>(int munitionCount) where T : WeaponBase
        {
            if (PrimaryWeapon is T)
            {
                PrimaryWeapon.RoundQuantity += munitionCount;
            }
            else
            {
                PrimaryWeapon = SiReflection.CreateInstanceOf<T>(new object[] { _engine, this });
                PrimaryWeapon.RoundQuantity += munitionCount;
            }
        }

        /// <summary>
        /// Adds a new secondary weapon or adds its ammo to the current of its type.
        /// </summary>
        /// <param name="weapon"></param>
        public void AddSecondaryWeapon<T>(int munitionCount) where T : WeaponBase
        {
            var weapon = GetSecondaryWeaponOfType<T>();
            if (weapon == null)
            {
                weapon = SiReflection.CreateInstanceOf<T>(new object[] { _engine, this });
                weapon.RoundQuantity += munitionCount;
                _secondaryWeapons.Add(weapon);
            }
            else
            {
                weapon.RoundQuantity += munitionCount;
            }

            if (SelectedSecondaryWeapon == null) //If there is no secondary weapon selected, then default to the newly added one.
            {
                SelectedSecondaryWeapon = weapon;
            }
        }

        public int TotalAvailableSecondaryWeaponRounds() => (from o in _secondaryWeapons select o.RoundQuantity).Sum();
        public int TotalSecondaryWeaponFiredRounds() => (from o in _secondaryWeapons select o.RoundsFired).Sum();

        public WeaponBase SelectPreviousAvailableUsableSecondaryWeapon()
        {
            WeaponBase previousWeapon = null;

            foreach (var weapon in _secondaryWeapons)
            {
                if (weapon == SelectedSecondaryWeapon)
                {
                    if (previousWeapon == null)
                    {
                        return SelectLastAvailableUsableSecondaryWeapon(); //No sutible weapon found after the current one. Go back to the end.
                    }
                    SelectedSecondaryWeapon = previousWeapon;
                    return previousWeapon;
                }

                previousWeapon = weapon;
            }

            return SelectFirstAvailableUsableSecondaryWeapon(); //No sutible weapon found after the current one. Go back to the beginning.
        }

        public WeaponBase SelectNextAvailableUsableSecondaryWeapon()
        {
            bool selectNextWeapon = false;

            foreach (var weapon in _secondaryWeapons)
            {
                if (selectNextWeapon)
                {
                    SelectedSecondaryWeapon = weapon;
                    return weapon;
                }

                if (weapon == SelectedSecondaryWeapon) //Find the current weapon in the collection;
                {
                    selectNextWeapon = true;
                }
            }

            return SelectFirstAvailableUsableSecondaryWeapon(); //No sutible weapon found after the current one. Go back to the beginning.
        }

        public bool HasSecondaryWeapon<T>() where T : WeaponBase
        {
            var existingWeapon = (from o in _secondaryWeapons where o.GetType() == typeof(T) select o).FirstOrDefault();
            return existingWeapon != null;
        }

        public bool HasSecondaryWeaponAndAmmo<T>() where T : WeaponBase
        {
            var existingWeapon = (from o in _secondaryWeapons where o.GetType() == typeof(T) select o).FirstOrDefault();
            return existingWeapon != null && existingWeapon.RoundQuantity > 0;
        }

        public bool HasPrimaryWeaponAndAmmo<T>() where T : WeaponBase
        {
            if (PrimaryWeapon is T)
            {
                return PrimaryWeapon.RoundQuantity > 0;
            }
            return false;
        }

        public bool HasPrimaryWeaponAndAmmo()
        {
            return PrimaryWeapon?.RoundQuantity > 0;
        }

        public bool HasSelectedPrimaryWeaponAndAmmo()
        {
            return PrimaryWeapon != null && PrimaryWeapon.RoundQuantity > 0;
        }

        public bool HasSelectedSecondaryWeaponAndAmmo()
        {
            return SelectedSecondaryWeapon != null && SelectedSecondaryWeapon.RoundQuantity > 0;
        }

        public WeaponBase SelectLastAvailableUsableSecondaryWeapon()
        {
            var existingWeapon = (from o in _secondaryWeapons where o.RoundQuantity > 0 select o).LastOrDefault();
            if (existingWeapon != null)
            {
                SelectedSecondaryWeapon = existingWeapon;
            }
            else
            {
                SelectedSecondaryWeapon = null;
            }
            return SelectedSecondaryWeapon;
        }

        public WeaponBase SelectFirstAvailableUsableSecondaryWeapon()
        {
            var existingWeapon = (from o in _secondaryWeapons where o.RoundQuantity > 0 select o).FirstOrDefault();
            if (existingWeapon != null)
            {
                SelectedSecondaryWeapon = existingWeapon;
            }
            else
            {
                SelectedSecondaryWeapon = null;
            }
            return SelectedSecondaryWeapon;
        }

        public WeaponBase GetSecondaryWeaponOfType<T>() where T : WeaponBase
        {
            return (from o in _secondaryWeapons where o.GetType() == typeof(T) select o).FirstOrDefault();
        }

        public WeaponBase SelectSecondaryWeapon<T>() where T : WeaponBase
        {
            SelectedSecondaryWeapon = GetSecondaryWeaponOfType<T>();
            return SelectedSecondaryWeapon;
        }

        #endregion

        /// <summary>
        /// Moves the sprite to the exact location as dictated by the remote connection.
        /// Also sets the current vector of the remote sprite so that we can move the sprite along that vector between updates.
        /// </summary>
        /// <param name="vector"></param>

        public override void ApplyMotion(float epoch, SiPoint displacementVector)
        {
            base.ApplyMotion(epoch, displacementVector);
        }
    }
}
