using System.Drawing;
using System.Windows.Forms;

namespace AI2D.Engine
{
    public class Core
    {
        public EngineInput Input { get; private set; }
        public EngineDisplay Display { get; private set; }
        public EngineActors Actors { get; private set; }
        private EngineThread _engineThread;

        public Core(Control drawingSurface, Size visibleSize)
        {
            Display = new EngineDisplay(drawingSurface, visibleSize);
            Actors = new EngineActors(this);
            Input = new EngineInput(this);

            _engineThread = new EngineThread(this);
        }

        public void Start()
        {
            Actors.BackgroundMusicSound.Play();

            for (int i = 0; i < 25; i++)
            {
                Actors.CreateEnemy();
            }

            Actors.ShowNewPlayer();
            _engineThread.Start();
        }

        public void Stop()
        {
            _engineThread.Stop();
        }
    }
}