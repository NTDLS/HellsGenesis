using AI2D.Objects;
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

        public static double CalculeAngle(PointD from, PointD to)
        {
            var radian = Math.Atan2((to.Y - from.Y), (to.X - from.X));
            var angle = ((radian * (180 / Math.PI) + 360) + 90) % 360;

            return angle;
        }
        public static double CalculeAngle(BaseObject from, BaseObject to)
        {
            return CalculeAngle(from.Location, to.Location);
        }

        public static double CalculeDistance(PointD from, PointD to)
        {
            var deltaX = Math.Pow((to.X - from.X), 2);
            var deltaY = Math.Pow((to.Y - from.Y), 2);

            var distance = Math.Sqrt(deltaY + deltaX);

            return distance;
        }

        public static double CalculeDistance(BaseObject from, BaseObject to)
        {
            return CalculeDistance(from.Location, to.Location);
        }

    }
}
