using System.Drawing;

namespace Si.Library.Types.Geometry
{
    public class SiPoint
    {
        public static SiPoint Zero = new();

        protected double _x;
        protected double _y;

        public virtual double X
        {
            get => _x;
            set => _x = value;
        }

        public virtual double Y
        {
            get => _y;
            set => _y = value;
        }

        public SiPoint()
        {
        }

        public SiPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public SiPoint(SiPoint p)
        {
            X = p.X;
            Y = p.Y;
        }

        public RectangleF ToRectangleF(float width, float height)
            => new RectangleF((float)_x, (float)_y, width, height);

        public RectangleF ToRectangleF(SizeF size)
            => new RectangleF((float)_x, (float)_y, size.Width, size.Height);

        public RectangleF ToRectangleF()
            => new RectangleF((float)_x, (float)_y, 1f, 1f);

        /// <summary>
        /// Calculates the euclidean distance between two points in a 2D space (slower but more precisie than DistanceSquaredTo);
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static double DistanceTo(SiPoint from, SiPoint to)
        {
            var deltaX = Math.Pow(to.X - from.X, 2);
            var deltaY = Math.Pow(to.Y - from.Y, 2);
            return Math.Sqrt(deltaY + deltaX);
        }

        /// <summary>
        ///  Calculates the distance squared between two points in a 2D space (faster and less precisie than DistanceTo);.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static double DistanceSquaredTo(SiPoint from, SiPoint to)
        {
            var deltaX = to.X - from.X;
            var deltaY = to.Y - from.Y;
            return deltaX * deltaX + deltaY * deltaY;
        }

        /// <summary>
        /// Calculates the angle from one obect to another, returns the degrees.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static double AngleTo360(SiPoint from, SiPoint to)
        {
            var radians = Math.Atan2(to.Y - from.Y, to.X - from.X);
            return (SiAngle.RadiansToDegrees(radians) + 360.0) % 360.0;
        }

        #region  Unary Operator Overloading.

        public static SiPoint operator -(SiPoint original, SiPoint modifier)
            => new SiPoint(original.X - modifier.X, original.Y - modifier.Y);

        public static SiPoint operator -(SiPoint original, double modifier)
           => new SiPoint(original.X - modifier, original.Y - modifier);

        public static SiPoint operator +(SiPoint original, SiPoint modifier)
            => new SiPoint(original.X + modifier.X, original.Y + modifier.Y);

        public static SiPoint operator +(SiPoint original, double modifier)
            => new SiPoint(original.X + modifier, original.Y + modifier);

        public static SiPoint operator *(SiPoint original, SiPoint scaleFactor)
            => new SiPoint(original.X * scaleFactor.X, original.Y * scaleFactor.Y);

        public static SiPoint operator *(SiPoint original, double scaleFactor)
            => new SiPoint(original.X * scaleFactor, original.Y * scaleFactor);

        public static SiPoint operator /(SiPoint original, SiPoint scaleFactor)
        {
            if (scaleFactor.X == 0.0 && scaleFactor.Y == 0.0)
            {
                return new SiPoint(0, 0);
            }
            return new SiPoint(original.X / scaleFactor.X, original.Y / scaleFactor.Y);
        }

        public static SiPoint operator /(SiPoint original, double scaleFactor)
        {
            if (scaleFactor == 0.0)
            {
                return new SiPoint(0, 0);
            }
            return new SiPoint(original.X / scaleFactor, original.Y / scaleFactor);
        }

        public override bool Equals(object? o)
            => Math.Round(((SiPoint?)o)?.X ?? double.NaN, 4) == X && Math.Round(((SiPoint?)o)?.Y ?? double.NaN, 4) == Y;

        #endregion

        public override int GetHashCode() => ToString().GetHashCode();

        public override string ToString()
            => $"{{{Math.Round(X, 4).ToString("#.####")},{Math.Round(Y, 4).ToString("#.####")}}}";

        public SiReadonlyPoint ToReadonlyCopy() => new SiReadonlyPoint(this);

        public SiPoint Clone() => new SiPoint(this);
    }
}
