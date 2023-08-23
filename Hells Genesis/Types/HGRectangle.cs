namespace HG.Types
{
    internal class HGRectangle<T>
    {
        public T X { get; set; }
        public T Y { get; set; }
        public T Width { get; set; }
        public T Height { get; set; }

        public HGRectangle()
        {
        }

        public HGRectangle(T x, T y, T width, T height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
