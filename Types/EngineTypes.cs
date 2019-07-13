namespace AI2D.Types
{
    public enum KeyPressState
    {
        Up,
        Down
    }
    public enum FiredFromType
    {
        Unspecified,
        Player,
        Enemy
    }

    public enum RotationMode
    {
        None, //Almost free.
        Clip, //Expensive...
        Upsize //Hella expensive!
    }

    public enum PlayerKey
    {
        Forward,
        Reverse,
        Fire,
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
