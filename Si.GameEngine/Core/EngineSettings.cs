using System;
using System.Drawing;
using System.Windows.Forms;

namespace Si.GameEngine.Core
{
    /// <summary>
    /// This contains all of the engine settings.
    /// </summary>
    public class EngineSettings
    {
        public int GraphicsAdapterId { get; set; } = 0;
        public int MunitionTraversalThreads { get; set; } = Environment.ProcessorCount * 2;
        public bool EnableSpriteInterrogation { get; set; } = false;
        public bool HighlightNatrualBounds { get; set; } = false;
        public bool HighlightAllSprites { get; set; } = false;

        public Size Resolution { get; set; }

        public bool PreCacheAllAssets { get; set; } = true;
        public bool FullScreen { get; set; }
        public bool AlwaysOnTop { get; set; }

        public bool PlayMusic { get; set; } = true;

        public bool LockPlayerAngleToNearbyEnemy { get; set; } = false;
        public bool EnableSpeedScaleFactoring { get; set; } = true;

        public float WorldTicksPerSecond { get; set; } = 120; //MillisecondPerEpochs = 1000 / WorldTicksPerSecond

        public float EnemyThrustRampUp { get; set; } = 0.0375f;
        public float EnemyThrustRampDown { get; set; } = 0.0075f;

        public float PlayerThrustRampUp { get; set; } = 0.0375f;
        public float PlayerThrustRampDown { get; set; } = 0.0075f;

        public int MaxHullHealth { get; set; } = 100000;
        public int MaxShieldPoints { get; set; } = 100000;

        public float MaxPlayerBoostAmount { get; set; } = 10000;
        public float PlayerBoostRebuildFloor { get; set; } = 1000;
        public float MaxRecoilPercentage { get; set; } = 0.4f; //Max amount that will be substracted from the thrust percentage.
        public float MaxPlayerRotationSpeedDegrees { get; set; } = 1.40f;

        public int InitialFrameStarCount { get; set; } = 100;
        public int DeltaFrameTargetStarCount { get; set; } = 200;

        public bool VerticalSync { get; set; } = false;
        public bool AntiAliasing { get; set; } = true;

        /// <summary>
        /// Ensure that the average framerate is within sane limits. This is especially important for vSync since we want to make sure a frame is availbele for the GPU.
        /// </summary>
        public bool FineTuneFramerate { get; set; } = true;
        public float TargetFrameRate { get; set; } = 70;
        public float MunitionSceneDistanceLimit { get; set; } = 2500; //The distance from the scene that a munition can travel before it is cleaned up.
        public float EnemySceneDistanceLimit { get; set; } = 5000; //The distance from the scene that a enemy can travel before it is cleaned up.

        /// <summary>
        /// How much larger than the screen (NatrualScreenSize) that we will make the canvas so we can zoom-out. (2 = 2x larger than screen.).
        /// </summary>
        public float OverdrawScale { get; set; } = 1.5f;

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
