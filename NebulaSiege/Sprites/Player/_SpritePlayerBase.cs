using NebulaSiege.Engine;
using NebulaSiege.Engine.Types;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Loudouts;
using NebulaSiege.Managers;
using NebulaSiege.Utility;
using NebulaSiege.Weapons.Munitions;
using Newtonsoft.Json;
using System;
using System.Drawing;

namespace NebulaSiege.Sprites.Player
{
    internal class _SpritePlayerBase : _SpriteShipBase
    {
        public HgPlayerClass ShipClass { get; set; }
        public ShipLoadout Loadout { get; private set; }
        public NsAudioClip AmmoLowSound { get; private set; }
        public NsAudioClip AmmoEmptySound { get; private set; }
        public NsAudioClip ShipEngineRoarSound { get; private set; }
        public NsAudioClip ShipEngineIdleSound { get; private set; }
        public NsAudioClip AllSystemsGoSound { get; private set; }
        public NsAudioClip ShieldFailSound { get; private set; }
        public NsAudioClip ShieldDownSound { get; private set; }
        public NsAudioClip ShieldMaxSound { get; private set; }
        public NsAudioClip ShieldNominalSound { get; private set; }
        public NsAudioClip SystemsFailingSound { get; private set; }
        public NsAudioClip HullBreachedSound { get; private set; }
        public NsAudioClip IntegrityLowSound { get; private set; }
        public NsAudioClip ShipEngineBoostSound { get; private set; }
        public int Bounty { get; set; } //Score points.
        public int MaxHullHealth { get; set; }
        public int MaxShieldPoints { get; set; }
        public SpriteAnimation ThrustAnimation { get; private set; }
        public SpriteAnimation BoostAnimation { get; private set; }

        public _SpritePlayerBase(EngineCore core)
            : base(core)
        {
            OnHit += SpritePlayer_OnHit;

            AmmoLowSound = _core.Assets.GetAudio(@"Sounds\Ship\Ammo Low.wav", 0.75f);
            SystemsFailingSound = _core.Assets.GetAudio(@"Sounds\Ship\Systems Failing.wav", 0.75f);
            HullBreachedSound = _core.Assets.GetAudio(@"Sounds\Ship\Hull Breached.wav", 0.75f);
            IntegrityLowSound = _core.Assets.GetAudio(@"Sounds\Ship\Integrity Low.wav", 0.75f);
            ShieldFailSound = _core.Assets.GetAudio(@"Sounds\Ship\Shield Fail.wav", 0.75f);
            ShieldDownSound = _core.Assets.GetAudio(@"Sounds\Ship\Shield Down.wav", 0.75f);
            ShieldMaxSound = _core.Assets.GetAudio(@"Sounds\Ship\Shield Max.wav", 0.75f);
            ShieldNominalSound = _core.Assets.GetAudio(@"Sounds\Ship\Shield Nominal.wav", 0.75f);
            AllSystemsGoSound = _core.Assets.GetAudio(@"Sounds\Ship\All Systems Go.wav", 0.75f);
            AmmoLowSound = _core.Assets.GetAudio(@"Sounds\Ship\Ammo Low.wav", 0.75f);
            AmmoEmptySound = _core.Assets.GetAudio(@"Sounds\Ship\Ammo Empty.wav", 0.75f);
            ShipEngineRoarSound = _core.Assets.GetAudio(@"Sounds\Ship\Engine Roar.wav", 0.5f, true);
            ShipEngineIdleSound = _core.Assets.GetAudio(@"Sounds\Ship\Engine Idle.wav", 0.5f, true);
            ShipEngineBoostSound = _core.Assets.GetAudio(@"Sounds\Ship\Engine Boost.wav", 0.5f, true);
        }

        public override void VisibilityChanged()
        {
            UpdateThrustAnimationPositions();
            if (Visable == false)
            {
                if (ThrustAnimation != null) ThrustAnimation.Visable = false;
                if (BoostAnimation != null) BoostAnimation.Visable = false;
            }
        }

        public override void RotationChanged() => UpdateThrustAnimationPositions();
        public override void PositionChanged() => UpdateThrustAnimationPositions();

        public string GetLoadoutHelpText()
        {
            string weaponName = NsReflection.GetStaticPropertyValue(Loadout.PrimaryWeapon.Type, "Name");
            string primaryWeapon = $"{weaponName} x{Loadout.PrimaryWeapon.Rounds}";

            string secondaryWeapons = string.Empty;
            foreach (var weapon in Loadout.SecondaryWeapons)
            {
                weaponName = NsReflection.GetStaticPropertyValue(weapon.Type, "Name");
                secondaryWeapons += $"{weaponName} x{weapon.Rounds}\n{new string(' ', 20)}";
            }

            string result = $"             Name : {Loadout.Name}\n";
            result += $"   Primary weapon : {primaryWeapon.Trim()}\n";
            result += $"Secondary Weapons : {secondaryWeapons.Trim()}\n";
            result += $"          Sheilds : {Loadout.ShieldHealth:n0}\n";
            result += $"    Hull Strength : {Loadout.HullHealth:n0}\n";
            result += $"        Max Speed : {Loadout.MaxSpeed:n1}\n";
            result += $"       Warp Drive : {Loadout.MaxBoost:n1}\n";
            result += $"\n{Loadout.Description}";

            return result;
        }

        public ShipLoadout LoadLoadoutFromFile(HgPlayerClass playerClass)
        {
            ShipLoadout loadout = null;

            var loadoutText = EngineAssetManager.GetUserText($"{playerClass}.loadout.json");

            try
            {
                if (string.IsNullOrWhiteSpace(loadoutText))
                {
                    loadout = JsonConvert.DeserializeObject<ShipLoadout>(loadoutText);
                }
            }
            catch
            {
                loadout = null;
            }

            return loadout;
        }

