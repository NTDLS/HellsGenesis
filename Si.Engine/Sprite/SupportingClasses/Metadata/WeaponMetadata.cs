using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Sprite.SupportingClasses.Metadata
{
    /// <summary>
    /// Contains sprite metadata.
    /// </summary>
    public class WeaponMetadata
    {
        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// If the sprite has an image, these are the paths to the bitmaps (be default, they are used at random)..
        /// </summary>
        public string[] SpritePaths { get; set; }

        public string SoundPath { get; set; }
        public float SoundVolume { get; set; } = 1.0f;

        /// <summary>
        /// The variance in degrees that the loaded munition will use for an initial heading angle.
        /// </summary>
        public float AngleVarianceDegrees { get; set; } = 0;
        /// <summary>
        /// The variance expressed in decimal percentage that determines the loaded munitions initial velovity.
        /// </summary>
        public float SpeedVariancePercent { get; set; } = 0;
        /// <summary>
        /// The distance from the total canvas that the munition will be allowed to travel before it is deleted.
        /// </summary>
        public float MunitionSceneDistanceLimit { get; set; }
        public float Speed { get; set; } = 25;

        public int FireDelayMilliseconds { get; set; } = 100;
        public int Damage { get; set; } = 1;

        public int MaxLockOnAngle { get; set; } = 10;
        public int MaxLocks { get; set; } = 1;
        public float MinLockDistance { get; set; } = 50;
        public float MaxLockDistance { get; set; } = 100;
        public bool ExplodesOnImpact { get; set; } = false;

        [JsonConverter(typeof(StringEnumConverter))]
        public MunitionType MunitionType { get; set; }

        /// <summary>
        /// The viewing angle that the munition will use for seeking/locking.
        /// </summary>
        public int GuidanceAngle { get; set; } = 20;

        /// <summary>
        /// The viewing distance that the munition will use for seeking/locking.
        /// </summary>
        public int GuidanceDistance { get; set; } = 20;

        /// <summary>
        /// Rate in degrees that the munition will rotate towards it target.
        /// </summary>
        public float GuidanceRotation { get; set; } = 0.25f;
    }
}
