using AI2D.Engine.Managers;
using AI2D.Engine.Menus;
using System.Drawing;
using System.Windows.Forms;

namespace AI2D.Engine
{
    internal class Core
    {
        public EngineSettings Settings { get; set; } = new();
        public EngineInputManager Input { get; private set; }
        public EngineDisplayManager Display { get; private set; }
        public EngineActorManager Actors { get; private set; }
        public SituationManager Situations { get; set; }
        public EngineEventManager Events { get; set; }
        public AudioManager Audio { get; set; }
        public ImageManager Imaging { get; set; }

        public EngineDrawingCacheManager DrawingCache { get; set; } = new();
        public bool IsRunning { get; private set; } = false;
        public bool IsRendering { get; set; } = false;
        public bool ShowDebug { get; set; } = false;
        public object DrawingSemaphore { get; set; } = new();

        private readonly EngineThread _engineThread;

        #region Events.

        public delegate void StartEvent(Core sender);
        public event StartEvent OnStart;

        public delegate void StopEvent(Core sender);
        public event StopEvent OnStop;

        #endregion

        public Core(Control drawingSurface)
        {
            Display = new EngineDisplayManager(this, drawingSurface, new Size(drawingSurface.Width, drawingSurface.Height));
            Actors = new EngineActorManager(this);
            Input = new EngineInputManager(this);
            Situations = new SituationManager(this);
            Events = new EngineEventManager(this);
            Audio = new AudioManager(this);
            Imaging = new ImageManager(this);

            _engineThread = new EngineThread(this);

            Events.Create(new System.TimeSpan(0, 0, 0, 1), NewGameMenuCallback);
        }

        private void NewGameMenuCallback(Core core, EngineCallbackEvent sender, object refObj)
        {
            Actors.Menus.Insert(new MenuStartNewGame(this));
        }

        public void Start()
        {
            if (IsRunning == false)
            {
                IsRunning = true;
                Actors.Start();
                //Actors.ResetPlayer();

                _engineThread.Start();

                //This is debug stuff. Will be moved to a logic engine.
                //_core.Actors.AddNewEngineCallbackEvent(new System.TimeSpan(0, 0, 0, 0, 0),
                //    AddDebugObjectsCallback, null, EngineCallbackEvent.CallbackEventMode.OneTime);

                /*
                var debug = _core.Actors.AddNewDebug();
                debug.X = _core.Display.VisibleSize.Width / 2;
                debug.Y = _core.Display.VisibleSize.Height / 2;

                if (_core.Actors.Animations.Count < 1)
                {
                    PlayMode mode = new PlayMode()
                    {
                        Replay = ReplayMode.LoopedPlay,
                        ReplayDelay = new System.TimeSpan(0, 0, 0, 1),
                        DeleteActorAfterPlay = false
                    };

                    var coinAnimation = new ObjAnimation(_core, @"..\..\..\Assets\Graphics\Animation\Coin.png", new Size(32, 23), 20, mode);
                }
                */

                OnStart?.Invoke(this);
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                _engineThread.Stop();
                Actors.Stop();
                OnStop?.Invoke(this);
            }
        }

        public bool IsPaused()
        {
            return _engineThread.IsPaused();
        }

        public void TogglePause()
        {
            _engineThread.TogglePause();
        }

        public void Pause()
        {
            _engineThread.Pause();
        }

        public void Resume()
        {
            _engineThread.Resume();
        }
    }
}