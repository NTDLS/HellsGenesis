using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Utility;
using System;
using System.Drawing;
using System.Linq;

namespace StrikeforceInfinity.Game.Sprites.PowerUp.BaseClasses
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

        public SpritePowerUpBase(EngineCore gameCore)
            : base(gameCore)
        {
            Initialize();

            int _hitImageIndex = HgRandom.Between(0, _assetHitAnimationFiles.Count());
            _hitAnimation = new SpriteAnimation(_gameCore, _assetHitAnimationPath + _assetHitAnimationFiles[_hitImageIndex], new Size(128, 128), 20);

            int _soundIndex = HgRandom.Between(0, _assetExplosionSoundFiles.Count());
            _explodeSound = _gameCore.Assets.GetAudio(_assetExplosionSoundPath + _assetExplosionSoundFiles[_soundIndex], 0.25f);

            RadarDotSize = new SiPoint(4, 4);
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }

        public override void Explode()
        {
            _explodeSound.Play();
            _hitAnimation.Reset();
            _gameCore.Sprites.Animations.InsertAt(_hitAnimation, this);
            QueueForDelete();
        }

        public virtual void ApplyIntelligence(SiPoint displacementVector)
        {
            if (Intersects(_gameCore.Player.Sprite))
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
