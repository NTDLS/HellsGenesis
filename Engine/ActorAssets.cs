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
    public class ActorAssets
    {
        private Game _game;

        public TextBlock DebugText { get; set; }
        public TextBlock PlayerStatsText { get; set; }
        public List<Enemy> Enemies { get; set; } = new List<Enemy>();
        public List<Star> Stars { get; set; } = new List<Star>();
        public List<Animation> Animations { get; set; } = new List<Animation>();
        public List<Bullet> Bullets { get; set; } = new List<Bullet>();
        public Player Player { get; set; }
        private Dictionary<string, AudioClip> _audioClips { get; set; } = new Dictionary<string, AudioClip>();
        private Dictionary<string, Bitmap> _Bitmaps { get; set; } = new Dictionary<string, Bitmap>();

        public AudioClip ShipEngineRoarSound { get; set; }
        public AudioClip ShipEngineIdleSound { get; set; }
        public AudioClip AllSystemsGoSound { get; set; }
        public AudioClip BackgroundMusicSound { get; set; }

        public ActorAssets(Game game)
        {
            _game = game;

            BackgroundMusicSound = GetAudioClip(@"..\..\Assets\Sounds\Background Music.wav", 0.25f, true);
            ShipEngineRoarSound = GetAudioClip(@"..\..\Assets\Sounds\Engine Roar.wav", 1.0f, true);
            ShipEngineIdleSound = GetAudioClip(@"..\..\Assets\Sounds\Engine Idle.wav", 0.5f, true);
            AllSystemsGoSound = GetAudioClip(@"..\..\Assets\Sounds\All Systems Go.wav", 0.5f, false);

            PlayerStatsText = new TextBlock(_game.Display, "ComicSans", 10, 5, 5);
            DebugText = new TextBlock(_game.Display, "ComicSans", 10, 5, PlayerStatsText.Height + 10);
        }

        public void ShowNewPlayer()
        {
            if (Player == null)
            {
                Player = new Player(_game);
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

        public Bitmap GetBitmap(string path)
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

        public AudioClip GetAudioClip(string wavFilePath, float initialVolumne, bool loopForever = false)
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

        public Animation CreateAnimation(string imageFrames, Size frameSize)
        {
            lock (Animations)
            {
                Animation obj = new Animation(_game, imageFrames, frameSize);
                Animations.Add(obj);
                return obj;
            }
        }

        public void PlaceAnimationOnTopOf(Animation animation, BaseObject defaultPosition)
        {
            lock (Animations)
            {
                animation.X = defaultPosition.X + ((defaultPosition.Size.Width - animation.Size.Width) / 2.0);
                animation.Y = defaultPosition.Y + ((defaultPosition.Size.Height - animation.Size.Height) / 2.0);
                animation.Velocity = defaultPosition.Velocity;
                Animations.Add(animation);
            }
        }

        public void DeleteAnimation(Animation obj)
        {
            lock (Animations)
            {
                obj.Cleanup();
                Animations.Remove(obj);
            }
        }

        public Star CreateStar(double x, double y)
        {
            lock (Stars)
            {
                Star obj = new Star(_game)
                {
                    X = x,
                    Y = y
                };
                Stars.Add(obj);
                return obj;
            }
        }

        public Star CreateStar()
        {
            lock (Stars)
            {
                Star obj = new Star(_game);
                Stars.Add(obj);
                return obj;
            }
        }

        public void DeleteStar(Star obj)
        {
            lock (Stars)
            {
                obj.Cleanup();
                Stars.Remove(obj);
            }
        }

        public Enemy CreateEnemy()
        {
            lock (Enemies)
            {
                Enemy obj = new Enemy(_game);
                Enemies.Add(obj);
                return obj;
            }
        }

        public void DeleteEnemy(Enemy obj)
        {
            lock (Enemies)
            {
                obj.Cleanup();
                Enemies.Remove(obj);
            }
        }

        public Bullet CreateBullet(BaseObject firedFrom)
        {
            lock (Bullets)
            {
                Bullet obj = new Bullet(_game, firedFrom);
                Bullets.Add(obj);
                return obj;
            }
        }

        public void DeleteBullet(Bullet obj)
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
            DebugText.Render(dc);
            PlayerStatsText.Render(dc);
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
                    if (obj.Bounds.IntersectsWith(_game.CurrentView))
                    {
                        obj.Render(dc);
                    }
                    else
                    {
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
