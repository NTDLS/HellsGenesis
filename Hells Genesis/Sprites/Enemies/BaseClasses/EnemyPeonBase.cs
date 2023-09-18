using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Sprites.BaseClasses;
using HG.Utility;
using System;
using System.Drawing;

namespace HG.Sprites.Enemies.BaseClasses
{
    internal class EnemyPeonBase : EnemyBase
    {
        public ActorAnimation ThrustAnimation { get; internal set; }
        public ActorAnimation BoostAnimation { get; internal set; }

        public EnemyPeonBase(EngineCore core, int hullHealth, int bountyMultiplier)
            : base(core, hullHealth, bountyMultiplier)
        {
            Velocity.ThrottlePercentage = 1;
            Initialize();

            OnVisibilityChanged += EnemyBase_OnVisibilityChanged;

            var playMode = new ActorAnimation.PlayMode()
            {
                Replay = HgAnimationReplayMode.LoopedPlay,
                DeleteActorAfterPlay = false,
                ReplayDelay = new TimeSpan(0)
            };

            ThrustAnimation = new ActorAnimation(_core, @"Graphics\Animation\ThrustStandard32x32.png", new Size(32, 32), 10, playMode);
            ThrustAnimation.Reset();
            _core.Actors.Animations.InsertAt(ThrustAnimation, this);

            BoostAnimation = new ActorAnimation(_core, @"Graphics\Animation\ThrustBoost32x32.png", new Size(32, 32), 10, playMode);
            BoostAnimation.Reset();
            _core.Actors.Animations.InsertAt(BoostAnimation, this);

            UpdateThrustAnimationPositions();
        }

        public override void PositionChanged() => UpdateThrustAnimationPositions();

        private void UpdateThrustAnimationPositions()
        {
            if (ThrustAnimation != null && ThrustAnimation.Visable)
            {
                var pointRight = HgMath.AngleFromPointAtDistance(Velocity.Angle + 180, new HgPoint(20, 20));
                ThrustAnimation.Velocity.Angle.Degrees = Velocity.Angle.Degrees - 180;
                ThrustAnimation.X = X + pointRight.X;
                ThrustAnimation.Y = Y + pointRight.Y;
            }
            if (BoostAnimation != null && BoostAnimation.Visable)
            {
                var pointRight = HgMath.AngleFromPointAtDistance(Velocity.Angle + 180, new HgPoint(20, 20));
                BoostAnimation.Velocity.Angle.Degrees = Velocity.Angle.Degrees - 180;
                BoostAnimation.X = X + pointRight.X;
                BoostAnimation.Y = Y + pointRight.Y;
            }
        }

        private void EnemyBase_OnVisibilityChanged(ActorBase sender)
        {
            if (ThrustAnimation != null)
            {
                ThrustAnimation.Visable = false;
            }
            if (BoostAnimation != null)
            {
                BoostAnimation.Visable = false;
            }
        }

        public override void ApplyMotion(HgPoint displacementVector)
        {
            base.ApplyMotion(displacementVector);

            if (ThrustAnimation != null)
            {
                ThrustAnimation.Visable = Velocity.ThrottlePercentage > 0;
            }
            if (BoostAnimation != null)
            {
                BoostAnimation.Visable = Velocity.BoostPercentage > 0;
            }
        }

        public override void Cleanup()
        {
            ThrustAnimation?.QueueForDelete();
            BoostAnimation?.QueueForDelete();

            base.Cleanup();
        }
    }
}
