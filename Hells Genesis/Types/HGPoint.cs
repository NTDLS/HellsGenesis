using System;
using System.Drawing;

namespace HG.Types
{
    internal class HgPoint<T>
    {
        private static readonly Lazy<HgPoint<T>> _zero = new(() => new HgPoint<T>());
        public static HgPoint<T> Zero => _zero.Value;

        private T _x;
        private T _y;

        public T X
        {
            get => _x;
            set
            {
                if (IsReadonly) throw new Exception("The point is readonly");
                _x = value;
            }
        }

        public T Y
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

        public HgPoint(T x, T y)
        {
            X = x;
            Y = y;
        }

        public HgPoint(HgPoint<T> p)
        {
            X = p.X;
            Y = p.Y;
        }

        public HgPoint(bool isReadonly)
        {
            IsReadonly = isReadonly;
        }

        public HgPoint(T x, T y, bool isReadonly)
        {
            IsReadonly = isReadonly;
            _x = x;
            _y = y;
        }

        public HgPoint(HgPoint<T> p, bool isReadonly)
        {
            IsReadonly = isReadonly;
            _x = p.X;
            _y = p.Y;
        }

        public HgPoint<T> ToWriteableCopy() => new HgPoint<T>(this);

        public RectangleF ToRectangleF(float width, float height)
        {
            return new RectangleF((float)(dynamic)_x, (float)(dynamic)_y, width, height);
        }

        public RectangleF ToRectangleF(float sizeX, Size size)
        {
            return new RectangleF((float)(dynamic)_x, (float)(dynamic)_y, size.Width, size.Height);
        }

        public RectangleF ToRectangleF()
        {
            return new RectangleF((float)(dynamic)_x, (float)(dynamic)_y, 1f, 1f);
        }

        /// <summary>
        /// Calculates the distance from one object to another.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static T DistanceTo(HgPoint<T> from, HgPoint<T> to)
        {
            var deltaX = Math.Pow((to.X - (dynamic)from.X), 2);
            var deltaY = Math.Pow((to.Y - (dynamic)from.Y), 2);
            return Math.Sqrt(deltaY + deltaX);
        }

        /// <summary>
        /// Calculates the angle from one obect to another, returns the degrees.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static T AngleTo(HgPoint<T> from, HgPoint<T> to)
        {
            var radians = Math.Atan2((to.Y - (dynamic)from.Y), (to.X - (dynamic)from.X));
            return ((HgAngle<T>.RadiansToDegrees(radians) + 360.0) + HgAngle<T>.AngleOffsetDegrees) % 360.0;
        }

        #region  Unary Operator Overloading.

        public static HgPoint<T> operator -(HgPoint<T> original, HgPoint<T> modifier)
        {
            return new HgPoint<T>(original.X - (dynamic)modifier.X, original.Y - (dynamic)modifier.Y);
        }

        public static HgPoint<T> operator -(HgPoint<T> original, T modifier)
        {
            return new HgPoint<T>(original.X - (dynamic)modifier, original.Y - (dynamic)modifier);
        }

        public static HgPoint<T> operator +(HgPoint<T> original, HgPoint<T> modifier)
        {
            return new HgPoint<T>(original.X + (dynamic)modifier.X, original.Y + (dynamic)modifier.Y);
        }

        public static HgPoint<T> operator +(HgPoint<T> original, T modifier)
        {
            return new HgPoint<T>(original.X + (dynamic)modifier, original.Y + (dynamic)modifier);
        }

        public static HgPoint<T> operator *(HgPoint<T> original, HgPoint<T> modifier)
        {
            return new HgPoint<T>(original.X * (dynamic)modifier.X, original.Y * (dynamic)modifier.Y);
        }

        public static HgPoint<T> operator *(HgPoint<T> original, T modifier)
        {
            return new HgPoint<T>(original.X * (dynamic)modifier, original.Y * (dynamic)modifier);
        }

        public override bool Equals(object o)
        {
            return (Math.Round((dynamic)((HgPoint<T>)o).X, 4) == X && Math.Round((dynamic)((HgPoint<T>)o).Y, 4) == Y);
        }

        #endregion

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{{{Math.Round((dynamic)X, 4).ToString("#.####")},{Math.Round((dynamic)Y, 4).ToString("#.####")}}}";
        }
    }
}
