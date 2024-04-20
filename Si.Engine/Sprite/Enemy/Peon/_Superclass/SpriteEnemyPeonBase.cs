using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Library.Mathematics;
using System;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite.Enemy.Peon._Superclass
{
    /// <summary>
    /// Base class for "Peon" enemies. These guys are basically all the same in theit functionality and animations.
    /// </summary>
    internal class SpriteEnemyPeonBase : SpriteEnemyBase
    {
        public SpriteAnimation ThrusterAnimation { get; internal set; }
        public SpriteAnimation BoosterAnimation { get; internal set; }

        public SpriteEnemyPeonBase(EngineCore engine, string imagePath)
            : base(engine, imagePath)
        {
            RecalculateMovementVector();

            OnVisibilityChanged += EnemyBase_OnVisibilityChanged;

            var playMode = new SpriteAnimation.PlayMode()
            {
                ReplyMode = SiAnimationReplayMode.Infinite,
                DeleteSpriteAfterPlay = false,
                ReplayDelay = new TimeSpan(0)
            };

            ThrusterAnimation = new SpriteAnimation(_engine, @"Sprites\Animation\ThrustStandard32x32.png")
            {
                OwnerUID = UID
            };
            ThrusterAnimation.Reset();
            _engine.Sprites.Animations.Insert(ThrusterAnimation, this);

            BoosterAnimation = new SpriteAnimation(_engine, @"Sprites\Animation\ThrustBoost32x32.png")
            {
                OwnerUID = UID
            };
            BoosterAnimation.Reset();
            _engine.Sprites.Animations.Insert(BoosterAnimation, this);

            UpdateThrustAnimationPositions();
        }

        public override void LocationChanged() => UpdateThrustAnimationPositions();

        private void UpdateThrustAnimationPositions()
        {
            var pointBehind = (Orientation * -1) * new SiVector(20, 20);

            if (ThrusterAnimation != null && ThrusterAnimation.Visable)
            {
                ThrusterAnimation.Orientation = Orientation;
                ThrusterAnimation.Location = Location + pointBehind;
            }
            if (BoosterAnimation != null && BoosterAnimation.Visable)
            {
                BoosterAnimation.Orientation = Orientation;
                BoosterAnimation.Location = Location + pointBehind;
            }
        }

        private void EnemyBase_OnVisibilityChanged(SpriteBase sender)
        {
            if (ThrusterAnimation != null)
            {
                ThrusterAnimation.Visable = false;
            }
            if (BoosterAnimation != null)
            {
                BoosterAnimation.Visable = false;
            }
        }

        /// <summary>
        /// Moves the sprite based on its thrust/boost (velocity).
        /// </summary>
        /// <param name="displacementVector"></param>
        public override void ApplyMotion(float epoch, SiVector displacementVector)
        {
            base.ApplyMotion(epoch, displacementVector);

            if (ThrusterAnimation != null)
            {
                ThrusterAnimation.Visable = MovementVector.Sum() > 0;
            }
            if (BoosterAnimation != null)
            {
                BoosterAnimation.Visable = Throttle > 0;
            }
        }

        public override void Cleanup()
        {
            ThrusterAnimation?.QueueForDelete();
            BoosterAnimation?.QueueForDelete();

            base.Cleanup();
        }
    }
}
