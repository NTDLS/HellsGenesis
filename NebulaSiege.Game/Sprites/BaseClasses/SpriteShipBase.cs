using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Engine.Types;
using NebulaSiege.Game.Engine.Types.Geometry;
using NebulaSiege.Game.Utility;
using NebulaSiege.Game.Weapons.Munitions;
using System.Drawing;
using System.IO;

namespace NebulaSiege.Game.Sprites
{
    /// <summary>
    /// The ship base is a ship object that moves, can be hit, explodes and can be the subject of locking weapons.
    /// </summary>
    internal class _SpriteShipBase : SpriteBase
    {
        public SpriteRadarPositionIndicator RadarPositionIndicator { get; protected set; }
        public SpriteRadarPositionTextBlock RadarPositionText { get; protected set; }
        public NsTimeRenewableResources RenewableResources { get; set; } = new();

        private readonly string _assetPathlockedOnImage = @"Graphics\Weapon\Locked On.png";
        private readonly string _assetPathlockedOnSoftImage = @"Graphics\Weapon\Locked Soft.png";
        private readonly string _assetPathHitSound = @"Sounds\Ship\Object Hit.wav";
        private readonly string _assetPathshieldHit = @"Sounds\Ship\Shield Hit.wav";

        private const string _assetPathExplosionAnimation = @"Graphics\Animation\Explode\Explosion 256x256\";
        private readonly int _explosionAnimationCount = 6;
        private int _selectedExplosionAnimationIndex = 0;

        private const string _assetPathHitExplosionAnimation = @"Graphics\Animation\Explode\Hit Explosion 22x22\";
        private readonly int _hitExplosionAnimationCount = 2;
        private int _selectedHitExplosionAnimationIndex = 0;

        private const string _assetExplosionSoundPath = @"Sounds\Explode\";
        private readonly int _explosionSoundCount = 4;
        private int _selectedExplosionSoundIndex = 0;

        public _SpriteShipBase(EngineCore core, string name = "")
            : base(core, name)
        {
            _core = core;
        }

        public override void Initialize(string imagePath = null, Size? size = null)
        {
            _hitSound = _core.Assets.GetAudio(_assetPathHitSound, 0.5f);
            _shieldHit = _core.Assets.GetAudio(_assetPathshieldHit, 0.5f);

            _selectedExplosionSoundIndex = HgRandom.Generator.Next(0, 1000) % _explosionSoundCount;
            _explodeSound = _core.Assets.GetAudio(Path.Combine(_assetExplosionSoundPath, $"{_selectedExplosionSoundIndex}.wav"), 1.0f);

            _selectedExplosionAnimationIndex = HgRandom.Generator.Next(0, 1000) % _explosionAnimationCount;
            _explosionAnimation = new SpriteAnimation(_core, Path.Combine(_assetPathExplosionAnimation, $"{_selectedExplosionAnimationIndex}.png"), new Size(256, 256));

            _selectedHitExplosionAnimationIndex = HgRandom.Generator.Next(0, 1000) % _hitExplosionAnimationCount;
            _hitExplosionAnimation = new SpriteAnimation(_core, Path.Combine(_assetPathHitExplosionAnimation, $"{_selectedHitExplosionAnimationIndex}.png"), new Size(22, 22));

            _lockedOnImage = _core.Assets.GetBitmap(_assetPathlockedOnImage);
            _lockedOnSoftImage = _core.Assets.GetBitmap(_assetPathlockedOnSoftImage);

            base.Initialize(imagePath, size);
        }

        public override void Explode()
        {
            _explodeSound?.Play();
            _explosionAnimation?.Reset();
            _core.Sprites.Animations.InsertAt(_explosionAnimation, this);
            base.Explode();
        }

        public void CreateParticlesExplosion()
        {
            _core.Sprites.Particles.CreateRandomShipPartParticlesAt(this, HgRandom.Between(30, 50));
            _core.Audio.PlayRandomExplosion();
        }

        /// <summary>
        /// Allows for the testing of hits from a munition. This is called for each movement along a munitions path.
        /// </summary>
        /// <param name="displacementVector">The background offset vector.</param>
        /// <param name="munition">The munition object that is being tested for.</param>
        /// <param name="hitTestPosition">The position to test for hit.</param>
        /// <returns></returns>
        public virtual bool TryMunitionHit(NsPoint displacementVector, MunitionBase munition, NsPoint hitTestPosition)
        {
            if (Intersects(hitTestPosition))
            {
                Hit(munition);
                if (HullHealth <= 0)
                {
                    Explode();
                }
                return true;
            }
            return false;
        }

        public override void Cleanup()
        {
            if (RadarPositionIndicator != null)
            {
                RadarPositionIndicator.QueueForDelete();
                RadarPositionText.QueueForDelete();
            }

            base.Cleanup();
        }
    }
}
