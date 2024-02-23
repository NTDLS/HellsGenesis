using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;
using Si.Library;
using Si.Library.Types.Geometry;
using System;
using System.Drawing;
using System.Linq;

namespace Si.GameEngine.Sprites.Powerup._Superclass
{
    /// <summary>
    /// Represents a "power-up" that the player can pick up to gain some ability / stat-improvement.
    /// </summary>
    public class SpritePowerupBase : SpriteBase
    {
        private const string _assetHitAnimationPath = @"Graphics\Animation\Powerup\";
        private readonly string[] _assetHitAnimationFiles = {
            #region Image Paths.
            "PowerUpShort128x128.png",
            #endregion
        };

        private const string _assetExplosionSoundPath = @"Sounds\Powerup\";
        private readonly string[] _assetExplosionSoundFiles = {
            #region Sound Paths.
            "PowerUp1.wav",
            #endregion
        };


        /// <summary>
        /// The power up amount (number of boost points, shield points, repair, etc.).
        /// </summary>
        public int PowerupAmount { get; set; } = 1;

        /// <summary>
        /// Time until the powerup exploded on its own.
        /// </summary>
        public double TimeToLive { get; set; } = 30000;
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        public double AgeInMilliseconds
        {
            get
            {
                return (DateTime.UtcNow - CreationTime).TotalMilliseconds;
            }
        }

        public SpritePowerupBase(GameEngineCore gameEngine)
            : base(gameEngine)
        {
            Initialize();

            int _hitImageIndex = SiRandom.Between(0, _assetHitAnimationFiles.Count() - 1);
            _hitAnimation = new SpriteAnimation(_gameEngine, _assetHitAnimationPath + _assetHitAnimationFiles[_hitImageIndex], new Size(128, 128), 20);

            int _soundIndex = SiRandom.Between(0, _assetExplosionSoundFiles.Count() - 1);
            _explodeSound = _gameEngine.Assets.GetAudio(_assetExplosionSoundPath + _assetExplosionSoundFiles[_soundIndex], 0.25f);

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
            _gameEngine.Sprites.Animations.AddAt(_hitAnimation, this);
            QueueForDelete();
        }

        public virtual void ApplyIntelligence(double epoch, SiPoint displacementVector)
        {
            if (Intersects(_gameEngine.Player.Sprite))
            {
                Explode();
            }
            else if (AgeInMilliseconds > TimeToLive)
            {
                Explode();
            }
        }
    }
}
