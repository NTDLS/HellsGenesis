using Si.GameEngine.Sprites._Superclass;
using Si.Library.Mathematics.Geometry;
using System;

namespace Si.GameEngine.Sprites.Powerup._Superclass
{
    /// <summary>
    /// Represents a "power-up" that the player can pick up to gain some ability / stat-improvement.
    /// </summary>
    public class SpritePowerupBase : SpriteBase
    {
        /// <summary>
        /// The power up amount (number of boost points, shield points, repair, etc.).
        /// </summary>
        public int PowerupAmount { get; set; } = 1;

        /// <summary>
        /// Time until the powerup exploded on its own.
        /// </summary>
        public float TimeToLive { get; set; } = 30000;
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        public float AgeInMilliseconds
        {
            get
            {
                return (float)(DateTime.UtcNow - CreationTime).TotalMilliseconds;
            }
        }

        public SpritePowerupBase(GameEngineCore gameEngine)
            : base(gameEngine)
        {
            Initialize();

            RadarDotSize = new SiPoint(4, 4);
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }

        public override void Explode()
        {
            _gameEngine.Assets.GetAudio(@"Sounds\Powerup\PowerUp1.wav", 0.25f).Play();
            QueueForDelete();
        }

        public virtual void ApplyIntelligence(float epoch, SiPoint displacementVector)
        {
            if (Intersects(_gameEngine.Player.Sprite))
            {
                Explode();
            }
            else if (AgeInMilliseconds > TimeToLive)
            {
                Explode();
            }
        }
    }
}
