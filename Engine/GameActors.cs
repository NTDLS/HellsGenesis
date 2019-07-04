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
    public class GameActors
    {
        private Game _game;
        private Dictionary<string, AudioClip> _audioClips { get; set; } = new Dictionary<string, AudioClip>();
        private Dictionary<string, Bitmap> _Bitmaps { get; set; } = new Dictionary<string, Bitmap>();

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

        public GameActors(Game game)
        {
            _game = game;

            BackgroundMusicSound = GetSoundCached(@"..\..\Assets\Sounds\Background Music.wav", 0.25f, true);
            ShipEngineRoarSound = GetSoundCached(@"..\..\Assets\Sounds\Engine Roar.wav", 1.0f, true);
            ShipEngineIdleSound = GetSoundCached(@"..\..\Assets\Sounds\Engine Idle.wav", 0.5f, true);
            AllSystemsGoSound = GetSoundCached(@"..\..\Assets\Sounds\All Systems Go.wav", 0.5f, false);

            PlayerStatsText = CreateTextBlock("ComicSans", 10, 5, 5);
            QuadrantText = CreateTextBlock("ComicSans", 10, 5, PlayerStatsText.Y + PlayerStatsText.Height + 10);
            DebugText = CreateTextBlock("ComicSans", 10, 5, QuadrantText.Y + QuadrantText.Height + 10);
        }

        public void ShowNewPlayer()
        {
            if (Player == null)
            {
                Player = new ObjPlayer(_game);
            }

            Player.Velocity.Speed = 5;
            Player.RotationSpeed = 3;
            Player.Visable = true;
            Player.HitPoints = 100;
            Player.BulletsRemaining = 100;
            Player.X = _game.Display.VisibleSize.Width / 2;
            Player.Y = _game.Display.VisibleSize.Height / 2;

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
                ObjAnimation obj = new ObjAnimation(_game, imageFrames, frameSize);
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
                ObjStar obj = new ObjStar(_game)
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
                int deltaX = createInQuad.Bounds.X - _game.CurrentQuadrant.Bounds.X;
                int deltaY = createInQuad.Bounds.Y - _game.CurrentQuadrant.Bounds.Y;



                ObjStar obj = new ObjStar(_game)
                {
                    X = Utility.Random.Next(deltaX, createInQuad.Bounds.Width - 100),
                    Y = Utility.Random.Next(deltaY, createInQuad.Bounds.Height - 100)
                    //X = createInQuad.Bounds.X - (int)_game.BackgroundOffset.X, //This adds a start to X:0 of the current screen.
                    //Y = createInQuad.Bounds.Y - (int)_game.BackgroundOffset.Y //This adds a start to Y:0 of the current screen.
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

        public ObjTextBlock CreateTextBlock(string font, double size, double x, double y)
        {
            lock (TextBlocks)
            {
                ObjTextBlock obj = new ObjTextBlock(_game, font, size, x, y);
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
                ObjStar obj = new ObjStar(_game);
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
                ObjEnemy obj = new ObjEnemy(_game);
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
                ObjBullet obj = new ObjBullet(_game, firedFrom);
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

        public void RenderObjects(Graphics dc)
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
