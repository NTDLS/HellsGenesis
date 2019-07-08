using AI2D.GraphicObjects.Enemies;
using AI2D.Weapons;
using System.Drawing;
using System.Windows.Forms;

namespace AI2D.Engine
{
    public class Core
    {
        public EngineInput Input { get; private set; }
        public EngineDisplay Display { get; private set; }
        public EngineActors Actors { get; private set; }

        public bool IsRunning { get; private set; } = false;
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
        }

        public void Start()
        {
            if (IsRunning == false)
            {
                IsRunning = true;

                Actors.BackgroundMusicSound.Play();

                Actors.ResetPlayer();
                _engineThread.Start();

                //This is debug stuff. Will be moved to a logic engine.
                Actors.CreateEngineCallbackEvent(new System.TimeSpan(0, 0, 0, 0, 500), FirstShowPlayerCallback);

                //This is debug stuff. Will be moved to a logic engine.
                Actors.CreateEngineCallbackEvent(new System.TimeSpan(0, 0, 0, 0, 1000),
                    AddFreshEnemiesCallback, null, EngineCallbackEvent.CallbackEventMode.Recurring);

                //This is debug stuff. Will be moved to a logic engine.
                //Actors.CreateEngineCallbackEvent(new System.TimeSpan(0, 0, 0, 0, 0),
                //    AddDebugObjectsCallback, null, EngineCallbackEvent.CallbackEventMode.OneTime);

                OnStart?.Invoke(this);
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                _engineThread.Stop();
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


        /// <summary>
        /// This is debug stuff. Will be moved to a logic engine.
        /// </summary>
        /// <param name="core"></param>
        /// <param name="refObj"></param>
        private void AddDebugObjectsCallback(Core core, object refObj)
        {
            Actors.CreateDebug();
            Actors.CreateDebug();
        }

        /// <summary>
        /// This is debug stuff. Will be moved to a logic engine.
        /// </summary>
        /// <param name="core"></param>
        /// <param name="refObj"></param>
        private void FirstShowPlayerCallback(Core core, object refObj)
        {
            Actors.ResetAndShowPlayer();
        }

        /// <summary>
        /// This is debug stuff. Will be moved to a logic engine.
        /// </summary>
        /// <param name="core"></param>
        /// <param name="refObj"></param>
        private void AddFreshEnemiesCallback(Core core, object refObj)
        {
            if (Actors.Enemies.Count < -1)
            {
                for (int i = 0; i < 2; i++)
                {
                    var enemy = Actors.CreateEnemy<EnemyAvvol>();

                    enemy.AddWeapon(new WeaponPhotonTorpedo(this)
                    {
                        RoundQuantity = 10, //Could make the enemy retreat after running out of ammo?
                        FireDelayMilliseconds = 500,
                    });

                    enemy.AddWeapon(new WeaponVulcanCannon(this)
                    {
                        FireDelayMilliseconds = 250,
                        //RoundQuantity = 100 //Could make the enemy retreat after running out of ammo?
                    });

                    enemy.SelectWeapon(typeof(WeaponPhotonTorpedo));
                }

                for (int i = 0; i < 2; i++)
                {
                    var enemy = Actors.CreateEnemy<EnemyScinzad>();

                    enemy.AddWeapon(new WeaponVulcanCannon(this)
                    {
                        FireDelayMilliseconds = 250,
                        //RoundQuantity = 100 //Could make the enemy retreat after running out of ammo?
                    });

                    enemy.SelectWeapon(typeof(WeaponPhotonTorpedo));
                }


            }
        }
    }
}