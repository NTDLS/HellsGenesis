using System;
using System.Drawing;

namespace HG.Types.Geometry
{
    internal class HgPoint
    {
        public static HgPoint Zero = new();

        private double _x;
        private double _y;

        public double X
        {
            get => _x;
            set
            {
                if (IsReadonly) throw new Exception("The point is readonly");
                _x = value;
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                if (IsReadonly) throw new Exception("The point is readonly");
                _y = value;
            }
        }

        public bool IsReadonly { get; private set; }

        public HgPoint()
        {
        }

        public HgPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public HgPoint(HgPoint p)
        {
            X = p.X;
            Y = p.Y;
        }

        public HgPoint(bool isReadonly)
        {
            IsReadonly = isReadonly;
        }

        public HgPoint(double x, double y, bool isReadonly)
        {
            IsReadonly = isReadonly;
            _x = x;
            _y = y;
        }

        public HgPoint(HgPoint p, bool isReadonly)
        {
            IsReadonly = isReadonly;
            _x = p.X;
            _y = p.Y;
        }

        public HgPoint ToWriteableCopy() => new HgPoint(this);

        public RectangleF ToRectangleF(float width, float height)
        {
            return new RectangleF((float)_x, (float)_y, width, height);
        }

        public RectangleF ToRectangleF(SizeF size)
        {
            return new RectangleF((float)_x, (float)_y, size.Width, size.Height);
        }

        public RectangleF ToRectangleF()
        {
            return new RectangleF((float)_x, (float)_y, 1f, 1f);
        }

        /// <summary>
        /// Calculates the distance from one object to another.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static double DistanceTo(HgPoint from, HgPoint to)
        {
            var deltaX = Math.Pow(to.X - from.X, 2);
            var deltaY = Math.Pow(to.Y - from.Y, 2);
            return Math.Sqrt(deltaY + deltaX);
        }

        /// <summary>
        /// Calculates the angle from one obect to another, returns the degrees.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static double AngleTo(HgPoint from, HgPoint to)
        {
            var radians = Math.Atan2(to.Y - from.Y, to.X - from.X);
            return (HgAngle.RadiansToDegrees(radians) + 360.0 + HgAngle.AngleOffsetDegrees) % 360.0;
        }

        #region  Unary Operator Overloading.

        public static HgPoint operator -(HgPoint original, HgPoint modifier)
        {
            return new HgPoint(original.X - modifier.X, original.Y - modifier.Y);
        }

        public static HgPoint operator -(HgPoint original, double modifier)
        {
            return new HgPoint(original.X - modifier, original.Y - modifier);
        }

        public static HgPoint operator +(HgPoint original, HgPoint modifier)
        {
            return new HgPoint(original.X + modifier.X, original.Y + modifier.Y);
        }

        public static HgPoint operator +(HgPoint original, double modifier)
        {
            return new HgPoint(original.X + modifier, original.Y + modifier);
        }

        public static HgPoint operator *(HgPoint original, HgPoint modifier)
        {
            return new HgPoint(original.X * modifier.X, original.Y * modifier.Y);
        }

        public static HgPoint operator *(HgPoint original, double modifier)
        {
            return new HgPoint(original.X * modifier, original.Y * modifier);
        }

        public override bool Equals(object o)
        {
            return Math.Round(((HgPoint)o).X, 4) == X && Math.Round(((HgPoint)o).Y, 4) == Y;
        }

        #endregion

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{{{Math.Round(X, 4).ToString("#.####")},{Math.Round(Y, 4).ToString("#.####")}}}";
        }
    }
}
