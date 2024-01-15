using Si.GameEngine.Sprites._Superclass;

namespace Si.GameEngine.Core.Debug._Superclass
{
    public interface IDebugForm
    {
        public void StartWatch(GameEngineCore gameEngine, SpriteBase sprite);
        public void WriteLine(string text, System.Drawing.Color color);
        public void Write(string text, System.Drawing.Color color);
        public void ClearText();
        public void Show();
        public void Hide();
    }
}
