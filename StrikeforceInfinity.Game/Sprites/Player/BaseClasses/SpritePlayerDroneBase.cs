using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using static StrikeforceInfinity.Game.Sprites.SpriteAnimation;
using System.Drawing;
using StrikeforceInfinity.Game.Utility;
using System;

namespace StrikeforceInfinity.Game.Sprites.Player.BaseClasses
{
    /// <summary>
    /// The player base is a sub-class of the ship base. This is used as the enemy drone of the player in multiplay.
    /// </summary>
    internal class SpritePlayerDroneBase : _SpriteShipBase
    {
        public SpriteAnimation ThrustAnimation { get; internal set; }
        public SpriteAnimation BoostAnimation { get; internal set; }

        public HgPlayerClass ShipClass { get; set; }

        public SpritePlayerDroneBase(EngineCore gameCore)
            : base(gameCore)
        {
            var playMode = new SpriteAnimation.PlayMode()
            {
                Replay = HgAnimationReplayMode.LoopedPlay,
                DeleteSpriteAfterPlay = false,
                ReplayDelay = new TimeSpan(0)
            };

            ThrustAnimation = new SpriteAnimation(_gameCore, @"Graphics\Animation\ThrustStandard32x32.png", new Size(32, 32), 10, playMode);
            ThrustAnimation.Reset();
            _gameCore.Sprites.Animations.InsertAt(ThrustAnimation, this);

            BoostAnimation = new SpriteAnimation(_gameCore, @"Graphics\Animation\ThrustBoost32x32.png", new Size(32, 32), 10, playMode);
            BoostAnimation.Reset();
            _gameCore.Sprites.Animations.InsertAt(BoostAnimation, this);

            UpdateThrustAnimationPositions();
        }

        public override void PositionChanged() => UpdateThrustAnimationPositions();

        public override void RotationChanged() => UpdateThrustAnimationPositions();

        private void UpdateThrustAnimationPositions()
        {
            if (ThrustAnimation != null && ThrustAnimation.Visable)
            {
                var pointRight = HgMath.PointFromAngleAtDistance360(Velocity.Angle + 180, new SiPoint(20, 20));
                ThrustAnimation.Velocity.Angle.Degrees = Velocity.Angle.Degrees - 180;
                ThrustAnimation.X = X + pointRight.X;
                ThrustAnimation.Y = Y + pointRight.Y;
            }
            if (BoostAnimation != null && BoostAnimation.Visable)
            {
                var pointRight = HgMath.PointFromAngleAtDistance360(Velocity.Angle + 180, new SiPoint(20, 20));
                BoostAnimation.Velocity.Angle.Degrees = Velocity.Angle.Degrees - 180;
                BoostAnimation.X = X + pointRight.X;
                BoostAnimation.Y = Y + pointRight.Y;
            }
        }

        public virtual void ApplyIntelligence(SiPoint displacementVector)
        {
            //For the player class, this is only used for multiplay.
        }
    }
}
