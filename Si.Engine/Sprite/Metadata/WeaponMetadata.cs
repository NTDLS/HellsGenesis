namespace Si.GameEngine.Sprite.Metadata
{
    /// <summary>
    /// Contains sprite metadata.
    /// </summary>
    public class WeaponMetadata
    {
        public string Name { get; set; }
        public string Description { get; set; }

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
        public bool CanLockOn { get; set; } = false;
        public int MaxLockOnAngle { get; set; } = 10;
        public int MaxLocks { get; set; } = 1;
        public float MinLockDistance { get; set; } = 50;
        public float MaxLockDistance { get; set; } = 100;
        public bool ExplodesOnImpact { get; set; } = false;

    }
}
