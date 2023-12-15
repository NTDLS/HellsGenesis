using System;
using System.Drawing;

namespace NebulaSiege.Engine.Types.Geometry
{
    internal class NsPoint
    {
        public static NsPoint Zero = new();

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

        public NsPoint()
        {
        }

        public NsPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public NsPoint(NsPoint p)
        {
            X = p.X;
            Y = p.Y;
        }

        public NsPoint(bool isReadonly)
        {
            IsReadonly = isReadonly;
        }

        public NsPoint(double x, double y, bool isReadonly)
        {
            IsReadonly = isReadonly;
            _x = x;
            _y = y;
        }

        public NsPoint(NsPoint p, bool isReadonly)
        {
            IsReadonly = isReadonly;
            _x = p.X;
            _y = p.Y;
        }

        public NsPoint ToWriteableCopy() => new NsPoint(this);

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
        public static double DistanceTo(NsPoint from, NsPoint to)
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
        public static double AngleTo360(NsPoint from, NsPoint to)
        {
            var radians = Math.Atan2(to.Y - from.Y, to.X - from.X);
            return (NsAngle.RadiansToDegrees(radians) + 360.0 + NsAngle.AngleOffsetDegrees) % 360.0;
        }

        #region  Unary Operator Overloading.

        public static NsPoint operator -(NsPoint original, NsPoint modifier)
        {
            return new NsPoint(original.X - modifier.X, original.Y - modifier.Y);
        }

        public static NsPoint operator -(NsPoint original, double modifier)
        {
            return new NsPoint(original.X - modifier, original.Y - modifier);
        }

        public static NsPoint operator +(NsPoint original, NsPoint modifier)
        {
            return new NsPoint(original.X + modifier.X, original.Y + modifier.Y);
        }

        public static NsPoint operator +(NsPoint original, double modifier)
        {
            return new NsPoint(original.X + modifier, original.Y + modifier);
        }

        public static NsPoint operator *(NsPoint original, NsPoint modifier)
        {
            return new NsPoint(original.X * modifier.X, original.Y * modifier.Y);
        }

        public static NsPoint operator *(NsPoint original, double modifier)
        {
            return new NsPoint(original.X * modifier, original.Y * modifier);
        }

        public override bool Equals(object o)
        {
            return Math.Round(((NsPoint)o).X, 4) == X && Math.Round(((NsPoint)o).Y, 4) == Y;
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
