using System;

namespace HG.Engine
{
    internal static class Constants
    {
        public static readonly TimeSpan OneMilisecond = TimeSpan.FromMilliseconds(1);

        public enum RelativeDirection
        {
            None,
            Left,
            Right
        }

        public enum CardinalDirection
        {
            None,
            North,
            East,
            South,
            West
        }

        public enum PlayerClass
        {
            Debug = 0,
            Nimitz = 1,
            Knox = 2,
            Luhu = 3,
            Atlant = 4,
            Whidbey = 5,
            Kirov = 6
        }
    }
}
