namespace AI2D.Engine
{
    public static class Constants
    {
        public const double PlayerThrustRampUp = 0.05;
        public const double PlayerThrustRampDown = 0.01;

        public static class Limits
        {
            public const int MinSpeed = 1;
            public const int MaxSpeed = 2;

            public const int MinEnemyHealth = 2;
            public const int MaxEnemyHealth = 10;

            public const double FrameLimiter = 80.0;

            public const double BulletSceneDistanceLimit = 500; //The distance from the scene that a bullet can travel before it is cleaned up.

            public const double InfiniteScrollWall = 400; //Where "infinite scrolling" begins.
        }

    }
}
