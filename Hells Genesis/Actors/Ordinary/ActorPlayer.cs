using HG.Actors.BaseClasses;
using HG.Actors.Weapons.BaseClasses;
using HG.Actors.Weapons.Bullets.BaseClasses;
using HG.Engine;
using HG.Loudouts;
using HG.Types;
using HG.Utility;
using System;
using System.Drawing;

namespace HG.Actors.Ordinary
{
    internal class ActorPlayer : ActorShipBase
    {
        public ShipLoadout Loadout { get; private set; }
        public AudioClip AmmoLowSound { get; private set; }
        public AudioClip AmmoEmptySound { get; private set; }
        public AudioClip ShipEngineRoarSound { get; private set; }
        public AudioClip ShipEngineIdleSound { get; private set; }
        public AudioClip AllSystemsGoSound { get; private set; }
        public AudioClip ShieldFailSound { get; private set; }
        public AudioClip ShieldDownSound { get; private set; }
        public AudioClip ShieldMaxSound { get; private set; }
        public AudioClip ShieldNominalSound { get; private set; }
        public AudioClip SystemsFailingSound { get; private set; }
        public AudioClip HullBreachedSound { get; private set; }
        public AudioClip IntegrityLowSound { get; private set; }
        public AudioClip ShipEngineBoostSound { get; private set; }
        public int Bounty { get; set; } //Score points.
        public int MaxHullHealth { get; set; }
        public int MaxShieldPoints { get; set; }
        public ActorAnimation ThrustAnimation { get; private set; }
        public ActorAnimation BoostAnimation { get; private set; }

        public ActorPlayer(Core core, ShipLoadout loadout)
            : base(core)
        {
            Loadout = loadout;

            string imagePath = @$"Graphics\Player\Ships\{loadout.ImageIndex}.png";

            Initialize(imagePath, new Size(32, 32));

            OnHit += ActorPlayer_OnHit;

            AmmoLowSound = _core.Audio.Get(@"Sounds\Ship\Ammo Low.wav", 0.75f);
            SystemsFailingSound = _core.Audio.Get(@"Sounds\Ship\Systems Failing.wav", 0.75f);
            HullBreachedSound = _core.Audio.Get(@"Sounds\Ship\Hull Breached.wav", 0.75f);
            IntegrityLowSound = _core.Audio.Get(@"Sounds\Ship\Integrity Low.wav", 0.75f);
            ShieldFailSound = _core.Audio.Get(@"Sounds\Ship\Shield Fail.wav", 0.75f);
            ShieldDownSound = _core.Audio.Get(@"Sounds\Ship\Shield Down.wav", 0.75f);
            ShieldMaxSound = _core.Audio.Get(@"Sounds\Ship\Shield Max.wav", 0.75f);
            ShieldNominalSound = _core.Audio.Get(@"Sounds\Ship\Shield Nominal.wav", 0.75f);
            AllSystemsGoSound = _core.Audio.Get(@"Sounds\Ship\All Systems Go.wav", 0.75f);
            AmmoLowSound = _core.Audio.Get(@"Sounds\Ship\Ammo Low.wav", 0.75f);
            AmmoEmptySound = _core.Audio.Get(@"Sounds\Ship\Ammo Empty.wav", 0.75f);
            ShipEngineRoarSound = _core.Audio.Get(@"Sounds\Ship\Engine Roar.wav", 0.5f, true);
            ShipEngineIdleSound = _core.Audio.Get(@"Sounds\Ship\Engine Idle.wav", 0.5f, true);
            ShipEngineBoostSound = _core.Audio.Get(@"Sounds\Ship\Engine Boost.wav", 0.5f, true);
        }

        /// <summary>
        /// Resets ship image, state, health etc while allowing you to change the class.
        /// </summary>
        /// <param name="playerClass"></param>
        public void Reset(ShipLoadout loadout)
        {
            Loadout = loadout;
            SetImage(@$"Graphics\Player\Ships\{Loadout.ImageIndex}.png", new Size(32, 32));
            Reset();
        }

