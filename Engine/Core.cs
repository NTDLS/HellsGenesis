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

            Actors.ResetPlayer();
            _engineThread.Start();

            Actors.CreateEngineCallbackEvent(new System.TimeSpan(0, 0, 0, 0, 500), FirstShowPlayerCallback);

            Actors.CreateEngineCallbackEvent(new System.TimeSpan(0, 0, 0, 10),
                AddFreshEnemiesCallback, null, EngineCallbackEvent.CallbackEventMode.Recurring);
        }

        private void FirstShowPlayerCallback(Core core, object refObj)
        {
            Actors.ResetAndShowPlayer();
        }

        private void AddFreshEnemiesCallback(Core core, object refObj)
        {
            if (Actors.Enemies.Count < 15)
            {
                for (int i = 0; i < 2; i++)
                {
                    Actors.CreateEnemy();
                }
            }
        }

        public void Stop()
        {
            _engineThread.Stop();
        }
    }
}