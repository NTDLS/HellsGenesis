using AI2D.Types;
using System;

namespace AI2D.Engine
{
    public class Utility
    {
        public static Random Random = new Random();
        public static bool FlipCoin()
        {
            return Random.Next(0, 1000) >= 500;
        }

        public static PointD AngleToXY(double angle)
        {
            double radians = (Math.PI / 180) * (angle - 90);

            PointD result = new PointD()
            {
                X = Math.Cos(radians),
                Y = Math.Sin(-radians)
            };

            return result;
        }

        public static double CalculeAngle(PointD perspective, PointD pointOfDirection)
        {
            var radian = Math.Atan2((pointOfDirection.Y - perspective.Y), (pointOfDirection.X - perspective.X));
            var angle = ((radian * (180 / Math.PI) + 360) + 90) % 360;

            return angle;
        }

        public static double CalculeDistance(PointD perspective, PointD pointOfDirection)
        {
            var deltaX = Math.Pow((pointOfDirection.X - perspective.X), 2);
            var deltaY = Math.Pow((pointOfDirection.Y - perspective.Y), 2);

            var distance = Math.Sqrt(deltaY + deltaX);

            return distance;
        }
    }
}
