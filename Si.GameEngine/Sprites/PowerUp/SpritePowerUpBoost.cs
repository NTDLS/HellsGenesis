using Si.GameEngine.Core;
using Si.GameEngine.Sprites.Powerup._Superclass;
using Si.Library;
using Si.Library.Types.Geometry;
using System.Drawing;

namespace Si.GameEngine.Sprites.Powerup
{
    internal class SpritePowerupBoost : SpritePowerupBase
    {
        private readonly int imageCount = 3;

        public SpritePowerupBoost(GameEngineCore gameEngine)
            : base(gameEngine)
        {
            PowerupAmount = 100;

            int multiplier = SiRandom.Generator.Next(0, 1000) % imageCount;
            SetImage(@$"Graphics\Powerup\Boost\{multiplier}.png", new Size(32, 32));
            PowerupAmount *= multiplier + 1;
        }

        public override void ApplyIntelligence(SiPoint displacementVector)
        {
            if (Intersects(_gameEngine.Player.Sprite))
            {
                _gameEngine.Player.Sprite.Velocity.AvailableBoost += PowerupAmount;
                Explode();
            }
            else if (AgeInMiliseconds > TimeToLive)
            {
                Explode();
            }
        }
    }
}
