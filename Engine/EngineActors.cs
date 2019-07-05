using AI2D.Objects;
using AI2D.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI2D.Engine
{
    public class EngineActors
    {
        private Core _core;
        private Dictionary<string, AudioClip> _audioClips { get; set; } = new Dictionary<string, AudioClip>();
        private Dictionary<string, Bitmap> _Bitmaps { get; set; } = new Dictionary<string, Bitmap>();

        public List<EngineCallbackEvent> EngineEvents { get; private set; } = new List<EngineCallbackEvent>();

        public List<ObjTextBlock> TextBlocks { get; private set; } = new List<ObjTextBlock>();
        public List<ObjEnemy> Enemies { get; private set; } = new List<ObjEnemy>();
        public List<ObjStar> Stars { get; private set; } = new List<ObjStar>();
        public List<ObjAnimation> Animations { get; private set; } = new List<ObjAnimation>();
        public List<ObjBullet> Bullets { get; private set; } = new List<ObjBullet>();
        public ObjPlayer Player { get; private set; }

        public AudioClip ShipEngineRoarSound { get; private set; }
        public AudioClip ShipEngineIdleSound { get; private set; }
        public AudioClip AllSystemsGoSound { get; private set; }
        public AudioClip BackgroundMusicSound { get; private set; }

        public ObjTextBlock PlayerStatsText { get; private set; }
        public ObjTextBlock QuadrantText { get; private set; }
        public ObjTextBlock DebugText { get; private set; }

        public EngineActors(Core core)
        {
            _core = core;

            BackgroundMusicSound = GetSoundCached(@"..\..\Assets\Sounds\Background Music.wav", 0.25f, true);
            ShipEngineRoarSound = GetSoundCached(@"..\..\Assets\Sounds\Engine Roar.wav", 1.0f, true);
            ShipEngineIdleSound = GetSoundCached(@"..\..\Assets\Sounds\Engine Idle.wav", 0.6f, true);
            AllSystemsGoSound = GetSoundCached(@"..\..\Assets\Sounds\All Systems Go.wav", 0.75f, false);

            //PlayerStatsText = CreateTextBlock("ComicSans", 10, 5, 5);
            QuadrantText = CreateTextBlock("Consolas", 10, 5, 5);
            //DebugText = CreateTextBlock("ComicSans", 10, 5, QuadrantText.Y + QuadrantText.Height + 10);
        }

        public void ResetPlayer()
        {
            if (Player == null)
            {
                //There is a bit of a dependency between Code and Actors, so this can not be done in the constructor.
                Player = new ObjPlayer(_core) { Visable = false };
            }

            Player.Velocity.Speed = 5;
            Player.RotationSpeed = 3;
            Player.HitPoints = 100;
            Player.BulletsRemaining = 1000;
            Player.X = _core.Display.VisibleSize.Width / 2;
            Player.Y = _core.Display.VisibleSize.Height / 2;
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

        public void PlaceAnimationOnTopOf(ObjAnimation animation, ObjBase defaultPosition)
        {
            lock (Animations)
            {
                animation.X = defaultPosition.X + ((defaultPosition.Size.Width - animation.Size.Width) / 2.0);
                animation.Y = defaultPosition.Y + ((defaultPosition.Size.Height - animation.Size.Height) / 2.0);
                animation.Velocity = defaultPosition.Velocity;
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

        public ObjStar CreateStar(Quadrant createInQuad)
        {
            lock (Stars)
            {
                int deltaX = createInQuad.Bounds.X - _core.Display.CurrentQuadrant.Bounds.X;
                int deltaY = createInQuad.Bounds.Y - _core.Display.CurrentQuadrant.Bounds.Y;



                ObjStar obj = new ObjStar(_core)
                {
                    X = Utility.Random.Next(deltaX, createInQuad.Bounds.Width - 100),
                    Y = Utility.Random.Next(deltaY, createInQuad.Bounds.Height - 100)
                    //X = createInQuad.Bounds.X - (int)_core.BackgroundOffset.X, //This adds a start to X:0 of the current screen.
                    //Y = createInQuad.Bounds.Y - (int)_core.BackgroundOffset.Y //This adds a start to Y:0 of the current screen.
                    //X = 500,
                    //Y = 500

                    //X = deltaX,
                    //Y = deltaY
                };

                Console.WriteLine($"x{obj.X}, y{obj.Y}      {deltaX} - {deltaY}");

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

        public ObjTextBlock CreateTextBlock(string font, double size, double x, double y)
        {
            lock (TextBlocks)
            {
                ObjTextBlock obj = new ObjTextBlock(_core, font, size, x, y);
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

        public ObjEnemy CreateEnemy()
        {
            lock (Enemies)
            {
                ObjEnemy obj = new ObjEnemy(_core);
                Enemies.Add(obj);
                return obj;
            }
        }

        public void DeleteEnemy(ObjEnemy obj)
        {
            lock (Enemies)
            {
                obj.Cleanup();
                Enemies.Remove(obj);
            }
        }

        public ObjBullet CreateBullet(ObjBase firedFrom)
        {
            lock (Bullets)
            {
                ObjBullet obj = new ObjBullet(_core, firedFrom);
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
                    obj.Render(dc);
                }
            }
        }

        void RenderEnemies(Graphics dc)
        {
            lock (Enemies)
            {
                foreach (var obj in Enemies)
                {
                    obj.Render(dc);
                }
            }
        }

        void RenderBullets(Graphics dc)
        {
            lock (Bullets)
            {
                foreach (var obj in Bullets)
                {
                    obj.Render(dc);
                }
            }
        }
        void RenderStars(Graphics dc)
        {
            lock (Stars)
            {
                foreach (var obj in Stars)
                {
                    //if (currentQuad.Bounds.IntersectsWith(obj.BoundsI))
                    {
                        obj.Render(dc);
                    }
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
        }

        #endregion
    }
}
