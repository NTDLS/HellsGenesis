namespace NebulaSiege.Game.Engine.Types.Geometry
{
    internal class NsSize
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public NsSize()
        {

        }

        public NsSize(double width, double height)
        {
            Width = width;
            Height = height;
        }
    }
}
