using Si.Game.Engine;
using Si.Game.Engine.Types.Geometry;
using Si.Game.Sprites.PowerUp.BasesAndInterfaces;
using Si.Game.Utility;
using System.Drawing;
using System.IO;

namespace Si.Game.Sprites.PowerUp
{
    internal class SpritePowerUpBoost : SpritePowerUpBase
    {
        private const string _assetPath = @"Graphics\PowerUp\Boost\";
        private readonly int imageCount = 3;
        private readonly int selectedImageIndex = 0;

        private readonly int _powerUpAmount = 100;

        public SpritePowerUpBoost(EngineCore gameCore)
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
                _gameCore.Player.Sprite.Velocity.AvailableBoost += _powerUpAmount;
                Explode();
            }
            else if (AgeInMiliseconds > TimeToLive)
            {
                Explode();
            }
        }
    }
}
