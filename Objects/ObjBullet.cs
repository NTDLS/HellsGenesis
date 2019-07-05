using AI2D.Engine;
using AI2D.Types;

namespace AI2D.Objects
{
    public class ObjBullet: ObjBase
    {
        public FiredFromType FiredFromType { get; set; }

        private const string _imagePath = @"..\..\Assets\Graphics\Bullet\Vulcan.png";

        public ObjBullet(Core core, ObjBase firedFrom)
            : base(core)
        {
            Vector initialVector = new Vector()
            {
                Angle = new Angle(firedFrom.Velocity.Angle.Degree),
                Speed = 25
            };

            var initialLocation = firedFrom.Location;

            initialLocation.X = (initialLocation.X + (firedFrom.Size.Width / 2.0));
            initialLocation.Y = (initialLocation.Y + (firedFrom.Size.Height / 2.0));

            if (firedFrom is ObjEnemy)
            {
                FiredFromType = FiredFromType.Enemy;
            }
            else if (firedFrom is ObjPlayer)
            {
                FiredFromType = FiredFromType.Player;
            }

            LoadResources(_imagePath, null, initialLocation, initialVector);
        }
    }
}
