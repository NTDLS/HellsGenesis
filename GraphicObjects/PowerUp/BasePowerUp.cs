using AI2D.Engine;
using AI2D.Types;
using System.Drawing;
using System.Linq;

namespace AI2D.GraphicObjects.PowerUp
{
    public class BasePowerUp : BaseGraphicObject
    {
        private const string _assetHitAnimationPath = @"..\..\Assets\Graphics\Animation\PowerUp\";
        private readonly string[] _assetHitAnimationFiles = {
            #region Image Paths.
            "PowerUp1.png",
            #endregion
        };

        private const string _assetExplosionSoundPath = @"..\..\Assets\Sounds\PowerUp\";
        private readonly string[] _assetExplosionSoundFiles = {
            #region Sound Paths.
            "PowerUp1.wav",
            #endregion
        };

        private AudioClip _explodeSound;

        private ObjAnimation _hitAnimation { get; set; }

        public BasePowerUp(Core core)
            : base(core)
        {
            Initialize();

            int _hitImageIndex = Utility.RandomNumber(0, _assetHitAnimationFiles.Count());
            _hitAnimation = new ObjAnimation(_core, _assetHitAnimationPath + _assetHitAnimationFiles[_hitImageIndex], new Size(111, 109), 50);

            int _soundIndex = Utility.RandomNumber(0, _assetExplosionSoundFiles.Count());
            _explodeSound = _core.Actors.GetSoundCached(_assetExplosionSoundPath + _assetExplosionSoundFiles[_soundIndex], 0.25f);
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }

        public new void Explode()
        {
            _explodeSound.Play();
            _hitAnimation.Reset();
            _core.Actors.PlaceAnimationOnTopOf(_hitAnimation, this);
            QueueForDelete();;
        }

        public virtual void ApplyIntelligence(PointD frameAppliedOffset)
        {
            if (Intersects(_core.Actors.Player))
            {
                Explode();
            }
        }
    }
}
