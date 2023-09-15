using HG.Controllers;
using HG.Engine.ImageProcessing;
using HG.Engine.Types;
using HG.Loudouts;
using HG.Menus;
using HG.TickHandlers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace HG.Engine
{
    internal class EngineCore
    {
        public PrefabPlayerLoadouts PrefabPlayerLoadouts { get; private set; }
        public DirectX DirectX { get; private set; }
        public EngineInputController Input { get; private set; }
        public EngineDisplayController Display { get; private set; }
        public EngineActorController Actors { get; private set; }
        public SituationTickHandler Situations { get; private set; }
        public EventTickHandler Events { get; private set; }
        public EngineAudioController Audio { get; private set; }
        public EngineImageController Imaging { get; private set; }
        public EngineAssetController Assets { get; private set; }
        public EngineSettings Settings { get; private set; }

        public MenuTickHandler Menus { get; private set; }
        public PlayerTickHandler Player { get; private set; }
        public EngineDrawingCacheController DrawingCache { get; private set; }

        public bool IsRunning { get; private set; } = false;
        public bool IsRendering { get; set; } = false;
        public bool ShowDebug { get; set; } = false;

        private readonly EngineWorldClock _gameLoop;

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

        public delegate void StartEvent(EngineCore sender);
        public event StartEvent OnStart;

        public delegate void StopEvent(EngineCore sender);
        public event StopEvent OnStop;

        #endregion

        public EngineCore(Control drawingSurface)
        {
            Settings = LoadSettings();

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
            DirectX = new DirectX(this);

            LoadPrefabs();

            _gameLoop = new EngineWorldClock(this);

            Events.Create(new System.TimeSpan(0, 0, 0, 1), NewGameMenuCallback);
        }

        public static EngineSettings LoadSettings()
        {
            string executablePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string executableDirectory = Path.GetDirectoryName(executablePath);
            string settingsFile = Path.Combine(executableDirectory, "enginesettings.json");

            if (File.Exists(settingsFile) == false)
            {
                File.WriteAllText(settingsFile, JsonConvert.SerializeObject(new EngineSettings(), Formatting.Indented));
            }

            return JsonConvert.DeserializeObject<EngineSettings>(File.ReadAllText(settingsFile));
        }

        public static void SaveSettings(EngineSettings settings)
        {
            string executablePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string executableDirectory = Path.GetDirectoryName(executablePath);
            string settingsFile = Path.Combine(executableDirectory, "enginesettings.json");
            File.WriteAllText(settingsFile, JsonConvert.SerializeObject(settings, Formatting.Indented));
        }

        public void Render()
        {
            try
            {
                DirectX.ScreenRenderTarget.BeginDraw();
                DirectX.IntermediateRenderTarget.BeginDraw();

                DirectX.ScreenRenderTarget.Clear(DirectX.Materials.Raw.Black);

                DirectX.IntermediateRenderTarget.Clear(DirectX.Materials.Raw.Black);
                Actors.RenderPreScaling(DirectX.IntermediateRenderTarget);
                DirectX.IntermediateRenderTarget.EndDraw();

                if (Settings.AutoZoomWhenMoving)
                {
                    DirectX.ApplyScaling((float)Display.SpeedOrientedFrameScalingFactor());
                }
                else
                {
                    DirectX.ApplyScaling((float)Display.BaseDrawScale);
                }
                Actors.RenderPostScaling(DirectX.ScreenRenderTarget);

                DirectX.ScreenRenderTarget.EndDraw();
            }
            catch
            {
            }
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

        private void NewGameMenuCallback(EngineCore core, HgEngineCallbackEvent sender, object refObj)
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
                DirectX.Cleanup();
            }
        }

        public bool IsPaused() => _gameLoop.IsPaused();
        public void TogglePause() => _gameLoop.TogglePause();
        public void Pause() => _gameLoop.Pause();
        public void Resume() => _gameLoop.Resume();
    }
}