        public void SaveLoadoutToFile(ShipLoadout loadout)
        {
            var serializedText = JsonConvert.SerializeObject(loadout, Formatting.Indented);
            EngineAssetManager.PutUserText($"{loadout.Class}.loadout.json", serializedText);
        }

        /// <summary>
        /// Resets ship image, state, health etc while allowing you to change the class.
        /// </summary>
        /// <param name="playerClass"></param>
        public void ResetLoadout(ShipLoadout loadout)
        {
            Loadout = loadout;
            SetImage(@$"Graphics\Player\Ships\{loadout.Name}.png", new Size(32, 32));
            Reset();
        }

        /// <summary>
        /// Resets ship state, health etc while keeping the existing class.
        /// </summary>
        public void Reset()
        {
            ClearPrimaryWeapon();
            ClearSecondaryWeapons();

            IsDead = false;

            X = _core.Display.TotalCanvasSize.Width / 2;
            Y = _core.Display.TotalCanvasSize.Height / 2;

            Velocity.Angle = new NsAngle(45);

            Velocity.ThrottlePercentage = 0;
            Velocity.AvailableBoost = _core.Settings.MaxPlayerBoostAmount;

            #region Reset loadout.

            Velocity.MaxSpeed = Loadout.MaxSpeed;
            Velocity.MaxBoost = Loadout.MaxBoost;

            SetHullHealth(Loadout.HullHealth);
            SetShieldHealth(Loadout.ShieldHealth);

            SetPrimaryWeapon(Loadout.PrimaryWeapon.Type, Loadout.PrimaryWeapon.Rounds);

            foreach (var secondaryWeapon in Loadout.SecondaryWeapons)
            {
                AddSecondaryWeapon(secondaryWeapon.Type, secondaryWeapon.Rounds);
            }

            #endregion

            SelectFirstAvailableUsableSecondaryWeapon();
        }

        public override void AddShieldHealth(int pointsToAdd)
        {
            if (ShieldHealth < _core.Settings.MaxShieldPoints && ShieldHealth + pointsToAdd >= _core.Settings.MaxShieldPoints)
            {
                ShieldMaxSound.Play(); //If we didnt have full shields but now we do, tell the player.
            }

            base.AddShieldHealth(pointsToAdd);
        }

        private void UpdateThrustAnimationPositions()
        {
            if (ThrustAnimation == null || ThrustAnimation.ReadyForDeletion == true)
            {
                var playMode = new SpriteAnimation.PlayMode()
                {
                    Replay = HgAnimationReplayMode.LoopedPlay,
                    DeleteSpriteAfterPlay = false,
                    ReplayDelay = new TimeSpan(0)
                };
                ThrustAnimation = new SpriteAnimation(_core, @"Graphics\Animation\ThrustStandard32x32.png", new Size(32, 32), 10, playMode)
                {
                    IsFixedPosition = true,
                    Visable = false
                };
                ThrustAnimation.Reset();
                _core.Sprites.Animations.InsertAt(ThrustAnimation, this);
                ThrustAnimation.OnVisibilityChanged += (sender) => UpdateThrustAnimationPositions();
            }

            if (BoostAnimation == null || BoostAnimation.ReadyForDeletion == true)
            {
                var playMode = new SpriteAnimation.PlayMode()
                {
                    Replay = HgAnimationReplayMode.LoopedPlay,
                    DeleteSpriteAfterPlay = false,
                    ReplayDelay = new TimeSpan(0)
                };
                BoostAnimation = new SpriteAnimation(_core, @"Graphics\Animation\ThrustBoost32x32.png", new Size(32, 32), 10, playMode)
                {
                    IsFixedPosition = true,
                    Visable = false
                };
                BoostAnimation.Reset();
                _core.Sprites.Animations.InsertAt(BoostAnimation, this);
                BoostAnimation.OnVisibilityChanged += (sender) => UpdateThrustAnimationPositions();
            }

            if (ThrustAnimation != null)
            {
                if (Visable)
                {
                    var pointBehind = HgMath.AngleFromPointAtDistance(Velocity.Angle + 180, new NsPoint(20, 20));
                    ThrustAnimation.Velocity.Angle.Degrees = Velocity.Angle.Degrees - 180;
                    ThrustAnimation.X = X + pointBehind.X;
                    ThrustAnimation.Y = Y + pointBehind.Y;
                }
            }

            if (BoostAnimation != null)
            {
                if (Visable)
                {
                    var pointBehind = HgMath.AngleFromPointAtDistance(Velocity.Angle + 180, new NsPoint(20, 20));
                    BoostAnimation.Velocity.Angle.Degrees = Velocity.Angle.Degrees - 180;
                    BoostAnimation.X = X + pointBehind.X;
                    BoostAnimation.Y = Y + pointBehind.Y;
                }
            }
        }

        public override bool TryMunitionHit(NsPoint displacementVector, _MunitionBase munition, NsPoint hitTestPosition)
        {
            if (munition.FiredFromType == HgFiredFromType.Enemy)
            {
                if (Intersects(hitTestPosition))
                {
                    //We don't auto delete the player because there is only one instance, the engine always assumes its valid.
                    Hit(munition);
                    return true;
                }
            }
            return false;
        }

        private void SpritePlayer_OnHit(_SpriteBase sender, HgDamageType damageType, int damageAmount)
        {
            if (damageType == HgDamageType.Shield)
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
    }
}