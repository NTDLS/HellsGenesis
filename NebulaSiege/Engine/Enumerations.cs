namespace NebulaSiege.Engine
{
    public enum HgSituationState
    {
        NotStarted,
        Running,
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
        Debug = 0,
        Frigate = 1,
        Cruiser = 2,
        Destroyer = 3,
        Dreadnaught = 4,
        Reaver = 5,
        Serpent = 6,
        Starfighter = 7
    }

    public enum HgMenuItemType
    {
        Title,
        Text,
        Item
    }

    public enum HgAnimationReplayMode
    {
        SinglePlay,
        LoopedPlay
    };

    public enum HgKeyPressState
    {
        Up,
        Down
    }

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
