using AI2D.Engine;
using AI2D.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI2D.Objects
{
    public class Bullet: BaseObject
    {
        #region ~/Ctor

        private const string _imagePath = @"..\..\Assets\Graphics\BasicBullet.png";

        public Bullet(Game game, BaseObject firedFrom)
        {

            Vector initialVector = new Vector()
            {
                Angle = new Angle(firedFrom.Velocity.Angle.Degree),
                Speed = 25
            };

            var initialLocation = firedFrom.Location;

            initialLocation.X = (initialLocation.X + (firedFrom.Size.Width / 2.0));
            initialLocation.Y = (initialLocation.Y + (firedFrom.Size.Height / 2.0));

            Initialize(game, _imagePath, null, initialLocation, initialVector);
        }

        #endregion
    }
}
