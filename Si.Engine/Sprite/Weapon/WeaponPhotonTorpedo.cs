using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Weapon
{
    internal class WeaponPhotonTorpedo : WeaponBase
    {
        static string Name { get; } = "Photon Torpedo";
        private const string soundPath = @"Sounds\Weapons\PhotonTorpedo.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponPhotonTorpedo(EngineCore engine, SpriteInteractiveBase owner)
            : base(engine, owner, Name, soundPath, soundVolumne)
        {
        }

        public WeaponPhotonTorpedo(EngineCore engine)
            : base(engine, Name, soundPath, soundVolumne)
        {
        }

        public override MunitionBase CreateMunition(SiVector location = null, SpriteInteractiveBase lockedTarget = null)
            => new MunitionPhotonTorpedo(_engine, this, Owner, location);

        public override bool Fire()
        {
            if (CanFire)
            {
                _engine.Rendering.AddScreenShake(4, 100);
                _fireSound.Play();
                RoundQuantity--;

                if (_toggle)
                {
                    var pointRight = Owner.Location
                        + (Owner.Orientation + SiMath.RADIANS_90).PointFromAngleAtDistance(new SiVector(10, 10));
                    _engine.Sprites.Munitions.Add(this, pointRight);
                }
                else
                {
                    var pointLeft = Owner.Location
                        + (Owner.Orientation - SiMath.RADIANS_90).PointFromAngleAtDistance(new SiVector(10, 10));
                    _engine.Sprites.Munitions.Add(this, pointLeft);
                }

                _toggle = !_toggle;

                return true;
            }
            return false;

        }
    }
}
