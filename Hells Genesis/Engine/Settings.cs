using System.Drawing.Drawing2D;

namespace HG.Engine
{
    /// <summary>
    /// This contains all of the engine settings.
    /// </summary>
    internal static class Settings
    {
        #region Debug settings.
        public static bool HighlightNatrualBounds { get; set; } = true;
        public static bool HighlightAllActors { get; set; } = false;
        #endregion

        public static bool LockPlayerAngleToNearbyEnemy { get; set; } = false;
        public static bool AutoZoomWhenMoving { get; set; } = true;

        public static double EnemyThrustRampUp { get; set; } = 0.05;
        public static double EnemyThrustRampDown { get; set; } = 0.01;

        public static double PlayerThrustRampUp { get; set; } = 0.05;
        public static double PlayerThrustRampDown { get; set; } = 0.01;

        public static int MaxHullHealth { get; set; } = 100000;
        public static int MaxShieldPoints { get; set; } = 100000;

        public static double MaxPlayerSpeed { get; set; } = 10;
        public static double MaxPlayerBoostSpeed { get; set; } = 5;
        public static double MaxPlayerBoost { get; set; } = 5000;
        public static double PlayerBoostRebuildMin { get; set; } = 1000;
        public static double MaxRecoilPercentage { get; set; } = 0.5; //Max amount that will be substracted from the thrust percentage.
        public static double MaxRotationSpeed { get; set; } = 2;

        public static int StartingPlayerHullHealth { get; set; } = 250;
        public static int StartingPlayerShieldHealth { get; set; } = 25;

        public static double MinPlayerThrust { get; set; } = 0; //0.25;

        public static int InitialStarCount { get; set; } = 100;
        public static int FramTargetStarCount { get; set; } = 200;

        public static int MinSpeed { get; set; } = 3;
        public static int MaxSpeed { get; set; } = 7;

        public static int MinEnemyHealth { get; set; } = 2;
        public static int MaxEnemyHealth { get; set; } = 20;

        public static double FrameLimiter { get; set; } = 100; //~100.0 seems to be a good rate.

        public static double BulletSceneDistanceLimit { get; set; } = 800; //The distance from the scene that a bullet can travel before it is cleaned up.
        public static double EnemySceneDistanceLimit { get; set; } = 5000; //The distance from the scene that a enemy can travel before it is cleaned up.

        public static double InfiniteScrollWallX { get; set; } = 200; //The size of the "box" where the player flies and where "infinite scrolling" begins.
        public static double InfiniteScrollWallY { get; set; } = 200; //The size of the "box" where the player flies and where "infinite scrolling" begins.

        /// <summary>
        /// How much larger than the screen (NatrualScreenSize) that we will make the canvas so we can zoom-out. (2 = 2x larger than screen.).
        /// </summary>
        public static double OverdrawScale { get; set; } = 1.2;

        public static InterpolationMode GraphicsScalingMode { get; set; } = InterpolationMode.NearestNeighbor;
    }
}
