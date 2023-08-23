using System;

namespace HG.Types
{
    internal class HGPoint<T>
    {
        public T X { get; set; }
        public T Y { get; set; }

        public HGPoint()
        {
        }
        public HGPoint(T x, T y)
        {
            X = x;
            Y = y;
        }

        public HGPoint(HGPoint<T> p)
        {
            X = p.X;
            Y = p.Y;
        }

        public static T DistanceTo(HGPoint<T> from, HGPoint<T> to)
        {
            var deltaX = Math.Pow((to.X - (dynamic)from.X), 2);
            var deltaY = Math.Pow((to.Y - (dynamic)from.Y), 2);
            return Math.Sqrt(deltaY + deltaX);
        }

        public static T AngleTo(HGPoint<T> from, HGPoint<T> to)
        {
            var fRadians = Math.Atan2((to.Y - (dynamic)from.Y), (to.X - (dynamic)from.X));
            var fDegrees = ((HGAngle<double>.RadiansToDegrees(fRadians) + 360.0) + HGAngle<double>.DegreeOffset) % 360.0;
            return fDegrees;
        }

        #region  Unary Operator Overloading.

        public static HGPoint<T> operator -(HGPoint<T> original, HGPoint<T> modifier)
        {
            return new HGPoint<T>(original.X - (dynamic)modifier.X, original.Y - (dynamic)modifier.Y);
        }

        public static HGPoint<T> operator -(HGPoint<T> original, T modifier)
        {
            return new HGPoint<T>(original.X - (dynamic)modifier, original.Y - (dynamic)modifier);
        }

        public static HGPoint<T> operator +(HGPoint<T> original, HGPoint<T> modifier)
        {
            return new HGPoint<T>(original.X + (dynamic)modifier.X, original.Y + (dynamic)modifier.Y);
        }

        public static HGPoint<T> operator +(HGPoint<T> original, T modifier)
        {
            return new HGPoint<T>(original.X + (dynamic)modifier, original.Y + (dynamic)modifier);
        }

        public static HGPoint<T> operator *(HGPoint<T> original, HGPoint<T> modifier)
        {
            return new HGPoint<T>(original.X * (dynamic)modifier.X, original.Y * (dynamic)modifier.Y);
        }

        public static HGPoint<T> operator *(HGPoint<T> original, T modifier)
        {
            return new HGPoint<T>(original.X * (dynamic)modifier, original.Y * (dynamic)modifier);
        }

        public override bool Equals(object o)
        {
            return (Math.Round((dynamic)((HGPoint<T>)o).X, 4) == X && Math.Round((dynamic)((HGPoint<T>)o).Y, 4) == Y);
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
