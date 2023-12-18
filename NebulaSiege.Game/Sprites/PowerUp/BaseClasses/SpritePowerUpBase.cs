using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Engine.Types.Geometry;
using NebulaSiege.Game.Utility;
using System;
using System.Drawing;
using System.Linq;

namespace NebulaSiege.Game.Sprites.PowerUp.BaseClasses
{
    /// <summary>
    /// Represents a "power-up" that the player can pick up to gain some ability / stat-improvement.
    /// </summary>
    internal class SpritePowerUpBase : SpriteBase
    {
        private const string _assetHitAnimationPath = @"Graphics\Animation\PowerUp\";
        private readonly string[] _assetHitAnimationFiles = {
            #region Image Paths.
            "PowerUpShort128x128.png",
            #endregion
        };

        private const string _assetExplosionSoundPath = @"Sounds\PowerUp\";
        private readonly string[] _assetExplosionSoundFiles = {
            #region Sound Paths.
            "PowerUp1.wav",
            #endregion
        };

        /// <summary>
        /// Time until the powerup exploded on its own.
        /// </summary>
        public double TimeToLive { get; set; } = 30000;
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        public double AgeInMiliseconds
        {
            get
            {
                return (DateTime.UtcNow - CreationTime).TotalMilliseconds;
            }
        }

        public SpritePowerUpBase(EngineCore core)
            : base(core)
        {
            Initialize();

            int _hitImageIndex = HgRandom.Between(0, _assetHitAnimationFiles.Count());
            _hitAnimation = new SpriteAnimation(_core, _assetHitAnimationPath + _assetHitAnimationFiles[_hitImageIndex], new Size(128, 128), 20);

            int _soundIndex = HgRandom.Between(0, _assetExplosionSoundFiles.Count());
            _explodeSound = _core.Assets.GetAudio(_assetExplosionSoundPath + _assetExplosionSoundFiles[_soundIndex], 0.25f);

            RadarDotSize = new NsPoint(4, 4);
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }

        public override void Explode()
        {
            _explodeSound.Play();
            _hitAnimation.Reset();
            _core.Sprites.Animations.InsertAt(_hitAnimation, this);
            QueueForDelete();
        }

        public virtual void ApplyIntelligence(NsPoint displacementVector)
        {
            if (Intersects(_core.Player.Sprite))
            {
                Explode();
            }
            else if (AgeInMiliseconds > TimeToLive)
            {
                Explode();
            }
        }
    }
}
