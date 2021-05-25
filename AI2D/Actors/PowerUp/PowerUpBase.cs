using AI2D.Engine;
using AI2D.Types;
using System;
using System.Drawing;
using System.Linq;

namespace AI2D.Actors.PowerUp
{
    public class PowerUpBase : ActorBase
    {
        private const string _assetHitAnimationPath = @"..\..\..\Assets\Graphics\Animation\PowerUp\";
        private readonly string[] _assetHitAnimationFiles = {
            #region Image Paths.
            "PowerUp1.png",
            #endregion
        };

        private const string _assetExplosionSoundPath = @"..\..\..\Assets\Sounds\PowerUp\";
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

        private AudioClip _explodeSound;

        private ActorAnimation _hitAnimation { get; set; }

        public PowerUpBase(Core core)
            : base(core)
        {
            Initialize();

            int _hitImageIndex = Utility.RandomNumber(0, _assetHitAnimationFiles.Count());
            _hitAnimation = new ActorAnimation(_core, _assetHitAnimationPath + _assetHitAnimationFiles[_hitImageIndex], new Size(111, 109), 50);

            int _soundIndex = Utility.RandomNumber(0, _assetExplosionSoundFiles.Count());
            _explodeSound = _core.Actors.GetSoundCached(_assetExplosionSoundPath + _assetExplosionSoundFiles[_soundIndex], 0.25f);

            this.RadarDotSize = new Point<int>(4, 4);
            this.RadarDotColor = Color.FromArgb(255, 255, 0);
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }

        public void Explode()
        {
            _explodeSound.Play();
            _hitAnimation.Reset();
            _core.Actors.PlaceAnimationOnTopOf(_hitAnimation, this);
            QueueForDelete();;
        }

        public virtual void ApplyIntelligence(Point<double> frameAppliedOffset)
        {
            if (Intersects(_core.Actors.Player))
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
