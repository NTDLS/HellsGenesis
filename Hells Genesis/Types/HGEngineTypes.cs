namespace HG.Types
{
    public enum HGKeyPressState
    {
        Up,
        Down
    }
    public enum HGFiredFromType
    {
        Unspecified,
        Player,
        Enemy
    }

    public enum HGRotationMode
    {
        None, //Almost free.
        Clip, //Expensive...
        Upsize //Hella expensive!
    }

    public enum HGPlayerKey
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
