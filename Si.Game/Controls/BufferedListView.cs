namespace Si.Game.Controls
{
    class BufferedListView : System.Windows.Forms.ListView
    {
        public BufferedListView()
        {
            DoubleBuffered = true;
        }
    }
}
