using Si.GameEngine.Engine;
using Si.GameEngine.Sprites.Powerup.BasesAndInterfaces;
using Si.Shared;
using Si.Shared.Types.Geometry;
using System.Drawing;

namespace Si.GameEngine.Sprites.Powerup
{
    internal class SpritePowerupSheild : SpritePowerupBase
    {
        private readonly int imageCount = 3;

        public SpritePowerupSheild(EngineCore gameCore)
            : base(gameCore)
        {
            PowerupAmount = 100;

            int multiplier = SiRandom.Generator.Next(0, 1000) % imageCount;
            SetImage(@$"Graphics\Powerup\Powerup\{multiplier}.png", new Size(32, 32));
            PowerupAmount *= multiplier + 1;
        }

        public override void ApplyIntelligence(SiPoint displacementVector)
        {
            if (Intersects(_gameCore.Player.Sprite))
            {
                _gameCore.Player.Sprite.AddShieldHealth(PowerupAmount);
                Explode();
            }
            else if (AgeInMiliseconds > TimeToLive)
            {
                Explode();
            }
        }
    }
}
