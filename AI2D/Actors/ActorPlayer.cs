using AI2D.Engine;
using AI2D.Types;
using AI2D.Weapons;
using System;
using System.Drawing;
using static AI2D.Engine.Constants;

namespace AI2D.Actors
{
    public class ActorPlayer : ActorBase
    {
        public PlayerClass Class { get; private set; }
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
        public int Score { get; set; }
        public int MaxHitPoints { get; set; }
        public int MaxShieldPoints { get; set; }
        public ActorAnimation ThrustAnimation { get; private set; }
        public ActorAnimation BoostAnimation { get; private set; }

        public ActorPlayer(Core core, PlayerClass playerClass)
            : base(core)
        {
            Class = playerClass;

            string imagePath = @$"..\..\..\Assets\Graphics\Player\Alien\{(int)Class}.png";

            Initialize(imagePath, new Size(32, 32));

            AmmoLowSound = _core.Actors.GetSoundCached(@"..\..\..\Assets\Sounds\Ship\Ammo Low.wav", 0.75f);
            SystemsFailingSound = _core.Actors.GetSoundCached(@"..\..\..\Assets\Sounds\Ship\Systems Failing.wav", 0.75f);
            HullBreachedSound = _core.Actors.GetSoundCached(@"..\..\..\Assets\Sounds\Ship\Hull Breached.wav", 0.75f);
            IntegrityLowSound = _core.Actors.GetSoundCached(@"..\..\..\Assets\Sounds\Ship\Integrity Low.wav", 0.75f);
            ShieldFailSound = _core.Actors.GetSoundCached(@"..\..\..\Assets\Sounds\Ship\Shield Fail.wav", 0.75f);
            ShieldDownSound = _core.Actors.GetSoundCached(@"..\..\..\Assets\Sounds\Ship\Shield Down.wav", 0.75f);
            ShieldMaxSound = _core.Actors.GetSoundCached(@"..\..\..\Assets\Sounds\Ship\Shield Max.wav", 0.75f);
            ShieldNominalSound = _core.Actors.GetSoundCached(@"..\..\..\Assets\Sounds\Ship\Shield Nominal.wav", 0.75f);
            AllSystemsGoSound = _core.Actors.GetSoundCached(@"..\..\..\Assets\Sounds\Ship\All Systems Go.wav", 0.75f);
            AmmoLowSound = _core.Actors.GetSoundCached(@"..\..\..\Assets\Sounds\Ship\Ammo Low.wav", 0.75f);
            AmmoEmptySound = _core.Actors.GetSoundCached(@"..\..\..\Assets\Sounds\Ship\Ammo Empty.wav", 0.75f);

            ShipEngineRoarSound = _core.Actors.GetSoundCached(@"..\..\..\Assets\Sounds\Ship\Engine Roar.wav", 1.0f, true);
            ShipEngineIdleSound = _core.Actors.GetSoundCached(@"..\..\..\Assets\Sounds\Ship\Engine Idle.wav", 0.6f, true);
            ShipEngineBoostSound = _core.Actors.GetSoundCached(@"..\..\..\Assets\Sounds\Ship\Engine Boost.wav", 1.0f, true);

            this.OnPositionChanged += ObjPlayer_OnPositionChanged;
            this.OnRotated += ObjPlayer_OnPositionChanged;
            this.OnVisibilityChange += ObjPlayer_OnVisibilityChange;
        }


        /// <summary>
        /// Resets ship image, state, health etc while allowing you to change the class.
        /// </summary>
        /// <param name="playerClass"></param>
        public void Reset(PlayerClass playerClass)
        {
            Class = playerClass;

            string imagePath = @$"..\..\..\Assets\Graphics\Player\Alien\{(int)Class}.png";

            SetImage(imagePath, new Size(32, 32));

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

            X = _core.Display.VisibleSize.Width / 2;
            Y = _core.Display.VisibleSize.Height / 2;

            Velocity.Angle = new Angle<double>(45);

            Velocity.ThrottlePercentage = Constants.Limits.MinPlayerThrust;
            Velocity.AvailableBoost = Constants.Limits.MaxPlayerBoost;
            Velocity.MaxRotationSpeed = Constants.Limits.MaxRotationSpeed;

            if (Class == PlayerClass.Nimitz)
            {
                Velocity.MaxSpeed = 3.0;
                Velocity.MaxBoost = 1.5;
                SetHitPoints(500);
                SetShieldPoints(100);

                AddPrimaryWeapon(new WeaponVulcanCannon(_core) { RoundQuantity = 5000 });
                AddSecondaryWeapon(new WeaponFragMissile(_core) { RoundQuantity = 16 });
                //AddSecondaryWeapon(new WeaponFragMissile(_core) { RoundQuantity = 500 }); //I dont have a second non guided missile.
            }
            else if (Class == PlayerClass.Knox)
            {
                Velocity.MaxSpeed = 2.2;
                Velocity.MaxBoost = 1.0;
                SetHitPoints(2500);
                SetShieldPoints(3000);

                AddPrimaryWeapon(new WeaponVulcanCannon(_core) { RoundQuantity = 5000 });
                AddSecondaryWeapon(new WeaponFragMissile(_core) { RoundQuantity = 16 });
                //AddSecondaryWeapon(new WeaponFragMissile(_core) { RoundQuantity = 500 }); //I dont have a second non guided missile.
            }
            else if (Class == PlayerClass.Luhu)
            {
                Velocity.MaxSpeed = 5.0;
                Velocity.MaxBoost = 2.5;
                SetHitPoints(500);
                SetShieldPoints(1000);

                AddPrimaryWeapon(new WeaponVulcanCannon(_core) { RoundQuantity = 5000 });
                AddSecondaryWeapon(new WeaponGuidedFragMissile(_core) { RoundQuantity = 16 });
            }
            else if (Class == PlayerClass.Atlant)
            {
                Velocity.MaxSpeed = 4.0;
                Velocity.MaxBoost = 2.0;
                SetHitPoints(500);
                SetShieldPoints(1000);

                AddPrimaryWeapon(new WeaponVulcanCannon(_core) { RoundQuantity = 5000 });
                AddSecondaryWeapon(new WeaponFragMissile(_core) { RoundQuantity = 16 });
                AddSecondaryWeapon(new WeaponGuidedFragMissile(_core) { RoundQuantity = 10 });
                AddSecondaryWeapon(new WeaponPrecisionGuidedFragMissile(_core) { RoundQuantity = 6 });
                AddSecondaryWeapon(new WeaponScramsMissile(_core) { RoundQuantity = 32 });
            }
            else if (Class == PlayerClass.Ticonderoga)
            {
                Velocity.MaxSpeed = 6.0;
                Velocity.MaxBoost = 3.5;
                SetHitPoints(500);
                SetShieldPoints(1000);

                AddPrimaryWeapon(new WeaponDualVulcanCannon(_core) { RoundQuantity = 5000 });
                AddSecondaryWeapon(new WeaponPhotonTorpedo(_core) { RoundQuantity = 100 });
                AddSecondaryWeapon(new WeaponPulseMeson(_core) { RoundQuantity = 8 });
            }
            else if (Class == PlayerClass.Kirov)
            {
                Velocity.MaxSpeed = 5.0;
                Velocity.MaxBoost = 3.5;
                SetHitPoints(100);
                SetShieldPoints(15000);

                AddPrimaryWeapon(new WeaponVulcanCannon(_core) { RoundQuantity = 5000 });
                AddSecondaryWeapon(new WeaponDualVulcanCannon(_core) { RoundQuantity = 2500 });
            }


            SelectFirstAvailableUsablePrimaryWeapon();
            SelectFirstAvailableUsableSecondaryWeapon();
        }

