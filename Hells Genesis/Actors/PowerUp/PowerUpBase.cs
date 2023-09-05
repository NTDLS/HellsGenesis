using HG.Engine;
using HG.Types;
using System;
using System.Drawing;
using System.Linq;

namespace HG.Actors.PowerUp
{
    internal class PowerUpBase : ActorBase
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

        private readonly AudioClip _explodeSound;

        private ActorAnimation _hitAnimation { get; set; }

        public PowerUpBase(Core core)
            : base(core)
        {
            Initialize();

            int _hitImageIndex = HgRandom.RandomNumber(0, _assetHitAnimationFiles.Count());
            _hitAnimation = new ActorAnimation(_core, _assetHitAnimationPath + _assetHitAnimationFiles[_hitImageIndex], new Size(128, 128), 20);

            int _soundIndex = HgRandom.RandomNumber(0, _assetExplosionSoundFiles.Count());
            _explodeSound = _core.Audio.Get(_assetExplosionSoundPath + _assetExplosionSoundFiles[_soundIndex], 0.25f);

            RadarDotSize = new HgPoint<int>(4, 4);
            RadarDotColor = Color.FromArgb(255, 255, 0);
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }

        public void Explode()
        {
            _explodeSound.Play();
            _hitAnimation.Reset();
            _core.Actors.Animations.CreateAt(_hitAnimation, this);
            QueueForDelete(); ;
        }

        public virtual void ApplyIntelligence(HgPoint<double> displacementVector)
        {
            if (Intersects(_core.Player.Actor))
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
