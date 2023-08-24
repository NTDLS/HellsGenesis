using HG.Engine.Managers;
using HG.Engine.Menus;
using HG.TickManagers;
using System.Drawing;
using System.Windows.Forms;

namespace HG.Engine
{
    internal class Core
    {
        public EngineSettings Settings { get; set; } = new();
        public EngineInputManager Input { get; private set; }
        public EngineDisplayManager Display { get; private set; }
        public EngineActorManager Actors { get; private set; }
        public SituationManager Situations { get; set; }
        public EventManager Events { get; set; }
        public EngineAudioManager Audio { get; set; }
        public EngineImageManager Imaging { get; set; }
        public MenuManager Menus { get; set; }
        public PlayerManager Player { get; set; }
        public EngineDrawingCacheManager DrawingCache { get; set; } = new();

        public bool IsRunning { get; private set; } = false;
        public bool IsRendering { get; set; } = false;
        public bool ShowDebug { get; set; } = false;
        public object DrawingSemaphore { get; set; } = new();

        private readonly GameLoop _gameLoop;

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
            Events = new EventManager(this);
            Audio = new EngineAudioManager(this);
            Imaging = new EngineImageManager(this);
            Menus = new MenuManager(this);
            Player = new PlayerManager(this);

            _gameLoop = new GameLoop(this);

            Events.Create(new System.TimeSpan(0, 0, 0, 1), NewGameMenuCallback);
        }

        private void NewGameMenuCallback(Core core, HgEngineCallbackEvent sender, object refObj)
        {
            Menus.Insert(new MenuStartNewGame(this));
        }

        public void Start()
        {
            if (IsRunning == false)
            {
                IsRunning = true;
                Actors.Start();
                //Actors.ResetPlayer();

                _gameLoop.Start();

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
                _gameLoop.Stop();
                Actors.Stop();
                OnStop?.Invoke(this);
            }
        }

        public bool IsPaused()
        {
            return _gameLoop.IsPaused();
        }

        public void TogglePause()
        {
            _gameLoop.TogglePause();
        }

        public void Pause()
        {
            _gameLoop.Pause();
        }

        public void Resume()
        {
            _gameLoop.Resume();
        }
    }
}