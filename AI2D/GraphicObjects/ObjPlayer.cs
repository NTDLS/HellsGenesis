using AI2D.Engine;
using AI2D.Types;
using System;
using System.Drawing;

namespace AI2D.GraphicObjects
{
    public class ObjPlayer : ActorBase
    {
        private const string _imagePath = @"..\..\..\Assets\Graphics\Player\Alien\1.png";

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
        public ObjAnimation ThrustAnimation { get; private set; }
        public ObjAnimation BoostAnimation { get; private set; }

        public ObjPlayer(Core core)
            : base(core)
        {
            Initialize(_imagePath, new Size(32, 32));

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

        private void ObjPlayer_OnVisibilityChange(ActorBase obj)
        {
            if (Visable == true)
            {
                if (ThrustAnimation == null || ThrustAnimation.ReadyForDeletion == true)
                {
                    string _debugAniPath = @"..\..\..\Assets\Graphics\Animation\AirThrust32x32.png";
                    var playMode = new ObjAnimation.PlayMode()
                    {
                        Replay = ObjAnimation.ReplayMode.LoopedPlay,
                        DeleteActorAfterPlay = false,
                        ReplayDelay = new TimeSpan(0)
                    };
                    ThrustAnimation = new ObjAnimation(_core, _debugAniPath, new Size(32, 32), 10, playMode);

                    ThrustAnimation.Reset();

                    _core.Actors.PlaceAnimationOnTopOf(ThrustAnimation, this);

                    var pointRight = Utility.AngleFromPointAtDistance(base.Velocity.Angle + 180, new PointD(20, 20));
                    ThrustAnimation.Velocity.Angle.Degrees = this.Velocity.Angle.Degrees - 180;
                    ThrustAnimation.X = this.X + pointRight.X;
                    ThrustAnimation.Y = this.Y + pointRight.Y;
                }

                if (BoostAnimation == null || BoostAnimation.ReadyForDeletion == true)
                {
                    string _debugAniPath = @"..\..\..\Assets\Graphics\Animation\FireThrust32x32.png";
                    var playMode = new ObjAnimation.PlayMode()
                    {
                        Replay = ObjAnimation.ReplayMode.LoopedPlay,
                        DeleteActorAfterPlay = false,
                        ReplayDelay = new TimeSpan(0)
                    };
                    BoostAnimation = new ObjAnimation(_core, _debugAniPath, new Size(32, 32), 10, playMode);

                    BoostAnimation.Reset();

                    _core.Actors.PlaceAnimationOnTopOf(BoostAnimation, this);

                    var pointRight = Utility.AngleFromPointAtDistance(base.Velocity.Angle + 180, new PointD(20, 20));
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
                var pointRight = Utility.AngleFromPointAtDistance(base.Velocity.Angle + 180, new PointD(20, 20));
                ThrustAnimation.Velocity.Angle.Degrees = this.Velocity.Angle.Degrees - 180;
                ThrustAnimation.X = this.X + pointRight.X;
                ThrustAnimation.Y = this.Y + pointRight.Y;
            }

            if (BoostAnimation != null && BoostAnimation.Visable)
            {
                var pointRight = Utility.AngleFromPointAtDistance(base.Velocity.Angle + 180, new PointD(20, 20));
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