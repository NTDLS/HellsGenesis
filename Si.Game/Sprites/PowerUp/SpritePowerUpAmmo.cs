using Si.Shared;
using Si.Game.Engine;
using Si.Game.Sprites.PowerUp.BasesAndInterfaces;
using Si.Game.Utility;
using System.Drawing;
using System.IO;
using Si.Shared.Types.Geometry;

namespace Si.Game.Sprites.PowerUp
{
    internal class SpritePowerUpAmmo : SpritePowerUpBase
    {
        private const string _assetPath = @"Graphics\PowerUp\Ammo\";
        private readonly int imageCount = 3;
        private readonly int selectedImageIndex = 0;

        private readonly int _powerUpAmount = 100;

        public SpritePowerUpAmmo(EngineCore gameCore)
            : base(gameCore)
        {
            selectedImageIndex = SiRandom.Generator.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"), new Size(32, 32));
            _powerUpAmount *= selectedImageIndex + 1;
        }

        public override void ApplyIntelligence(SiPoint displacementVector)
        {
            if (Intersects(_gameCore.Player.Sprite))
            {
                _gameCore.Player.Sprite.PrimaryWeapon.RoundQuantity += _powerUpAmount;
                if (_gameCore.Player.Sprite.SelectedSecondaryWeapon != null)
                {
                    _gameCore.Player.Sprite.SelectedSecondaryWeapon.RoundQuantity += _powerUpAmount;
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
