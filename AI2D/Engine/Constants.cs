namespace AI2D.Engine
{
    public static class Constants
    {
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
            Nimitz = 1,
            Knox = 2,
            Luhu = 3,
            Atlant = 4,
            Ticonderoga = 5,
            Kirov = 6
        }

        public const double PlayerThrustRampUp = 0.05;
        public const double PlayerThrustRampDown = 0.01;

        public static class Limits
        {
            public const int MaxHitpoints = 100000;
            public const int MaxShieldPoints = 100000;

            public const double MaxPlayerSpeed = 10;
            public const double MaxPlayerBoostSpeed = 5;
            public const double MaxPlayerBoost = 2500;

            public const double MaxRotationSpeed = 2.5;

            public const int StartingPlayerHitpoints = 250;
            public const int StartingPlayerShieldPoints = 25;

            public const double MinPlayerThrust = 0; //0.25;

            public const int MinSpeed = 3;
            public const int MaxSpeed = 7;

            public const int MinEnemyHealth = 2;
            public const int MaxEnemyHealth = 20;

            public const double FrameLimiter = 80.0; //80.0 seems to be a good rate.

            public const double BulletSceneDistanceLimit = 500; //The distance from the scene that a bullet can travel before it is cleaned up.
            public const double EnemySceneDistanceLimit = 5000; //The distance from the scene that a enemy can travel before it is cleaned up.

            public const double InfiniteScrollWallX = 200; //Where "infinite scrolling" begins.
            public const double InfiniteScrollWallY = 200; //Where "infinite scrolling" begins.
        }
    }
}
