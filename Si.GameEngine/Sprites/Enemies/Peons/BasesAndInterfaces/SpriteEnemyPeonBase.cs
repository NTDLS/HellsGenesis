using Si.GameEngine.Engine;
using Si.GameEngine.Sprites.Enemies.BasesAndInterfaces;
using Si.GameEngine.Utility;
using Si.Shared.Payload.DroneActions;
using Si.Shared.Types.Geometry;
using System;
using System.Drawing;
using static Si.Shared.SiConstants;

namespace Si.GameEngine.Sprites.Enemies.Peons.BasesAndInterfaces
{
    /// <summary>
    /// Base class for "Peon" enemies. These guys are basically all the same in theit functionality and animations.
    /// </summary>
    internal class SpriteEnemyPeonBase : SpriteEnemyBase
    {
        public SpriteAnimation ThrustAnimation { get; internal set; }
        public SpriteAnimation BoostAnimation { get; internal set; }

        public SpriteEnemyPeonBase(EngineCore gameCore, int hullHealth, int bountyMultiplier)
            : base(gameCore, hullHealth, bountyMultiplier)
        {
            Velocity.ThrottlePercentage = 1;
            Initialize();

            OnVisibilityChanged += EnemyBase_OnVisibilityChanged;

            var playMode = new SpriteAnimation.PlayMode()
            {
                Replay = SiAnimationReplayMode.LoopedPlay,
                DeleteSpriteAfterPlay = false,
                ReplayDelay = new TimeSpan(0)
            };

            ThrustAnimation = new SpriteAnimation(_gameCore, @"Graphics\Animation\ThrustStandard32x32.png", new Size(32, 32), 10, playMode)
            {
                IsFixedPosition = true,
                OwnerUID = UID
            };
            ThrustAnimation.Reset();
            _gameCore.Sprites.Animations.InsertAt(ThrustAnimation, this);

            BoostAnimation = new SpriteAnimation(_gameCore, @"Graphics\Animation\ThrustBoost32x32.png", new Size(32, 32), 10, playMode)
            {
                IsFixedPosition = true,
                OwnerUID = UID
            };
            BoostAnimation.Reset();
            _gameCore.Sprites.Animations.InsertAt(BoostAnimation, this);

            UpdateThrustAnimationPositions();
        }

        public override void PositionChanged() => UpdateThrustAnimationPositions();

        private void UpdateThrustAnimationPositions()
        {
            if (ThrustAnimation != null && ThrustAnimation.Visable)
            {
                var pointRight = SiMath.PointFromAngleAtDistance360(Velocity.Angle + 180, new SiPoint(20, 20));
                ThrustAnimation.Velocity.Angle.Degrees = Velocity.Angle.Degrees - 180;
                ThrustAnimation.LocalX = LocalX + pointRight.X;
                ThrustAnimation.LocalY = LocalY + pointRight.Y;
            }
            if (BoostAnimation != null && BoostAnimation.Visable)
            {
                var pointRight = SiMath.PointFromAngleAtDistance360(Velocity.Angle + 180, new SiPoint(20, 20));
                BoostAnimation.Velocity.Angle.Degrees = Velocity.Angle.Degrees - 180;
                BoostAnimation.LocalX = LocalX + pointRight.X;
                BoostAnimation.LocalY = LocalY + pointRight.Y;
            }
        }

        private void EnemyBase_OnVisibilityChanged(SpriteBase sender)
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

        /// <summary>
        /// Moves the sprite based on its thrust/boost (velocity) taking into account the background scroll.
        /// </summary>
        /// <param name="displacementVector"></param>
        public override void ApplyMotion(SiPoint displacementVector)
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

        /// <summary>
        /// Applies the multiplay position of the sprite as dictated by a remote game client.
        /// </summary>
        /// <param name="vector"></param>
        public override void ApplyMultiplayVector(SiDroneActionVector vector)
        {
            ThrustAnimation.Visable = vector.ThrottlePercentage > 0;
            BoostAnimation.Visable = vector.BoostPercentage > 0;
            base.ApplyMultiplayVector(vector);
        }

        public override void Cleanup()
        {
            ThrustAnimation?.QueueForDelete();
            BoostAnimation?.QueueForDelete();

            base.Cleanup();
        }
    }
}
