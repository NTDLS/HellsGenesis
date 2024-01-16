namespace Si.Library.Types.Geometry
{
    public class SiReadonlyPoint : SiPoint
    {
        public SiReadonlyPoint()
        {
        }

        public SiReadonlyPoint(double x, double y)
        {
            _x = x;
            _y = y;
        }

        public SiReadonlyPoint(SiReadonlyPoint p)
        {
            _x = p._x;
            _y = p._y;
        }

        public SiReadonlyPoint(SiPoint p)
        {
            _x = p.X;
            _y = p.Y;
        }

        public override double X
        {
            get => _x;
            set
            {
                throw new Exception("The point is readonly");
            }
        }

        public override double Y
        {
            get => _y;
            set
            {
                throw new Exception("The point is readonly");
            }
        }

        public SiPoint ToWriteableCopy() => new SiPoint(this);

        public new SiReadonlyPoint Clone() => new SiReadonlyPoint(this);
    }
}