        /// <summary>
        /// Resets ship state, health etc while keeping the existing class.
        /// </summary>
        public void Reset()
        {
            ClearPrimaryWeapons();
            ClearSecondaryWeapons();

            IsDead = false;

            X = _core.Display.TotalCanvasSize.Width / 2;
            Y = _core.Display.TotalCanvasSize.Height / 2;

            Velocity.Angle = new HgAngle<double>(45);

            Velocity.ThrottlePercentage = _core.Settings.MinPlayerThrust;
            Velocity.AvailableBoost = _core.Settings.MaxPlayerBoost;
            Velocity.MaxRotationSpeed = _core.Settings.MaxRotationSpeed;

            #region Reset loadout.

            Velocity.MaxSpeed = Loadout.Speed;
            Velocity.MaxBoost = Loadout.Boost;

            SetHullHealth(Loadout.Hull);
            SetShieldHealth(Loadout.Sheilds);

            var weapon = HgReflection.CreateInstanceOf<WeaponBase>(Loadout.PrimaryWeapon.Type, new object[] { _core });
            weapon.RoundQuantity = Loadout.PrimaryWeapon.Rounds;
            AddPrimaryWeapon(weapon);

            foreach (var secondaryWeapon in Loadout.SecondaryWeapons)
            {
                var secondaryWeaponInstance = HgReflection.CreateInstanceOf<WeaponBase>(secondaryWeapon.Type, new object[] { _core });
                secondaryWeaponInstance.RoundQuantity = secondaryWeapon.Rounds;
                AddSecondaryWeapon(secondaryWeaponInstance);
            }

            #endregion

            SelectFirstAvailableUsablePrimaryWeapon();
            SelectFirstAvailableUsableSecondaryWeapon();
        }

        public override void VisibilityChanged()
        {
            if (Visable == true)
            {
                if (ThrustAnimation == null || ThrustAnimation.ReadyForDeletion == true)
                {
                    var playMode = new ActorAnimation.PlayMode()
                    {
                        Replay = ActorAnimation.ReplayMode.LoopedPlay,
                        DeleteActorAfterPlay = false,
                        ReplayDelay = new TimeSpan(0)
                    };
                    ThrustAnimation = new ActorAnimation(_core, @"Graphics\Animation\ThrustStandard32x32.png", new Size(32, 32), 10, playMode);
                    ThrustAnimation.Reset();
                    ThrustAnimation.Visable = false;
                    _core.Actors.Animations.CreateAt(ThrustAnimation, this);

                    var pointRight = HgMath.AngleFromPointAtDistance(Velocity.Angle + 180, new HgPoint<double>(20, 20));
                    ThrustAnimation.Velocity.Angle.Degrees = Velocity.Angle.Degrees - 180;
                    ThrustAnimation.X = X + pointRight.X;
                    ThrustAnimation.Y = Y + pointRight.Y;
                }

                if (BoostAnimation == null || BoostAnimation.ReadyForDeletion == true)
                {
                    var playMode = new ActorAnimation.PlayMode()
                    {
                        Replay = ActorAnimation.ReplayMode.LoopedPlay,
                        DeleteActorAfterPlay = false,
                        ReplayDelay = new TimeSpan(0)
                    };
                    BoostAnimation = new ActorAnimation(_core, @"Graphics\Animation\ThrustBoost32x32.png", new Size(32, 32), 10, playMode);
                    BoostAnimation.Reset();
                    BoostAnimation.Visable = false;
                    _core.Actors.Animations.CreateAt(BoostAnimation, this);

                    var pointRight = HgMath.AngleFromPointAtDistance(Velocity.Angle + 180, new HgPoint<double>(20, 20));
                    BoostAnimation.Velocity.Angle.Degrees = Velocity.Angle.Degrees - 180;
                    BoostAnimation.X = X + pointRight.X;
                    BoostAnimation.Y = Y + pointRight.Y;
                }
            }
        }

        public override void RotationChanged()
        {
            PositionChanged();
        }

        public override void PositionChanged()
        {
            if (ThrustAnimation != null && ThrustAnimation.Visable)
            {
                var pointRight = HgMath.AngleFromPointAtDistance(Velocity.Angle + 180, new HgPoint<double>(20, 20));
                ThrustAnimation.Velocity.Angle.Degrees = Velocity.Angle.Degrees - 180;
                ThrustAnimation.X = X + pointRight.X;
                ThrustAnimation.Y = Y + pointRight.Y;
            }

            if (BoostAnimation != null && BoostAnimation.Visable)
            {
                var pointRight = HgMath.AngleFromPointAtDistance(Velocity.Angle + 180, new HgPoint<double>(20, 20));
                BoostAnimation.Velocity.Angle.Degrees = Velocity.Angle.Degrees - 180;
                BoostAnimation.X = X + pointRight.X;
                BoostAnimation.Y = Y + pointRight.Y;
            }
        }

        public override void Explode()
        {
            base.Explode();
        }

        public override bool TestHit(HgPoint<double> displacementVector, BulletBase bullet, HgPoint<double> hitTestPosition)
        {
            if (bullet.FiredFromType == HgFiredFromType.Enemy)
            {
                if (Intersects(hitTestPosition))
                {
                    //We don't auto delete the player because there is only one instance, the engine always assumes its valid.
                    if (Hit(bullet))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void ActorPlayer_OnHit(ActorBase sender, HgDamageType damageType, int damageAmount)
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