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

        //public List<BaseObject> Objects { get; set; } = new List<BaseObject>();

        public TextBlock DebugBlock { get; set; }
        public List<Enemy> Enemies { get; set; } = new List<Enemy>();
        public List<Star> Stars { get; set; } = new List<Star>();
        public List<Bullet> Bullets { get; set; } = new List<Bullet>();
        public Player Player { get; set; }
        private Dictionary<string, AudioClip> _audioClips { get; set; } = new Dictionary<string, AudioClip>();

        public AudioClip ShipEngineRoar { get; set; }
        public AudioClip ShipEngineIdle { get; set; }
        public AudioClip BackgroundMusic { get; set; }

        public ActorAssets(Game game)
        {
            _game = game;

            BackgroundMusic = GetAudioClip(@"..\..\Assets\Sound\BackgroundMusic.wav", 0.25f);
            ShipEngineRoar = GetAudioClip(@"..\..\Assets\Sound\EngineRoar.wav", 1.0f, true);
            ShipEngineIdle = GetAudioClip(@"..\..\Assets\Sound\EngineIdle.wav", 0.5f, true);

            DebugBlock = new TextBlock(_game.Display, "ComicSans", 10, 5, 5);
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

            ShipEngineIdle.Play();
        }

        public void HidePlayer()
        {
            Player.Visable = false;
            ShipEngineIdle.Play();
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
            DebugBlock.Render(dc);
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

        public void RenderObjects(Graphics dc)
        {
            RenderStars(dc);
            RenderBullets(dc);
            RenderEnemies(dc);
            RenderPlayer(dc);
            RenderText(dc);
        }

        #endregion
    }
}
