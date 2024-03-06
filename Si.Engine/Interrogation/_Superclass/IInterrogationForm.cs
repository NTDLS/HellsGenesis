using Si.Library.Sprite;

namespace Si.Engine.Interrogation._Superclass
{
    public interface IInterrogationForm
    {
        public void StartWatch(EngineCore engine, ISprite sprite);
        public void WriteLine(string text, System.Drawing.Color color);
        public void Write(string text, System.Drawing.Color color);
        public void ClearText();
        public void Show();
        public void Hide();
    }
}
