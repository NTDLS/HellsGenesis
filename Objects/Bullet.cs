using AI2D.Engine;
using AI2D.Types;

namespace AI2D.Objects
{
    public class Bullet: BaseObject
    {
        public FiredFromType FiredFromType { get; set; }

        private const string _imagePath = @"..\..\Assets\Graphics\BasicBullet.png";

        public Bullet(Game game, BaseObject firedFrom)
            : base(game)
        {
            Vector initialVector = new Vector()
            {
                Angle = new Angle(firedFrom.Velocity.Angle.Degree),
                Speed = 25
            };

            var initialLocation = firedFrom.Location;

            initialLocation.X = (initialLocation.X + (firedFrom.Size.Width / 2.0));
            initialLocation.Y = (initialLocation.Y + (firedFrom.Size.Height / 2.0));

            if (firedFrom is Enemy)
            {
                FiredFromType = FiredFromType.Enemy;
            }
            else if (firedFrom is Player)
            {
                FiredFromType = FiredFromType.Player;
            }

            LoadResources(_imagePath, null, initialLocation, initialVector);
        }
    }
}
