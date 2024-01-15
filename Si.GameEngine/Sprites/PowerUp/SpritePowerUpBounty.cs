using Si.GameEngine.Core;
using Si.GameEngine.Sprites.Powerup._Superclass;
using Si.Shared;
using Si.Shared.Types.Geometry;
using System.Drawing;

namespace Si.GameEngine.Sprites.Powerup
{
    internal class SpritePowerupBounty : SpritePowerupBase
    {
        private readonly int imageCount = 3;

        public SpritePowerupBounty(Engine gameCore)
            : base(gameCore)
        {
            PowerupAmount = 100;

            int multiplier = SiRandom.Generator.Next(0, 1000) % imageCount;
            SetImage(@$"Graphics\Powerup\Bounty\{multiplier}.png", new Size(32, 32));
            PowerupAmount *= multiplier + 1;
        }

        public override void ApplyIntelligence(SiPoint displacementVector)
        {
            if (Intersects(_gameCore.Player.Sprite))
            {
                _gameCore.Player.Sprite.Bounty += PowerupAmount;

                Explode();
            }
            else if (AgeInMiliseconds > TimeToLive)
            {
                Explode();
            }
        }
    }
}
