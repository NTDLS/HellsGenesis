using AI2D.Engine;
using AI2D.GraphicObjects.Enemies;
using AI2D.Types;
using AI2D.Weapons;
using System.Drawing;

namespace AI2D.GraphicObjects
{
    public class ObjBullet : BaseGraphicObject
    {
        public FiredFromType FiredFromType { get; set; }

        public WeaponBase Weapon { get; private set; }

        public BaseGraphicObject LockedTarget { get; private set; }

        public ObjBullet(Core core, WeaponBase weapon, BaseGraphicObject firedFrom, string imagePath,
             BaseGraphicObject lockedTarget = null, PointD xyOffset = null)
            : base(core)
        {
            Weapon = weapon;
            LockedTarget = lockedTarget;
            IsLockedOn = LockedTarget != null;

            VelocityD initialVector = new VelocityD()
            {
                Angle = new AngleD(firedFrom.Velocity.Angle.Degrees),
                MaxSpeed = weapon.Speed,
                ThrottlePercentage = 100
            };

            var initialLocation = firedFrom.Location;

            initialLocation.X = initialLocation.X + (xyOffset == null ? 0 : xyOffset.X);
            initialLocation.Y = initialLocation.Y + (xyOffset == null ? 0 : xyOffset.Y);

            if (firedFrom is BaseEnemy)
            {
                FiredFromType = FiredFromType.Enemy;
            }
            else if (firedFrom is ObjPlayer)
            {
                FiredFromType = FiredFromType.Player;
            }

            LoadResources(imagePath, null, null, null, initialLocation, initialVector);
        }
    }
}
