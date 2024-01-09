using Si.GameEngine.Engine;
using Si.GameEngine.Sprites.Powerup.BasesAndInterfaces;
using Si.Shared;
using Si.Shared.Types.Geometry;
using System.Drawing;

namespace Si.GameEngine.Sprites.Powerup
{
    internal class SpritePowerupAmmo : SpritePowerupBase
    {
        private readonly int imageCount = 3;

        public SpritePowerupAmmo(EngineCore gameCore)
            : base(gameCore)
        {
            PowerupAmount = 100;

            int multiplier = SiRandom.Generator.Next(0, 1000) % imageCount;
            SetImage(@$"Graphics\Powerup\Ammo\{multiplier}.png", new Size(32, 32));
            PowerupAmount *= multiplier + 1;
        }

        public override void ApplyIntelligence(SiPoint displacementVector)
        {
            if (Intersects(_gameCore.Player.Sprite))
            {
                _gameCore.Player.Sprite.PrimaryWeapon.RoundQuantity += PowerupAmount;
                if (_gameCore.Player.Sprite.SelectedSecondaryWeapon != null)
                {
                    _gameCore.Player.Sprite.SelectedSecondaryWeapon.RoundQuantity += PowerupAmount;
                }
                Explode();
            }
            else if (AgeInMiliseconds > TimeToLive)
            {
                Explode();
            }
        }
    }
}
