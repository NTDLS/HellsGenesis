namespace HG.Engine.Types.Geometry
{
    internal class HgSize
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public HgSize()
        {

        }

        public HgSize(double width, double height)
        {
            Width = width;
            Height = height;
        }
    }
}
