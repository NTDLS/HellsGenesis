using Newtonsoft.Json.Converters;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HG.Engine
{
    /// <summary>
    /// This contains all of the engine settings.
    /// </summary>
    internal class EngineSettings
    {
        #region Debug settings.
        public bool EnableActorsInterrogation { get; set; } = false;
        public bool HighlightNatrualBounds { get; set; } = false;
        public bool HighlightAllActors { get; set; } = false;
        #endregion

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

        public double MaxPlayerBoost { get; set; } = 10000;
        public double PlayerBoostRebuildMin { get; set; } = 1000;
        public double MaxRecoilPercentage { get; set; } = 0.5; //Max amount that will be substracted from the thrust percentage.
        public double MaxPlayerRotationSpeedDegrees { get; set; } = 2;

        public int InitialFrameStarCount { get; set; } = 100;
        public int DeltaFrameTargetStarCount { get; set; } = 200;

        public int MinEnemySpeed { get; set; } = 3;
        public int MaxEnemySpeed { get; set; } = 7;

        public double FrameLimiter { get; set; } = 120; //~120.0 seems to be a good rate. 2 frames per second?

        public double BulletSceneDistanceLimit { get; set; } = 800; //The distance from the scene that a bullet can travel before it is cleaned up.
        public double EnemySceneDistanceLimit { get; set; } = 5000; //The distance from the scene that a enemy can travel before it is cleaned up.

        /// <summary>
        /// How much larger than the screen (NatrualScreenSize) that we will make the canvas so we can zoom-out. (2 = 2x larger than screen.).
        /// </summary>
        public double OverdrawScale { get; set; } = 1.5;

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public InterpolationMode GraphicsScalingMode { get; set; } = InterpolationMode.NearestNeighbor;

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
