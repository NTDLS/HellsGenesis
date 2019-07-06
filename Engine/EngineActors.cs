using AI2D.GraphicObjects;
using AI2D.GraphicObjects.Enemies;
using AI2D.Types;
using AI2D.Weapons;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace AI2D.Engine
{
    public class EngineActors
    {
        private Core _core;
        private Dictionary<string, AudioClip> _audioClips { get; set; } = new Dictionary<string, AudioClip>();
        private Dictionary<string, Bitmap> _Bitmaps { get; set; } = new Dictionary<string, Bitmap>();

        public List<EngineCallbackEvent> EngineEvents { get; private set; } = new List<EngineCallbackEvent>();

        public List<ObjTextBlock> TextBlocks { get; private set; } = new List<ObjTextBlock>();
        public List<BaseEnemy> Enemies { get; private set; } = new List<BaseEnemy>();
        public List<ObjStar> Stars { get; private set; } = new List<ObjStar>();
        public List<ObjDebug> Debugs { get; private set; } = new List<ObjDebug>();
        public List<ObjAnimation> Animations { get; private set; } = new List<ObjAnimation>();
        public List<ObjBullet> Bullets { get; private set; } = new List<ObjBullet>();
        public ObjPlayer Player { get; private set; }

        public AudioClip ShipEngineRoarSound { get; private set; }
        public AudioClip ShipEngineIdleSound { get; private set; }
        public AudioClip AllSystemsGoSound { get; private set; }
        public AudioClip BackgroundMusicSound { get; private set; }

        public ObjTextBlock PlayerStatsText { get; private set; }
        public ObjTextBlock DebugText { get; private set; }

        public EngineActors(Core core)
        {
            _core = core;

            BackgroundMusicSound = GetSoundCached(@"..\..\Assets\Sounds\Background Music.wav", 0.25f, true);
            ShipEngineRoarSound = GetSoundCached(@"..\..\Assets\Sounds\Engine Roar.wav", 1.0f, true);
            ShipEngineIdleSound = GetSoundCached(@"..\..\Assets\Sounds\Engine Idle.wav", 0.6f, true);
            AllSystemsGoSound = GetSoundCached(@"..\..\Assets\Sounds\All Systems Go.wav", 0.75f, false);
            
            PlayerStatsText = CreateTextBlock("Consolas", Brushes.WhiteSmoke, 10, 5, 5);
            DebugText = CreateTextBlock("Consolas", Brushes.Aqua, 10, 5, PlayerStatsText.Y + PlayerStatsText.Height + 10);
        }

        public void ResetPlayer()
        {
            if (Player == null)
            {
                //There is a bit of a dependency between Code and Actors, so this can not be done in the constructor.
                Player = new ObjPlayer(_core) { Visable = false };
            }

            Player.ClearWeapons();

            Player.Velocity.Speed = 5;
            Player.RotationSpeed = 3;
            Player.HitPoints = 500;

            Player.X = _core.Display.VisibleSize.Width / 2;
            Player.Y = _core.Display.VisibleSize.Height / 2;

            Player.AddWeapon(new WeaponVulcanCannon(_core) { RoundQuantity = 500 });
            Player.AddWeapon(new WeaponDualVulcanCannon(_core) { RoundQuantity = int.MaxValue });
            Player.AddWeapon(new WeaponPhotonTorpedo(_core) { RoundQuantity = 500 });

            Player.SelectWeapon(typeof(WeaponVulcanCannon));
        }

        public void ResetAndShowPlayer()
        {
            ResetPlayer();

            Player.Visable = true;

            ShipEngineIdleSound.Play();
            AllSystemsGoSound.Play();
        }

        public void HidePlayer()
        {
            Player.Visable = false;
            ShipEngineIdleSound.Play();
        }

        public Bitmap GetBitmapCached(string path)
        {
            lock (_Bitmaps)
            {
                Bitmap result = null;

                path = path.ToLower();

                if (_Bitmaps.ContainsKey(path))
                {
                    result = _Bitmaps[path].Clone() as Bitmap;
                }
                else
                {
                    result = new Bitmap(Image.FromFile(path));
                    _Bitmaps.Add(path, result);
                }

                return result;
            }
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

        public void PlaceAnimationOnTopOf(ObjAnimation animation, BaseGraphicObject defaultPosition)
        {
            lock (Animations)
            {
                //animation.X = defaultPosition.X + ((defaultPosition.Size.Width - animation.Size.Width) / 2.0);
                //animation.Y = defaultPosition.Y + ((defaultPosition.Size.Height - animation.Size.Height) / 2.0);

                animation.X = defaultPosition.X;
                animation.Y = defaultPosition.Y;

                animation.Velocity = defaultPosition.Velocity;
                animation.RotationMode = Types.RotationMode.Clip; //Much less expensive. Use this or NONE if you can.

                Animations.Add(animation);
            }
        }

        public ObjAnimation CreateAnimation(string imageFrames, Size frameSize)
        {
            lock (Animations)
            {
                ObjAnimation obj = new ObjAnimation(_core, imageFrames, frameSize);
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

        public ObjStar CreateStar(double x, double y)
        {
            lock (Stars)
            {
                ObjStar obj = new ObjStar(_core)
                {
                    X = x,
                    Y = y
                };
                Stars.Add(obj);
                return obj;
            }
        }

        public EngineCallbackEvent CreateEngineCallbackEvent(
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

        public EngineCallbackEvent CreateEngineCallbackEvent(TimeSpan countdown, EngineCallbackEvent.OnExecute executeCallback, object refObj)
        {
            lock (EngineEvents)
            {
                EngineCallbackEvent obj = new EngineCallbackEvent(_core, countdown, executeCallback, refObj);
                EngineEvents.Add(obj);
                return obj;
            }
        }

        public EngineCallbackEvent CreateEngineCallbackEvent(TimeSpan countdown, EngineCallbackEvent.OnExecute executeCallback)
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

        public ObjTextBlock CreateTextBlock(string font, Brush color, double size, double x, double y)
        {
            lock (TextBlocks)
            {
                ObjTextBlock obj = new ObjTextBlock(_core, font, color, size, x, y);
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

        public ObjDebug CreateDebug()
        {
            lock (Debugs)
            {
                ObjDebug obj = new ObjDebug(_core);
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

        public ObjStar CreateStar()
        {
            lock (Stars)
            {
                ObjStar obj = new ObjStar(_core);
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

        public T CreateEnemy<T>() where T : BaseEnemy
        {
            lock (Enemies)
            {
                object[] param = { _core };
                BaseEnemy obj = (BaseEnemy)Activator.CreateInstance(typeof(T), param);
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

        public ObjBullet CreateBullet(string imagePath, int damage, BaseGraphicObject firedFrom, PointD xyOffset = null)
        {
            lock (Bullets)
            {
                ObjBullet obj = new ObjBullet(_core, imagePath, damage, firedFrom, xyOffset);
                Bullets.Add(obj);
                return obj;
            }
        }

        public void DeleteBullet(ObjBullet obj)
        {
            lock (Bullets)
            {
                obj.Cleanup();
                Bullets.Remove(obj);
            }
        }

        #endregion

        #region Rendering.

        private void RenderText(Graphics dc)
        {
            lock (TextBlocks)
            {
                foreach (var obj in TextBlocks)
                {
                    obj.Render(dc);
                }
            }
        }

        void RenderAnimations(Graphics dc)
        {
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
        }

        void RenderEnemies(Graphics dc)
        {
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
        }

        void RenderBullets(Graphics dc)
        {
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
        }
        void RenderStars(Graphics dc)
        {
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
        }

        void RenderDebugs(Graphics dc)
        {
            lock (Debugs)
            {
                foreach (var obj in Debugs)
                {
                    obj.Render(dc);
                }
            }
        }

        void RenderPlayer(Graphics dc)
        {
            if (Player != null)
            {
                Player.Render(dc);
            }
        }

        public void Render(Graphics dc)
        {
            RenderStars(dc);
            RenderBullets(dc);
            RenderEnemies(dc);
            RenderPlayer(dc);
            RenderText(dc);
            RenderAnimations(dc);
            RenderDebugs(dc);
        }

        #endregion
    }
}
