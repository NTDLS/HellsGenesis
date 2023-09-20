using NebulaSiege.Controller;
using NebulaSiege.Engine.GraphicsProcessing;
using NebulaSiege.Engine.Types;
using NebulaSiege.Loudouts;
using NebulaSiege.Managers;
using NebulaSiege.Menus;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace NebulaSiege.Engine
{
    internal class EngineCore
    {
        public SituationTickController Situations { get; private set; }
        public EventTickController Events { get; private set; }
        public PlayerSpriteTickController Player { get; private set; }

        public EngineInputManager Input { get; private set; }
        public EngineDisplayManager Display { get; private set; }
        public EngineSpriteManager Sprites { get; private set; } //Also contains all of the sprite tick controllers.
        public EngineAudioManager Audio { get; private set; }
        public EngineAssetManager Assets { get; private set; }
        public MenuTickHandler Menus { get; private set; }

        public PrefabPlayerLoadouts PrefabPlayerLoadouts { get; private set; }
        public DirectX DirectX { get; private set; }

        public EngineSettings Settings { get; private set; }

        public bool IsRunning { get; private set; } = false;
        public bool IsRendering { get; set; } = false;
        public bool ShowDebug { get; set; } = false;

        private readonly EngineWorldClock _worldClock;

        static uint _nextSequentialId = 1;
        static readonly object _nextSequentialLock = new();
        /// <summary>
        /// Used to give all loaded sprites a unique ID. Very handy for debugging.
        /// </summary>
        /// <returns></returns>
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

            Display = new EngineDisplayManager(this, drawingSurface, new Size(drawingSurface.Width, drawingSurface.Height));
            Assets = new EngineAssetManager(this);
            Sprites = new EngineSpriteManager(this);
            Input = new EngineInputManager(this);
            Situations = new SituationTickController(this);
            Events = new EventTickController(this);
            Audio = new EngineAudioManager(this);
            Menus = new MenuTickHandler(this);
            Player = new PlayerSpriteTickController(this);
            DirectX = new DirectX(this);

            LoadPrefabs();

            _worldClock = new EngineWorldClock(this);

            Events.Create(new System.TimeSpan(0, 0, 0, 1), NewGameMenuCallback);
        }

        public static EngineSettings LoadSettings()
        {
            string settingsFile = Path.Combine(@"..\..\..\Assets\Data", "EngineSettings.json");

            if (File.Exists(settingsFile) == false)
            {
                File.WriteAllText(settingsFile, JsonConvert.SerializeObject(new EngineSettings(), Formatting.Indented));
            }

            return JsonConvert.DeserializeObject<EngineSettings>(File.ReadAllText(settingsFile));
        }

        public static void SaveSettings(EngineSettings settings)
        {
            string settingsFile = Path.Combine(@"..\..\..\Assets\Data", "EngineSettings.json");
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
                Sprites.RenderPreScaling(DirectX.IntermediateRenderTarget);
                DirectX.IntermediateRenderTarget.EndDraw();

                if (Settings.AutoZoomWhenMoving)
                {
                    DirectX.ApplyScaling((float)Display.SpeedOrientedFrameScalingFactor());
                }
                else
                {
                    DirectX.ApplyScaling((float)Display.BaseDrawScale);
                }
                Sprites.RenderPostScaling(DirectX.ScreenRenderTarget);

                DirectX.ScreenRenderTarget.EndDraw();
            }
            catch
            {
            }
        }

        private void LoadPrefabs()
        {
            var playerLoadoutPath = "Data\\PlayerLoadouts.json";
            var playerLoadoutText = Assets.GetText(playerLoadoutPath);

#if !DEBUG
            if (string.IsNullOrEmpty(playerLoadoutText) == false)
            {
                PrefabPlayerLoadouts = JsonConvert.DeserializeObject<PrefabPlayerLoadouts>(playerLoadoutText);
            }
            else
            {
#endif
            PrefabPlayerLoadouts = new PrefabPlayerLoadouts();

            PrefabPlayerLoadouts.CreateDefaults(); //We couldnt find a file, create a default loadout.

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Converters = new JsonConverter[] { new StringEnumConverter() }
            };
            var defaultLoadout = JsonConvert.SerializeObject(PrefabPlayerLoadouts, Formatting.Indented, settings);

            //Create the missing loadout file.
            Assets.PutText(playerLoadoutPath, defaultLoadout);
#if !DEBUG
            }
#endif
        }

        private void NewGameMenuCallback(EngineCore core, NsEngineCallbackEvent sender, object refObj)
        {
            Menus.Insert(new MenuStartNewGame(this));
        }

        public void Start()
        {
            if (IsRunning == false)
            {
                IsRunning = true;
                Sprites.Start();
                //Sprites.ResetPlayer();

                _worldClock.Start();

                OnStart?.Invoke(this);
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                _worldClock.Stop();
                Sprites.Stop();
                OnStop?.Invoke(this);
                DirectX.Cleanup();
            }
        }

        public bool IsPaused() => _worldClock.IsPaused();
        public void TogglePause() => _worldClock.TogglePause();
        public void Pause() => _worldClock.Pause();
        public void Resume() => _worldClock.Resume();
    }
}