namespace HG.Types
{
    internal class HgRectangle<T>
    {
        public T X { get; set; }
        public T Y { get; set; }
        public T Width { get; set; }
        public T Height { get; set; }

        public HgRectangle()
        {
        }

        public HgRectangle(T x, T y, T width, T height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
