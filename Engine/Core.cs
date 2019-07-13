using AI2D.Engine.Menus;
using AI2D.Engine.Scenarios;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace AI2D.Engine
{
    public class Core
    {
        public EngineInput Input { get; private set; }
        public EngineDisplay Display { get; private set; }
        public EngineActors Actors { get; private set; }
        public List<BaseScenario> Scenarios = new List<BaseScenario>();
        public bool IsRunning { get; private set; } = false;
        public bool ShowDebug { get; set; } = false;
        public object DrawingSemaphore { get; set; } = new object();
        public BaseScenario CurrentScenario { get; private set; }

        private EngineThread _engineThread;

        #region Events.

        public delegate void StartEvent(Core sender);
        public event StartEvent OnStart;

        public delegate void StopEvent(Core sender);
        public event StopEvent OnStop;

        #endregion

        public Core(Control drawingSurface, Size visibleSize)
        {
            Display = new EngineDisplay(drawingSurface, visibleSize);
            Actors = new EngineActors(this);
            Input = new EngineInput(this);
            _engineThread = new EngineThread(this);

            Actors.InsertMenu(new MenuStartNewGame(this));
        }

        public void ClearScenarios()
        {
            CurrentScenario = null;
            Scenarios.Clear();
        }

        public void NewGame()
        {
            ClearScenarios();

            Scenarios.Add(new ScenarioIrlenFormations(this));
            Scenarios.Add(new ScenarioScinzadSkirmish(this));
            Scenarios.Add(new ScenarioAvvolAmbush(this));

            Actors.DeletaAllActors();

            AdvanceScenario();
        }

        public void Start()
        {
            if (IsRunning == false)
            {
                IsRunning = true;
                Actors.Start();
                Actors.ResetPlayer();

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

                    var coinAnimation = new ObjAnimation(_core, @"..\..\Assets\Graphics\Animation\Coin.png", new Size(32, 23), 20, mode);
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

        public void Pause()
        {
            _engineThread.Pause();
        }

        public void Resume()
        {
            _engineThread.Resume();
        }

        public void AdvanceScenario()
        {
            if (CurrentScenario != null)
            {
                Scenarios.Remove(CurrentScenario);
            }

            if (Scenarios.Count > 0)
            {
                CurrentScenario = Scenarios[0];
                CurrentScenario.Execute();
            }
            else
            {
                Actors.AddNewEngineCallbackEvent(new System.TimeSpan(0, 0, 0, 5), TheDoorIsAjarCallback);
            }
        }

        private void TheDoorIsAjarCallback(Core core, object refObj)
        {
            Actors.DoorIsAjarSound.Play();
            Actors.InsertMenu(new MenuStartNewGame(this));
        }
    }
}