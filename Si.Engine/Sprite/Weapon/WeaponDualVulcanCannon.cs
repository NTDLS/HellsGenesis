using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.Weapon
{
    internal class WeaponDualVulcanCannon : WeaponBase
    {
        static string Name { get; } = "Dual Vulcan Cannon";
        private const string soundPath = @"Sounds\Weapons\DualVulcanCannon.wav";
        private const float soundVolumne = 0.4f;

        public WeaponDualVulcanCannon(EngineCore engine, SpriteInteractiveBase owner)
            : base(engine, owner, Name, soundPath, soundVolumne)
        {
        }

        public WeaponDualVulcanCannon(EngineCore engine)
            : base(engine, Name, soundPath, soundVolumne)
        {
        }

        public override MunitionBase CreateMunition(SiVector location = null, SpriteInteractiveBase lockedTarget = null)
            => new MunitionVulcanCannon(_engine, this, Owner, location);

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();

                if (RoundQuantity > 0)
                {
                    var pointRight = Owner.Location + SiVector.PointFromAngleAtDistanceInUnsignedDegrees(
                        Owner.PointingAngle + SiMath.RADIANS_90, new SiVector(5, 5));
                    _engine.Sprites.Munitions.Add(this, pointRight);

                    var pointLeft = Owner.Location + SiVector.PointFromAngleAtDistanceInUnsignedDegrees(
                        Owner.PointingAngle - SiMath.RADIANS_90, new SiVector(5, 5));
                    _engine.Sprites.Munitions.Add(this, pointLeft);
                }

                RoundQuantity--;

                return true;
            }
            return false;
        }
    }
}
