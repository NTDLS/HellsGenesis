using Si.GameEngine.Core;
using Si.GameEngine.Sprites.Powerup._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;

namespace Si.GameEngine.Sprites.Powerup
{
    internal class SpritePowerupBounty : SpritePowerupBase
    {
        private readonly int imageCount = 3;

        public SpritePowerupBounty(GameEngineCore gameEngine)
            : base(gameEngine)
        {
            PowerupAmount = 100;

            int multiplier = SiRandom.Between(0, imageCount - 1);
            SetImage(@$"Graphics\Powerup\Bounty\{multiplier}.png");
            PowerupAmount *= multiplier + 1;
        }

        public override void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            if (Intersects(_gameEngine.Player.Sprite))
            {
                _gameEngine.Player.Sprite.Bounty += PowerupAmount;

                Explode();
            }
            else if (AgeInMilliseconds > TimeToLive)
            {
                Explode();
            }
        }
    }
}
