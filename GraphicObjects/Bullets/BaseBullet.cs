using AI2D.Engine;
using AI2D.GraphicObjects.Enemies;
using AI2D.Types;
using AI2D.Weapons;
using System;
using System.Drawing;
using System.Linq;

namespace AI2D.GraphicObjects.Bullets
{
    public class BaseBullet : BaseGraphicObject
    {
        public FiredFromType FiredFromType { get; set; }
        public WeaponBase Weapon { get; private set; }
        public BaseGraphicObject LockedTarget { get; private set; }
        public ObjAnimation _hitExplosionAnimation { get; set; }
        public DateTime CreatedDate { get; private set; } = DateTime.UtcNow;
        public double MaxAgeInMilliseconds { get; set; } = 5000;

        public double AgeInMilliseconds
        {
            get
            {
                return (DateTime.UtcNow - CreatedDate).TotalMilliseconds;
            }
        }

        private const string _assetHitExplosionAnimationPath = @"..\..\Assets\Graphics\Animation\Explode\";
        private readonly string[] _assetHitExplosionAnimationFiles = {
            #region Image Paths.
            "Hit Explosion 22 (1).png",
            "Hit Explosion 22 (2).png"
            #endregion
        };

        public BaseBullet(Core core, WeaponBase weapon, BaseGraphicObject firedFrom, string imagePath,
             BaseGraphicObject lockedTarget = null, PointD xyOffset = null)
            : base(core)
        {
            Initialize(imagePath);

            Weapon = weapon;
            LockedTarget = lockedTarget;
            Velocity.ThrottlePercentage = 100;

            VelocityD initialVelocity = new VelocityD()
            {
                Angle = new AngleD(firedFrom.Velocity.Angle.Degrees),
                MaxSpeed = weapon.Speed,
                ThrottlePercentage = 100
            };

            if (weapon != null && weapon.ExplodesOnImpact)
            {
                int _hitExplosionImageIndex = Utility.RandomNumber(0, _assetHitExplosionAnimationFiles.Count());
                _hitExplosionAnimation = new ObjAnimation(_core, _assetHitExplosionAnimationPath + _assetHitExplosionAnimationFiles[_hitExplosionImageIndex], new Size(22, 22));
            }

            var initialLocation = firedFrom.Location;
            initialLocation.X = initialLocation.X + (xyOffset == null ? 0 : xyOffset.X);
            initialLocation.Y = initialLocation.Y + (xyOffset == null ? 0 : xyOffset.Y);
            Location = initialLocation;

            if (firedFrom is BaseEnemy)
            {
                FiredFromType = FiredFromType.Enemy;
            }
            else if (firedFrom is ObjPlayer)
            {
                FiredFromType = FiredFromType.Player;
            }

            Velocity = initialVelocity;
        }

        public virtual void ApplyIntelligence()
        {
        }

        public virtual new void Explode()
        {
            if (Weapon.ExplodesOnImpact)
            {
                _hitExplosionAnimation.Reset();
                _core.Actors.PlaceAnimationOnTopOf(_hitExplosionAnimation, this);
            }
            ReadyForDeletion = true;
        }
    }
}
