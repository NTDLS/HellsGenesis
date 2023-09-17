using HG.Actors.PowerUp.BaseClasses;
using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Utility;
using System.Drawing;
using System.IO;

namespace HG.Actors.PowerUp
{
    internal class PowerUpRepair : PowerUpBase
    {
        private const string _assetPath = @"Graphics\PowerUp\Repair\";
        private readonly int imageCount = 3;
        private readonly int selectedImageIndex = 0;

        private readonly int _powerUpAmount = 100;

        public PowerUpRepair(EngineCore core)
            : base(core)
        {
            selectedImageIndex = HgRandom.Random.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"), new Size(32, 32));
            _powerUpAmount *= (selectedImageIndex + 1);
        }

        public override void ApplyIntelligence(HgPoint displacementVector)
        {
            if (Intersects(_core.Player.Actor))
            {
                _core.Player.Actor.AddHullHealth(_powerUpAmount);
                Explode();
            }
            else if (AgeInMiliseconds > TimeToLive)
            {
                Explode();
            }
        }
    }
}
