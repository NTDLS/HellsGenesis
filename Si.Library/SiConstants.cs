namespace Si.Library
{
    public static class SiConstants
    {
        public static string FriendlyName = "Strikeforce Infinite";

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
            Triangle
        }

        public enum ParticleColorType
        {
            SingleColor,
            Graident
        }

        public enum ParticleVectorType
        {
            /// <summary>
            /// Travel on the angle that is baked into the sprite Velocity.Angle.
            /// </summary>
            Native,
            /// <summary>
            /// Travel on an angle that is independent of the sprites Velocity.Angle.
            /// </summary>
            Independent
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

        public enum SiRelativeDirection
        {
            None,
            Left,
            Right
        }

        public enum SiCardinalDirection
        {
            None,
            North,
            East,
            South,
            West
        }

        public enum SiPlayerClass
        {
            Debug,
            Frigate,
            Cruiser,
            Destroyer,
            Dreadnaught,
            Reaver,
            Serpent,
            Starfighter
        }

        public enum SiEnemyClass
        {
            Debug,
            AITracer,
            Serf,
            Phoenix,
            Minnow,
            Merc,
            Scav,
            Devastator,
            Repulsor,
            Spectre,
            Garrison,
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
            SinglePlay,
            LoopedPlay
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
            StrafeLeft,
            StrafeRight,
            SpeedBoost,
            Forward,
            Reverse,
            PrimaryFire,
            SecondaryFire,
            RotateClockwise,
            RotateCounterClockwise,
            Escape,
            Left,
            Right,
            Up,
            Down,
            Enter
        }
    }
}
