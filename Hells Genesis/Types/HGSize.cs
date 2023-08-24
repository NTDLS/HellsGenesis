namespace HG.Types
{
    internal class HgSize<T>
    {
        public T Width { get; set; }
        public T Height { get; set; }

        public HgSize()
        {

        }

        public HgSize(T width, T height)
        {
            Width = width;
            Height = height;
        }
    }
}
