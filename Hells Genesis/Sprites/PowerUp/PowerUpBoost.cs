using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Sprites.PowerUp.BaseClasses;
using HG.Utility;
using System.Drawing;
using System.IO;

namespace HG.Sprites.PowerUp
{
    internal class PowerUpBoost : PowerUpBase
    {
        private const string _assetPath = @"Graphics\PowerUp\Boost\";
        private readonly int imageCount = 3;
        private readonly int selectedImageIndex = 0;

        private readonly int _powerUpAmount = 100;

        public PowerUpBoost(EngineCore core)
            : base(core)
        {
            selectedImageIndex = HgRandom.Random.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"), new Size(32, 32));
            _powerUpAmount *= selectedImageIndex + 1;
        }

        public override void ApplyIntelligence(HgPoint displacementVector)
        {
            if (Intersects(_core.Player.Actor))
            {
                _core.Player.Actor.Velocity.AvailableBoost += _powerUpAmount;
                Explode();
            }
            else if (AgeInMiliseconds > TimeToLive)
            {
                Explode();
            }
        }
    }
}
