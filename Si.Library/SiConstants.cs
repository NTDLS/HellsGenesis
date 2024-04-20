namespace Si.Library
{
    public static class SiConstants
    {
        public static string FriendlyName = "Strikeforce Infinite";

        /// <summary>
        /// Determines the behaivior of a attachment sprite's position.
        /// </summary>
        public enum AttachmentPositionType
        {
            /// <summary>
            /// The attached sprite's position will automatically stay at a fixed position on the owner sprite, even when the owner moves and rotates.
            /// Managed in ApplyMotion().
            /// </summary>
            FixedToOwner,

            /// <summary>
            /// The attached sprite's position will not be automatically managed by ApplyMotion().
            /// </summary>
            Independent
        }

        /// <summary>
        /// Determines the behaivior of a attachment sprite's orientation.
        /// </summary>
        public enum AttachmentOrientationType
        {
            /// <summary>
            /// The attached sprite should always face the direction of the owner sprite. Managed in ApplyMotion().
            /// </summary>
            FixedToOwner,

            /// <summary>
            /// The attached sprite's orientation will not be automatically managed by ApplyMotion().
            /// </summary>
            Independent
        }

        public enum SiEngineInitilizationType
        {
            Play,
            Edit
        }

        public enum SiWeaponsLockType
        {
            None,
            Hard,
            Soft
        }

        public enum ParticleCleanupMode
        {
            None,
            FadeToBlack,
            DistanceOffScreen
        }

        public enum ParticleShape
        {
            FilledEllipse,
            HollowEllipse,
            HollowRectangle,
            FilledRectangle,
            Triangle
        }

        public enum ParticleColorType
        {
            Solid,
            Graident
        }

        public enum ParticleVectorType
        {
            /// <summary>
            /// The sprite will travel in the direction determined by it's MovementVector.
            /// </summary>
            Default,
            /// <summary>
            /// The sprite will travel in the direction in which is is oriented.
            /// </summary>
            FollowOrientation
        }

        public enum SiRenderScaleOrder
        {
            /// <summary>
            /// Render this sprite before scaling the screen based on speed (the sprite will be scaled).
            /// </summary>
            PreScale,
            /// <summary>
            /// Render this sprite after scaling the screen based on speed (the sprite will not be scaled).
            /// </summary>
            PostScale
        }

        public enum SiLevelState
        {
            NotYetStarted,
            Started,
            Ended
        }

        public enum SiSituationState
        {
            NotYetStarted,
            Started,
            Ended
        }

        public enum SiCardinalDirection
        {
            None,
            North,
            East,
            South,
            West
        }

        public enum SiMenuItemType
        {
            Undefined,
            Title,
            Textblock,
            SelectableItem,
            SelectableTextInput
        }

        public enum SiAnimationReplayMode
        {
            Single,
            Infinite
        };

        public enum SiDamageType
        {
            Unspecified,
            Shield,
            Hull
        }

        public enum SiFiredFromType
        {
            Unspecified,
            Player,
            Enemy
        }

        public enum SiPlayerKey
        {
            SwitchWeaponLeft,
            SwitchWeaponRight,
            StrafeRight,
            StrafeLeft,
            SpeedBoost,
            Forward,
            Reverse,
            PrimaryFire,
            SecondaryFire,
            RotateCounterClockwise,
            RotateClockwise,
            Escape,
            Left,
            Right,
            Up,
            Down,
            Enter
        }
    }
}
