using System.Drawing;
using System.Windows.Forms;

namespace AI2D.Engine
{
    public class Game
    {
        public GameInput Input { get; private set; }
        public GameDisplay Display { get; private set; }
        public GameActors Actors { get; private set; }
        private GameThread _gameThread;


        public Game(Control drawingSurface, Size visibleSize)
        {
            Display = new GameDisplay(drawingSurface, visibleSize);
            Actors = new GameActors(this);
            Input = new GameInput(this);

            _gameThread = new GameThread(this);
        }

        public void Start()
        {
            Actors.BackgroundMusicSound.Play();

            for (int i = 0; i < 1; i++)
            {
                Actors.CreateEnemy();
            }

            Actors.ShowNewPlayer();
            _gameThread.Start();
        }

        public void Stop()
        {
            _gameThread.Stop();
        }

        public void RenderObjects(Graphics dc)
        {
            Actors.RenderObjects(dc);
        }
    }
}