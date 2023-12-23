﻿using Si.Game.Engine;
using Si.Game.Sprites.PowerUp.BasesAndInterfaces;
using Si.Shared;
using Si.Shared.Types.Geometry;
using System.Drawing;
using System.IO;

namespace Si.Game.Sprites.PowerUp
{
    internal class SpritePowerUpSheild : SpritePowerUpBase
    {
        private const string _assetPath = @"Graphics\PowerUp\Sheild\";
        private readonly int imageCount = 3;
        private readonly int selectedImageIndex = 0;

        private readonly int _powerUpAmount = 100;

        public SpritePowerUpSheild(EngineCore gameCore)
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
                _gameCore.Player.Sprite.AddShieldHealth(_powerUpAmount);
                Explode();
            }
            else if (AgeInMiliseconds > TimeToLive)
            {
                Explode();
            }
        }
    }
}
