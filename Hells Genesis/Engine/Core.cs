using HG.Engine.Controllers;
using HG.Loudouts;
using HG.Menus;
using HG.TickHandlers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Drawing;
using System.Windows.Forms;

namespace HG.Engine
{
    internal class Core
    {
        public PrefabPlayerLoadouts PrefabPlayerLoadouts { get; private set; }

        public EngineSettings Settings { get; private set; } = new();
        public EngineInputController Input { get; private set; }
        public EngineDisplayController Display { get; private set; }
        public EngineActorController Actors { get; private set; }
        public SituationTickHandler Situations { get; private set; }
        public EventTickHandler Events { get; private set; }
        public EngineAudioController Audio { get; private set; }
        public EngineImageController Imaging { get; private set; }
        public EngineAssetController Assets { get; private set; }

        public MenuTickHandler Menus { get; private set; }
        public PlayerTickHandler Player { get; private set; }
        public EngineDrawingCacheController DrawingCache { get; private set; }

        public bool IsRunning { get; private set; } = false;
        public bool IsRendering { get; set; } = false;
        public bool ShowDebug { get; set; } = false;
        public object DrawingSemaphore { get; private set; } = new();

        private readonly GameLoop _gameLoop;

        static uint _nextSequentialId = 1;
        static object _nextSequentialLock = new object();
        public static uint GetNextSequentialId()
        {
            lock (_nextSequentialLock)
            {
                return _nextSequentialId++;
            }
        }

        #region Events.

        public delegate void StartEvent(Core sender);
        public event StartEvent OnStart;

        public delegate void StopEvent(Core sender);
        public event StopEvent OnStop;

        #endregion

        public Core(Control drawingSurface)
        {
            Display = new EngineDisplayController(this, drawingSurface, new Size(drawingSurface.Width, drawingSurface.Height));
            Assets = new EngineAssetController(this);
            Actors = new EngineActorController(this);
            Input = new EngineInputController(this);
            Situations = new SituationTickHandler(this);
            Events = new EventTickHandler(this);
            Audio = new EngineAudioController(this);
            Imaging = new EngineImageController(this);
            Menus = new MenuTickHandler(this);
            Player = new PlayerTickHandler(this);
            DrawingCache = new EngineDrawingCacheController(this);

            LoadPrefabs();

            _gameLoop = new GameLoop(this);

            Events.Create(new System.TimeSpan(0, 0, 0, 1), NewGameMenuCallback);
        }

        private void LoadPrefabs()
        {
            var playerLoadoutPath = "Loadouts\\Player.json";
            var playerLoadoutText = Assets.GetText(playerLoadoutPath);
            if (string.IsNullOrEmpty(playerLoadoutText) == false)
            {
                PrefabPlayerLoadouts = JsonConvert.DeserializeObject<PrefabPlayerLoadouts>(playerLoadoutText);
            }
            else
            {
                PrefabPlayerLoadouts = new PrefabPlayerLoadouts();

                PrefabPlayerLoadouts.CreateDefaults(); //We couldnt find a file, create a default loadout/

                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    Converters = new JsonConverter[] { new StringEnumConverter() }
                };
                var defaultLoadout = JsonConvert.SerializeObject(PrefabPlayerLoadouts, Formatting.Indented, settings);

                //Create the missing loadout file.
                Assets.PutText(playerLoadoutPath, defaultLoadout);
            }
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

                    var coinAnimation = new ObjAnimation(_core, @"Graphics\Animation\Coin.png", new Size(32, 23), 20, mode);
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