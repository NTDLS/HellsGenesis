namespace Si.Library
{
    public static class SiConstants
    {
        public static string FriendlyName = "Strikeforce Infinity";
        public const string MultiplayServerAddress = "127.0.0.1";
        public const int MultiplayServerTCPPort = 6785;

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
            Distance
        }

        public enum ParticleShape
        {
            FilledEllipse,
            HollowEllipse,
            Triangle
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

        public enum SiPlayMode
        {
            SinglePlayer,
            MutiPlayerHost,
            MutiPlayerClient
        }

        public enum SiLogSeverity
        {
            Trace = 0, //Super-verbose, debug-like information.
            Verbose = 1, //General status messages.
            Warning = 2, //Something the user might want to be aware of.
            Exception = 3 //An actual exception has been thrown.
        }

        public enum SiControlledBy
        {
            /// <summary>
            /// This sprite is controlled by local AI and the actions are sent to the
            /// server to control the its drone clones sprite with a matching MultiplayUID.
            /// </summary>
            LocalAI,

            /// <summary>
            /// This sprite is controlled by a local human player.
            /// </summary>
            HumanPlayer,

            /// <summary>
            /// This ship is controlled by messages from the server and not by local AI and not by the local player..
            /// </summary>
            Server
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

        public enum SiDrawingCacheType
        {
            Scaling,
            Screen,
            Radar
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
            Spectre
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

        public enum SiRotationMode
        {
            None, //Almost free.
            Rotate
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
