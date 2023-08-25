namespace HG.Types
{
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
        Clip, //Expensive...
        Upsize //Hella expensive!
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
