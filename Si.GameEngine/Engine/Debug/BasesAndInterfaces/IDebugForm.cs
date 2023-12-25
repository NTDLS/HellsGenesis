namespace Si.GameEngine.Engine.Debug.BasesAndInterfaces
{
    public interface IDebugForm
    {
        public void WriteLine(string text, System.Drawing.Color color);
        public void Write(string text, System.Drawing.Color color);
        public void ClearText();
        public void Show();
        public void Hide();
    }
}
