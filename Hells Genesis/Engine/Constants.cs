using System;

namespace HG.Engine
{
    /// <summary>
    /// A place for engine constants.
    /// </summary>
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
            Frigate = 1,
            Cruiser = 2,
            Destroyer = 3,
            Dreadnaught = 4,
            Reaver = 5,
            Serpent = 6,
            Starfighter = 7
        }
    }
}
