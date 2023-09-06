using System;

namespace HG.Types
{
    internal class HgPoint<T>
    {
        private static readonly Lazy<HgPoint<T>> _zero = new Lazy<HgPoint<T>>(() => new HgPoint<T>());
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

        public HgPoint<T> ToWriteableCopy => new HgPoint<T>(this);
    

        public static T DistanceTo(HgPoint<T> from, HgPoint<T> to)
        {
            var deltaX = Math.Pow((to.X - (dynamic)from.X), 2);
            var deltaY = Math.Pow((to.Y - (dynamic)from.Y), 2);
            return Math.Sqrt(deltaY + deltaX);
        }

        public static T AngleTo(HgPoint<T> from, HgPoint<T> to)
        {
            var fRadians = Math.Atan2((to.Y - (dynamic)from.Y), (to.X - (dynamic)from.X));
            var fDegrees = ((HgAngle<double>.RadiansToDegrees(fRadians) + 360.0) + HgAngle<double>.DegreeOffset) % 360.0;
            return fDegrees;
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
