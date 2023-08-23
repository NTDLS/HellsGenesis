namespace HG.Types
{
    internal class HGSize<T>
    {
        public T Width { get; set; }
        public T Height { get; set; }

        public HGSize()
        {

        }

        public HGSize(T width, T height)
        {
            Width = width;
            Height = height;
        }
    }
}
