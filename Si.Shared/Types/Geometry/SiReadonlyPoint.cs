namespace Si.Shared.Types.Geometry
{
    public class SiReadonlyPoint : SiPoint
    {
        public SiReadonlyPoint()
        {
        }

        public SiReadonlyPoint(double x, double y)
            : base(x, y, true)
        {
        }

        public SiReadonlyPoint(SiReadonlyPoint p)
            : base(p)
        {
        }

        public SiReadonlyPoint(SiPoint p)
            : base(p)
        {
        }
    }
}
