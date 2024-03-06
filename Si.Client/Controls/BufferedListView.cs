namespace Si.Client.Controls
{
    class BufferedListView : System.Windows.Forms.ListView
    {
        public BufferedListView()
        {
            DoubleBuffered = true;
        }
    }
}
