using AI2D.Engine.Menus;
using AI2D.Engine.Scenarios;
using AI2D.GraphicObjects;
using AI2D.GraphicObjects.Bullets;
using AI2D.GraphicObjects.Enemies;
using AI2D.GraphicObjects.PowerUp;
using AI2D.Types;
using AI2D.Weapons;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace AI2D.Engine
{
    public class EngineActors
    {
        private Core _core;

        #region Actors.
        public BaseScenario CurrentScenario { get; private set; }
        public List<BaseScenario> Scenarios = new List<BaseScenario>();
        public List<EngineCallbackEvent> EngineEvents { get; private set; } = new List<EngineCallbackEvent>();
        public List<ObjTextBlock> TextBlocks { get; private set; } = new List<ObjTextBlock>();
        public List<BaseEnemy> Enemies { get; private set; } = new List<BaseEnemy>();
        public List<BasePowerUp> PowerUps { get; private set; } = new List<BasePowerUp>();
        public List<ObjStar> Stars { get; private set; } = new List<ObjStar>();
        public List<ObjDebug> Debugs { get; private set; } = new List<ObjDebug>();
        public List<ObjAnimation> Animations { get; private set; } = new List<ObjAnimation>();
        public List<ObjRadarPositionIndicator> RadarPositionIndicators { get; set; } = new List<ObjRadarPositionIndicator>();
        public List<BaseBullet> Bullets { get; private set; } = new List<BaseBullet>();
        public List<BaseMenu> Menus { get; private set; } = new List<BaseMenu>();
        public ObjPlayer Player { get; private set; }
        public ObjTextBlock PlayerStatsText { get; private set; }
        public ObjTextBlock DebugText { get; private set; }

        #endregion

        #region Resources.

        public AudioClip BackgroundMusicSound { get; private set; }
        public AudioClip RadarBlipsSound { get; private set; }
        public AudioClip DoorIsAjarSound { get; private set; }
        public AudioClip LockedOnBlip { get; private set; }

        private Dictionary<string, AudioClip> _audioClips { get; set; } = new Dictionary<string, AudioClip>();
        private Dictionary<string, Bitmap> _Bitmaps { get; set; } = new Dictionary<string, Bitmap>();


        #endregion

        public EngineActors(Core core)
        {
            _core = core;
        }

        public void Start()
        {
            Player = new ObjPlayer(_core) { Visable = false };

            DoorIsAjarSound = GetSoundCached(@"..\..\Assets\Sounds\Ship\Door Is Ajar.wav", 0.50f, false);
            RadarBlipsSound = GetSoundCached(@"..\..\Assets\Sounds\Ship\Radar Blips.wav", 0.20f, false);
            LockedOnBlip = GetSoundCached(@"..\..\Assets\Sounds\Ship\Locked On.wav", 0.20f, false);

            BackgroundMusicSound = GetSoundCached(@"..\..\Assets\Sounds\Music\Background.wav", 0.25f, true);

            PlayerStatsText = AddNewTextBlock("Consolas", Brushes.WhiteSmoke, 10, new PointD(5,5), true);
            DebugText = AddNewTextBlock("Consolas", Brushes.Aqua, 10, new PointD(5, PlayerStatsText.Y + PlayerStatsText.Height + 10), true);

            BackgroundMusicSound.Play();
        }

        public void Stop()
        {

        }

        public void ResetPlayer()
        {
            Player.ClearWeapons();

            Player.ReadyForDeletion = false;

            Player.Velocity.MaxSpeed = Constants.Limits.MaxPlayerSpeed;
            Player.Velocity.MaxBoost = Constants.Limits.MaxPlayerBoostSpeed;
            Player.Velocity.AvailableBoost = Constants.Limits.MaxPlayerBoost;
            Player.Velocity.MaxRotationSpeed = Constants.Limits.MaxRotationSpeed;
            Player.SetHitPoints(Constants.Limits.StartingPlayerHitpoints);
            Player.SetShieldPoints(Constants.Limits.StartingPlayerShieldPoints);
            Player.Velocity.Angle = new AngleD(45);
            Player.Velocity.ThrottlePercentage = Constants.Limits.MinPlayerThrust;

            Player.X = _core.Display.VisibleSize.Width / 2;
            Player.Y = _core.Display.VisibleSize.Height / 2;

            Player.AddWeapon(new WeaponVulcanCannon(_core) { RoundQuantity = 500 });
            Player.AddWeapon(new WeaponDualVulcanCannon(_core) { RoundQuantity = 100 });
            Player.AddWeapon(new WeaponPhotonTorpedo(_core) { RoundQuantity = 500 });
            Player.AddWeapon(new WeaponPulseMeson(_core) { RoundQuantity = 500 });
            Player.AddWeapon(new WeaponGuidedFragMissile(_core) { RoundQuantity = 500 });

            Player.SelectWeapon(typeof(WeaponVulcanCannon));
        }

        public void ClearScenarios()
        {
            lock (Scenarios)
            {
                foreach (var obj in Scenarios)
                {
                    obj.Cleanup();
                }
            }

            CurrentScenario = null;
            Scenarios.Clear();
        }

        public void NewGame()
        {
            lock (Scenarios)
            {
                ClearScenarios();

                Scenarios.Add(new ScenarioScinzadSkirmish(_core));
                Scenarios.Add(new ScenarioIrlenFormations(_core));
                Scenarios.Add(new ScenarioAvvolAmbush(_core));
            }

            DeleteAllActors();

            AdvanceScenario();
        }

        public void DeleteAllActors()
        {
            DeleteAllPowerUps();
            DeleteAllEnemies();
            DeleteAllBullets();
            DeleteAllAnimations();
        }

        public void ResetAndShowPlayer()
        {
            ResetPlayer();

            Player.Visable = true;

            Player.ShipEngineIdleSound.Play();
            Player.AllSystemsGoSound.Play();
        }

        public void HidePlayer()
        {
            Player.Visable = false;
            Player.ShipEngineIdleSound.Stop();
            Player.ShipEngineRoarSound.Stop();
        }

        public void AdvanceScenario()
        {
            lock (Scenarios)
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
                    CurrentScenario = null;
                    AddNewEngineCallbackEvent(new System.TimeSpan(0, 0, 0, 5), TheDoorIsAjarCallback);
                }
            }
        }

        private void TheDoorIsAjarCallback(Core core, object refObj)
        {
            DoorIsAjarSound.Play();
            InsertMenu(new MenuStartNewGame(_core));
        }

        public List<T> VisibleTextBlocksOfType<T>() where T : class
        {
            return (from o in _core.Actors.TextBlocks
                    where o is T
                    && o.Visable == true
                    select o as T).ToList();
        }

        public List<T> VisiblePowerUpsOfType<T>() where T : class
        {
            return (from o in _core.Actors.PowerUps
                    where o is T
                    && o.Visable == true
                    select o as T).ToList();
        }

        public List<T> VisibleEnemiesOfType<T>() where T : class
        {
            return (from o in _core.Actors.Enemies
                    where o is T
                    && o.Visable == true
                    select o as T).ToList();
        }

        public void DeleteAllPowerUps()
        {
            while (PowerUps.Count > 0)
            {
                lock (PowerUps)
                {
                    foreach (var obj in PowerUps)
                    {
                        obj.ReadyForDeletion = true;
                    }
                }
            }
        }

        public void DeleteAllEnemies()
        {
            while (Enemies.Count > 0)
            {
                lock (Enemies)
                {
                    foreach (var obj in Enemies)
                    {
                        obj.ReadyForDeletion = true;
                    }
                }
            }
        }

        public void DeleteAllBullets()
        {
            while (Enemies.Count > 0)
            {
                lock (Bullets)
                {
                    foreach (var obj in Bullets)
                    {
                        obj.ReadyForDeletion = true;
                    }
                }
            }
        }
        public void DeleteAllAnimations()
        {
            while (Animations.Count > 0)
            {
                lock (Animations)
                {
                    foreach (var obj in Animations)
                    {
                        obj.ReadyForDeletion = true;
                    }
                }
            }
        }

        public Bitmap GetBitmapCached(string path)
        {
            Bitmap result = null;

            path = path.ToLower();

            lock (_Bitmaps)
            {
                if (_Bitmaps.ContainsKey(path))
                {
                    result = _Bitmaps[path].Clone() as Bitmap;
                }
                else
                {
                    result = new Bitmap(Image.FromFile(path));
                    _Bitmaps.Add(path, result);
                }
            }

            return result;
        }

        public AudioClip GetSoundCached(string wavFilePath, float initialVolumne, bool loopForever = false)
        {
            lock (_audioClips)
            {
                AudioClip result = null;

                wavFilePath = wavFilePath.ToLower();

                if (_audioClips.ContainsKey(wavFilePath))
                {
                    result = _audioClips[wavFilePath];
                }
                else
                {
                    result = new AudioClip(wavFilePath, initialVolumne, loopForever);
                    _audioClips.Add(wavFilePath, result);
                }

                return result;
            }
        }

        #region Factories.

        public ObjRadarPositionIndicator AddNewRadarPositionIndicator()
        {
            lock (RadarPositionIndicators)
            {
                var obj = new ObjRadarPositionIndicator(_core);
                RadarPositionIndicators.Add(obj);
                return obj;
            }
        }

        public void DeleteRadarPositionIndicator(ObjRadarPositionIndicator obj)
        {
            lock (RadarPositionIndicators)
            {
                obj.Cleanup();
                obj.Visable = false;
                RadarPositionIndicators.Remove(obj);
            }
        }

        public void PlaceAnimationOnTopOf(ObjAnimation animation, BaseGraphicObject defaultPosition)
        {
            lock (Animations)
            {
                animation.X = defaultPosition.X;
                animation.Y = defaultPosition.Y;
                animation.RotationMode = Types.RotationMode.Clip; //Much less expensive. Use this or NONE if you can.
                Animations.Add(animation);
            }
        }

        public ObjAnimation AddNewAnimation(string imageFrames, Size frameSize, int _frameDelayMilliseconds = 10, ObjAnimation.PlayMode playMode = null)
        {
            lock (Animations)
            {
                ObjAnimation obj = new ObjAnimation(_core, imageFrames, frameSize, _frameDelayMilliseconds, playMode);
                Animations.Add(obj);
                return obj;
            }
        }

        public void DeleteAnimation(ObjAnimation obj)
        {
            lock (Animations)
            {
                obj.Cleanup();
                Animations.Remove(obj);
            }
        }

        public EngineCallbackEvent AddNewEngineCallbackEvent(
            TimeSpan countdown, EngineCallbackEvent.OnExecute executeCallback, object refObj,
            EngineCallbackEvent.CallbackEventMode callbackEventMode = EngineCallbackEvent.CallbackEventMode.OneTime,
            EngineCallbackEvent.CallbackEventAsync callbackEventAsync = EngineCallbackEvent.CallbackEventAsync.Synchronous)
        {
            lock (EngineEvents)
            {
                EngineCallbackEvent obj = new EngineCallbackEvent(_core, countdown, executeCallback, refObj, callbackEventMode, callbackEventAsync);
                EngineEvents.Add(obj);
                return obj;
            }
        }

        public EngineCallbackEvent AddNewEngineCallbackEvent(TimeSpan countdown, EngineCallbackEvent.OnExecute executeCallback, object refObj)
        {
            lock (EngineEvents)
            {
                EngineCallbackEvent obj = new EngineCallbackEvent(_core, countdown, executeCallback, refObj);
                EngineEvents.Add(obj);
                return obj;
            }
        }

        public EngineCallbackEvent AddNewEngineCallbackEvent(TimeSpan countdown, EngineCallbackEvent.OnExecute executeCallback)
        {
            lock (EngineEvents)
            {
                EngineCallbackEvent obj = new EngineCallbackEvent(_core, countdown, executeCallback);
                EngineEvents.Add(obj);
                return obj;
            }
        }

        public EngineCallbackEvent InjectCallbackEvent(EngineCallbackEvent obj)
        {
            lock (EngineEvents)
            {
                EngineEvents.Add(obj);
                return obj;
            }
        }

        public void DeleteEngineCallbackEvent(EngineCallbackEvent obj)
        {
            lock (EngineEvents)
            {
                EngineEvents.Remove(obj);
            }
        }

        public ObjRadarPositionTextBlock AddNewRadarPositionTextBlock(string font, Brush color, double size, PointD location)
        {
            lock (TextBlocks)
            {
                var obj = new ObjRadarPositionTextBlock(_core, font, color, size, location);
                TextBlocks.Add(obj);
                return obj;
            }
        }

        public ObjTextBlock AddNewTextBlock(string font, Brush color, double size, PointD location, bool isPositionStatic)
        {
            lock (TextBlocks)
            {
                var obj = new ObjTextBlock(_core, font, color, size, location, isPositionStatic);
                TextBlocks.Add(obj);
                return obj;
            }
        }

        public void DeleteTextBlock(ObjTextBlock obj)
        {
            lock (TextBlocks)
            {
                obj.Cleanup();
                TextBlocks.Remove(obj);
            }
        }

        public ObjDebug AddNewDebug()
        {
            lock (Debugs)
            {
                var obj = new ObjDebug(_core);
                Debugs.Add(obj);
                return obj;
            }
        }

        public void DeleteDebug(ObjDebug obj)
        {
            lock (Debugs)
            {
                obj.Cleanup();
                Debugs.Remove(obj);
            }
        }

        public ObjStar AddNewStar(double x, double y)
        {
            lock (Stars)
            {
                var obj = new ObjStar(_core)
                {
                    X = x,
                    Y = y
                };
                Stars.Add(obj);
                return obj;
            }
        }

        public ObjStar AddNewStar()
        {
            lock (Stars)
            {
                var obj = new ObjStar(_core);
                Stars.Add(obj);
                return obj;
            }
        }

        public void DeleteStar(ObjStar obj)
        {
            lock (Stars)
            {
                obj.Cleanup();
                Stars.Remove(obj);
            }
        }

        public void InjectEnemy(BaseEnemy obj)
        {
            lock (Enemies)
            {
                Enemies.Add(obj);
            }
        }

        public void InjectPowerUp(BasePowerUp obj)
        {
            lock (PowerUps)
            {
                PowerUps.Add(obj);
            }
        }

        public void DeletePowerUp(BasePowerUp obj)
        {
            lock (PowerUps)
            {
                obj.Cleanup();
                PowerUps.Remove(obj);
            }
        }

        public T AddNewPowerUp<T>() where T : BasePowerUp
        {
            lock (PowerUps)
            {
                object[] param = { _core };
                BasePowerUp obj = (BasePowerUp)Activator.CreateInstance(typeof(T), param);

                obj.Location = _core.Display.RandomOffScreenLocation(100, 1000);

                PowerUps.Add(obj);
                return (T)obj;
            }
        }

        public T AddNewEnemy<T>() where T : BaseEnemy
        {
            lock (Enemies)
            {
                object[] param = { _core };
                BaseEnemy obj = (BaseEnemy)Activator.CreateInstance(typeof(T), param);

                obj.Location = _core.Display.RandomOffScreenLocation();
                obj.Velocity.MaxSpeed = Utility.Random.Next(Constants.Limits.MinSpeed, Constants.Limits.MaxSpeed);
                obj.Velocity.Angle.Degrees = Utility.Random.Next(0, 360);

                Enemies.Add(obj);
                return (T)obj;
            }
        }

        public void DeleteEnemy(BaseEnemy obj)
        {
            lock (Enemies)
            {
                obj.Cleanup();
                Enemies.Remove(obj);
            }
        }

        public BaseBullet AddNewLockedBullet(WeaponBase weapon, BaseGraphicObject firedFrom, BaseGraphicObject lockedTarget, PointD xyOffset = null)
        {
            lock (Bullets)
            {
                var obj = weapon.CreateBullet(lockedTarget, xyOffset);
                Bullets.Add(obj);
                return obj;
            }
        }

        public BaseBullet AddNewBullet(WeaponBase weapon, BaseGraphicObject firedFrom, PointD xyOffset = null)
        {
            lock (Bullets)
            {
                var obj = weapon.CreateBullet(null, xyOffset);
                Bullets.Add(obj);
                return obj;
            }
        }

        public void DeleteBullet(BaseBullet obj)
        {
            lock (Bullets)
            {
                obj.Cleanup();
                Bullets.Remove(obj);
            }
        }

        public void InsertMenu(BaseMenu menu)
        {
            lock (Menus)
            {
                Menus.Add(menu);
            }
        }

        public void DeleteMenu(BaseMenu menu)
        {
            lock (Menus)
            {
                menu.Cleanup();
                Menus.Remove(menu);
            }
        }

        #endregion

        #region Rendering.

        public void Render(Graphics dc)
        {
            _core.IsRendering = true;

            var timeout = TimeSpan.FromMilliseconds(1);
            bool lockTaken = false;

            try
            {
                Monitor.TryEnter(_core.DrawingSemaphore, timeout, ref lockTaken);
                if (lockTaken)
                {
                    lock (TextBlocks)
                    {
                        foreach (var obj in TextBlocks)
                        {
                            obj.Render(dc);
                        }
                    }

                    lock (PowerUps)
                    {
                        foreach (var obj in PowerUps)
                        {
                            if (_core.Display.VisibleBounds.IntersectsWith(obj.Bounds))
                            {
                                obj.Render(dc);
                            }
                        }
                    }

                    lock (Animations)
                    {
                        foreach (var obj in Animations)
                        {
                            if (_core.Display.VisibleBounds.IntersectsWith(obj.Bounds))
                            {
                                obj.Render(dc);
                            }
                        }
                    }

                    lock (Enemies)
                    {
                        foreach (var obj in Enemies)
                        {
                            if (_core.Display.VisibleBounds.IntersectsWith(obj.Bounds))
                            {
                                obj.Render(dc);
                            }
                        }
                    }

                    lock (Bullets)
                    {
                        foreach (var obj in Bullets)
                        {
                            if (_core.Display.VisibleBounds.IntersectsWith(obj.Bounds))
                            {
                                obj.Render(dc);
                            }
                        }
                    }

                    lock (Stars)
                    {
                        foreach (var obj in Stars)
                        {
                            if (_core.Display.VisibleBounds.IntersectsWith(obj.Bounds))
                            {
                                obj.Render(dc);
                            }
                        }
                    }

                    lock (Debugs)
                    {
                        foreach (var obj in Debugs)
                        {
                            obj.Render(dc);
                        }
                    }

                    lock (RadarPositionIndicators)
                    {
                        foreach (var obj in RadarPositionIndicators)
                        {
                            obj.Render(dc);
                        }
                    }

                    Player?.Render(dc);

                    lock (Menus)
                    {
                        foreach (var obj in Menus)
                        {
                            obj.Render(dc);
                        }
                    }
                }
                else
                {
                }
            }
            finally
            {
                // Ensure that the lock is released.
                if (lockTaken)
                {
                    Monitor.Exit(_core.DrawingSemaphore);
                }
            }

            _core.IsRendering = false;

        }

        #endregion
    }
}