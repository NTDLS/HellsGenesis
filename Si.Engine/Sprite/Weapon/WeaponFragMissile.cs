using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.Weapon
{
    internal class WeaponFragMissile : WeaponBase
    {
        static new string Name { get; } = "Frag Missile";
        private const string soundPath = @"Sounds\Weapons\FragMissile.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponFragMissile(EngineCore engine, SpriteShipBase owner)
            : base(engine, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponFragMissile(EngineCore engine)
            : base(engine, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 10;
            FireDelayMilliseconds = 250;
            Speed = 8.25f;
            SpeedVariancePercent = 0.10f;

            CanLockOn = false;
            ExplodesOnImpact = true;
        }

        public override MunitionBase CreateMunition(SiPoint location = null, float? angle = null, SpriteBase lockedTarget = null)
            => new MunitionFragMissile(_engine, this, Owner, location, angle);

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();
                RoundQuantity--;

                if (LockedTargets == null || LockedTargets.Count == 0)
                {
                    if (_toggle)
                    {
                        var pointRight = Owner.Location + SiPoint.PointFromAngleAtDistance360(Owner.Velocity.ForwardAngle + SiPoint.RADIANS_90, new SiPoint(10, 10));
                        _engine.Sprites.Munitions.Create(this, pointRight);
                    }
                    else
                    {
                        var pointLeft = Owner.Location + SiPoint.PointFromAngleAtDistance360(Owner.Velocity.ForwardAngle - SiPoint.RADIANS_90, new SiPoint(10, 10));
                        _engine.Sprites.Munitions.Create(this, pointLeft);
                    }

                    _toggle = !_toggle;
                }

                return true;
            }

            return false;
        }
    }
}
