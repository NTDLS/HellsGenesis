using HG.Actors.BaseClasses;
using HG.Actors.Ordinary;
using HG.Engine;
using HG.Types;
using System;
using System.Drawing;

namespace HG.Actors.Enemies.BaseClasses
{
    internal class EnemyPeonBase : EnemyBase
    {
        public ActorAnimation ThrustAnimation { get; internal set; }
        public ActorAnimation BoostAnimation { get; internal set; }

        public EnemyPeonBase(Core core, int hullHealth, int bountyMultiplier)
            : base(core, hullHealth, bountyMultiplier)
        {
            Velocity.ThrottlePercentage = 1;
            Initialize();

            OnVisibilityChanged += EnemyBase_OnVisibilityChanged;

            var playMode = new ActorAnimation.PlayMode()
            {
                Replay = ActorAnimation.ReplayMode.LoopedPlay,
                DeleteActorAfterPlay = false,
                ReplayDelay = new TimeSpan(0)
            };
            ThrustAnimation = new ActorAnimation(_core, @"Graphics\Animation\ThrustStandard32x32.png", new Size(32, 32), 10, playMode);

            ThrustAnimation.Reset();
            _core.Actors.Animations.CreateAt(ThrustAnimation, this);

            var pointRight = HgMath.AngleFromPointAtDistance(Velocity.Angle + 180, new HgPoint<double>(20, 20));
            ThrustAnimation.Velocity.Angle.Degrees = Velocity.Angle.Degrees - 180;
            ThrustAnimation.X = X + pointRight.X;
            ThrustAnimation.Y = Y + pointRight.Y;

            BoostAnimation = new ActorAnimation(_core, @"Graphics\Animation\ThrustBoost32x32.png", new Size(32, 32), 10, playMode);

            BoostAnimation.Reset();
            _core.Actors.Animations.CreateAt(BoostAnimation, this);

            pointRight = HgMath.AngleFromPointAtDistance(Velocity.Angle + 180, new HgPoint<double>(20, 20));
            BoostAnimation.Velocity.Angle.Degrees = Velocity.Angle.Degrees - 180;
            BoostAnimation.X = X + pointRight.X;
            BoostAnimation.Y = Y + pointRight.Y;
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

        public override void ApplyMotion(HgPoint<double> displacementVector)
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
