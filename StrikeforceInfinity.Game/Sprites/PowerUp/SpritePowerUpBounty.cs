using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Sprites.PowerUp.BasesAndInterfaces;
using StrikeforceInfinity.Game.Utility;
using System.Drawing;
using System.IO;

namespace StrikeforceInfinity.Game.Sprites.PowerUp
{
    internal class SpritePowerUpBounty : SpritePowerUpBase
    {
        private const string _assetPath = @"Graphics\PowerUp\Bounty\";
        private readonly int imageCount = 3;
        private readonly int selectedImageIndex = 0;

        private readonly int _powerUpAmount = 100;

        public SpritePowerUpBounty(EngineCore gameCore)
            : base(gameCore)
        {
            selectedImageIndex = HgRandom.Generator.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"), new Size(32, 32));
            _powerUpAmount *= selectedImageIndex + 1;
        }

        public override void ApplyIntelligence(SiPoint displacementVector)
        {
            if (Intersects(_gameCore.Player.Sprite))
            {
                _gameCore.Player.Sprite.Bounty += _powerUpAmount;

                Explode();
            }
            else if (AgeInMiliseconds > TimeToLive)
            {
                Explode();
            }
        }
    }
}