        private void ObjPlayer_OnVisibilityChange(ActorBase obj)
        {
            if (Visable == true)
            {
                if (ThrustAnimation == null || ThrustAnimation.ReadyForDeletion == true)
                {
                    string _debugAniPath = @"..\..\..\Assets\Graphics\Animation\AirThrust32x32.png";
                    var playMode = new ActorAnimation.PlayMode()
                    {
                        Replay = ActorAnimation.ReplayMode.LoopedPlay,
                        DeleteActorAfterPlay = false,
                        ReplayDelay = new TimeSpan(0)
                    };
                    ThrustAnimation = new ActorAnimation(_core, _debugAniPath, new Size(32, 32), 10, playMode);

                    ThrustAnimation.Reset();

                    _core.Actors.PlaceAnimationOnTopOf(ThrustAnimation, this);

                    var pointRight = Utility.AngleFromPointAtDistance(base.Velocity.Angle + 180, new Point<double>(20, 20));
                    ThrustAnimation.Velocity.Angle.Degrees = this.Velocity.Angle.Degrees - 180;
                    ThrustAnimation.X = this.X + pointRight.X;
                    ThrustAnimation.Y = this.Y + pointRight.Y;
                }

                if (BoostAnimation == null || BoostAnimation.ReadyForDeletion == true)
                {
                    string _debugAniPath = @"..\..\..\Assets\Graphics\Animation\FireThrust32x32.png";
                    var playMode = new ActorAnimation.PlayMode()
                    {
                        Replay = ActorAnimation.ReplayMode.LoopedPlay,
                        DeleteActorAfterPlay = false,
                        ReplayDelay = new TimeSpan(0)
                    };
                    BoostAnimation = new ActorAnimation(_core, _debugAniPath, new Size(32, 32), 10, playMode);

                    BoostAnimation.Reset();

                    _core.Actors.PlaceAnimationOnTopOf(BoostAnimation, this);

                    var pointRight = Utility.AngleFromPointAtDistance(base.Velocity.Angle + 180, new Point<double>(20, 20));
                    BoostAnimation.Velocity.Angle.Degrees = this.Velocity.Angle.Degrees - 180;
                    BoostAnimation.X = this.X + pointRight.X;
                    BoostAnimation.Y = this.Y + pointRight.Y;
                }
            }
        }

        private void ObjPlayer_OnPositionChanged(ActorBase obj)
        {
            if (ThrustAnimation != null && ThrustAnimation.Visable)
            {
                var pointRight = Utility.AngleFromPointAtDistance(base.Velocity.Angle + 180, new Point<double>(20, 20));
                ThrustAnimation.Velocity.Angle.Degrees = this.Velocity.Angle.Degrees - 180;
                ThrustAnimation.X = this.X + pointRight.X;
                ThrustAnimation.Y = this.Y + pointRight.Y;
            }

            if (BoostAnimation != null && BoostAnimation.Visable)
            {
                var pointRight = Utility.AngleFromPointAtDistance(base.Velocity.Angle + 180, new Point<double>(20, 20));
                BoostAnimation.Velocity.Angle.Degrees = this.Velocity.Angle.Degrees - 180;
                BoostAnimation.X = this.X + pointRight.X;
                BoostAnimation.Y = this.Y + pointRight.Y;
            }
        }

        public new void Explode(bool autoKill = true, bool autoDelete = true)
        {
            base.Explode(true, false);
        }
    }
}