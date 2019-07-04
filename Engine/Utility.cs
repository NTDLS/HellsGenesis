using AI2D.Objects;
using AI2D.Types;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AI2D.Engine
{
    public class Utility
    {
        const double RADIAN_CONV = 0.01745329251994329576923690768; // (PI / 180.0f)
        const double OFFSET90DEGREES = 1.57079632679489661923132169156; // (180.0f * RADIAN_CONV)						 
        const double FULLCIRCLE = 6.28318530717958647692528676624; // (360.0f * RADIAN_CONV)

        public static double RequiredAngleTo(BaseObject from, BaseObject to)
        {
            return RequiredAngleTo(from.Location, to.Location);
        }

        public static double RequiredAngleTo(PointD from, PointD to)
        {
            var fRadians = Math.Atan2((to.Y - from.Y), (to.X - from.X));
            var fDegrees = ((fRadians * (180 / Math.PI) + 360) + 90) % 360;
            return fDegrees;
        }

        public static bool IsPointingAt(BaseObject fromObj, BaseObject atObj, double toleranceDegrees)
        {
            var deltaAngle = Math.Abs(GetDeltaAngle(fromObj, atObj));
            return deltaAngle < toleranceDegrees;
        }

        public static double GetDeltaAngle(BaseObject fromObj, BaseObject atObj)
        {
            double angleTo = RequiredAngleTo(fromObj, atObj);

            if (fromObj.Velocity.Angle.Degree < 0) fromObj.Velocity.Angle.Degree = (0 - fromObj.Velocity.Angle.Degree);
            if (angleTo < 0) angleTo = (0 - angleTo);

            return fromObj.Velocity.Angle.Degree - angleTo;
        }

        public static Random Random = new Random();
        public static bool FlipCoin()
        {
            return Random.Next(0, 1000) >= 500;
        }

        public static Double RandomNumber(double min, double max)
        {
            return Random.Next(0, 1000) % max;
        }

        public static int RandomNumber(int min, int max)
        {
            return Random.Next(0, 1000) % max;
        }

        public static Image ResizeImage(Image image, int new_height, int new_width)
        {
            Bitmap new_image = new Bitmap(new_width, new_height);
            Graphics g = Graphics.FromImage((Image)new_image);
            g.InterpolationMode = InterpolationMode.High;
            g.DrawImage(image, 0, 0, new_width, new_height);
            return new_image;
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
