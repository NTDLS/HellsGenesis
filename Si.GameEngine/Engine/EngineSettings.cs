using System.Drawing;
using System.Windows.Forms;

namespace Si.GameEngine.Engine
{
    /// <summary>
    /// This contains all of the engine settings.
    /// </summary>
    public class EngineSettings
    {
        public bool EnableSpriteInterrogation { get; set; } = false;
        public bool HighlightNatrualBounds { get; set; } = false;
        public bool HighlightAllSprites { get; set; } = false;

        public Size Resolution { get; set; }
        public bool FullScreen { get; set; }
        public bool AlwaysOnTop { get; set; }

        public bool LockPlayerAngleToNearbyEnemy { get; set; } = false;
        public bool AutoZoomWhenMoving { get; set; } = true;

        public double EnemyThrustRampUp { get; set; } = 0.05;
        public double EnemyThrustRampDown { get; set; } = 0.01;

        public double PlayerThrustRampUp { get; set; } = 0.05;
        public double PlayerThrustRampDown { get; set; } = 0.01;

        public int MaxHullHealth { get; set; } = 100000;
        public int MaxShieldPoints { get; set; } = 100000;

        public double MaxPlayerBoostAmount { get; set; } = 10000;
        public double PlayerBoostRebuildFloor { get; set; } = 1000;
        public double MaxRecoilPercentage { get; set; } = 0.4; //Max amount that will be substracted from the thrust percentage.
        public double MaxPlayerRotationSpeedDegrees { get; set; } = 2;

        public int InitialFrameStarCount { get; set; } = 100;
        public int DeltaFrameTargetStarCount { get; set; } = 200;

        public int MinEnemySpeed { get; set; } = 5;
        public int MaxEnemySpeed { get; set; } = 10;

        public double FramePerSecondLimit { get; set; } = 60; //~60 seems to be a good rate.
        public double MunitionSceneDistanceLimit { get; set; } = 1500; //The distance from the scene that a munition can travel before it is cleaned up.
        public double EnemySceneDistanceLimit { get; set; } = 5000; //The distance from the scene that a enemy can travel before it is cleaned up.

        /// <summary>
        /// How much larger than the screen (NatrualScreenSize) that we will make the canvas so we can zoom-out. (2 = 2x larger than screen.).
        /// </summary>
        public double OverdrawScale { get; set; } = 1.5;

        public EngineSettings()
        {
            int x = (int)(Screen.PrimaryScreen.Bounds.Width * 0.75);
            int y = (int)(Screen.PrimaryScreen.Bounds.Height * 0.75);
            if (x % 2 != 0) x++;
            if (y % 2 != 0) y++;
            Resolution = new Size(x, y);
        }
    }
}
