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

        public TextBlock DebugBlock { get; set; }
        public List<Boulder> Boulders { get; set; } = new List<Boulder>();
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

            Player.Velocity.Speed = 1;
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

        public Boulder CreateBoulder()
        {
            lock (Boulders)
            {
                Boulder result = new Boulder(_game);

                result.X = 400;
                result.Y = 400;

                result.MoveInDirectionOf(new PointD(0, 0));

                Boulders.Add(result);

                return result;
            }
        }

        public void DeleteBoulder(Boulder boulder)
        {
            lock (Boulders)
            {
                boulder.Cleanup();
                Boulders.Remove(boulder);
            }
        }

        public Bullet CreateBullet(BaseObject firedFrom)
        {
            lock (Bullets)
            {
                Bullet result = new Bullet(_game, firedFrom);
                Bullets.Add(result);
                return result;
            }
        }

        public void DeleteBullet(Bullet bullet)
        {
            lock (Bullets)
            {
                bullet.Cleanup();
                Bullets.Remove(bullet);
            }
        }

        #endregion

        #region Rendering.

        private void RenderText(Graphics dc)
        {
            DebugBlock.Render(dc);
        }

        void RenderBoulders(Graphics dc)
        {
            lock (Boulders)
            {
                foreach (var boulder in Boulders)
                {
                    boulder.Render(dc);
                }
            }
        }

        void RenderBullets(Graphics dc)
        {
            lock (Bullets)
            {
                foreach (var bullet in Bullets)
                {
                    bullet.Render(dc);
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
            RenderText(dc);
            RenderBoulders(dc);
            RenderBullets(dc);
            RenderPlayer(dc);
        }

        #endregion
    }
}
