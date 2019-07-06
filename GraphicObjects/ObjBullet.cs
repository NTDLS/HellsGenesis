using AI2D.Engine;
using AI2D.GraphicObjects.Enemies;
using AI2D.Types;

namespace AI2D.GraphicObjects
{
    public class ObjBullet: BaseGraphicObject
    {
        public FiredFromType FiredFromType { get; set; }

        public int Damage { get; set; }

        public ObjBullet(Core core, string imagePath, int damage, BaseGraphicObject firedFrom)
            : base(core)
        {
            Damage = damage;

            Vector initialVector = new Vector()
            {
                Angle = new Angle(firedFrom.Velocity.Angle.Degree),
                Speed = 25
            };

            var initialLocation = firedFrom.Location;

            initialLocation.X = initialLocation.X;
            initialLocation.Y = initialLocation.Y;

            //initialLocation.X = (initialLocation.X + (firedFrom.Size.Width / 2.0));
            //initialLocation.Y = (initialLocation.Y + (firedFrom.Size.Height / 2.0));

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

        public ObjBullet(Core core, string imagePath, int damage, Vector initialVector, PointD initialLocation)
            : base(core)
        {
            Damage = damage;

            LoadResources(imagePath, null, null, null, initialLocation, initialVector);
        }
    }
}
