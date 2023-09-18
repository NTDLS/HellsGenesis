using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Utility;
using System.Drawing;
using System.IO;

namespace HG.Sprites.PowerUp
{
    internal class SpritePowerUpRepair : SpritePowerUpBase
    {
        private const string _assetPath = @"Graphics\PowerUp\Repair\";
        private readonly int imageCount = 3;
        private readonly int selectedImageIndex = 0;

        private readonly int _powerUpAmount = 100;

        public SpritePowerUpRepair(EngineCore core)
            : base(core)
        {
            selectedImageIndex = HgRandom.Random.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"), new Size(32, 32));
            _powerUpAmount *= selectedImageIndex + 1;
        }

        public override void ApplyIntelligence(HgPoint displacementVector)
        {
            if (Intersects(_core.Player.Sprite))
            {
                _core.Player.Sprite.AddHullHealth(_powerUpAmount);
                Explode();
            }
            else if (AgeInMiliseconds > TimeToLive)
            {
                Explode();
            }
        }
    }
}
