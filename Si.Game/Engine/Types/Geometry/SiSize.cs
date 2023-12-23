namespace Si.Game.Engine.Types.Geometry
{
    internal class SiSize
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public SiSize()
        {

        }

        public SiSize(double width, double height)
        {
            Width = width;
            Height = height;
        }
    }
}
