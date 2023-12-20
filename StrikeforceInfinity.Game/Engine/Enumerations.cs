namespace StrikeforceInfinity.Game.Engine
{

    public enum HgControlledBy
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


    public enum HgPlayMode
    {
        SinglePlayer,
        MutiPlayerHost,
        MutiPlayerClient
    }

    public enum HgLevelState
    {
        NotYetStarted,
        Started,
        Ended
    }

    public enum HgSituationState
    {
        NotYetStarted,
        Started,
        Ended
    }

    public enum HgDrawingCacheType
    {
        Scaling,
        Screen,
        Radar
    }

    public enum HgRelativeDirection
    {
        None,
        Left,
        Right
    }

    public enum HgCardinalDirection
    {
        None,
        North,
        East,
        South,
        West
    }

    public enum HgPlayerClass
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

    public enum HgEnemyClass
    {
        Debug,
        AITracer,
        Irlen,
        Phoenix,
        Scinzad,
        Theda,
        Uves,
        Devastator,
        Repulsor,
        Spectre
    }

    public enum HgMenuItemType
    {
        Undefined,
        Title,
        Textblock,
        SelectableItem,
        SelectableTextInput
    }

    public enum HgAnimationReplayMode
    {
        SinglePlay,
        LoopedPlay
    };

    public enum HgDamageType
    {
        Unspecified,
        Shield,
        Hull
    }

    public enum HgFiredFromType
    {
        Unspecified,
        Player,
        Enemy
    }

    public enum HgRotationMode
    {
        None, //Almost free.
        Rotate
    }

    public enum HgPlayerKey
    {
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
