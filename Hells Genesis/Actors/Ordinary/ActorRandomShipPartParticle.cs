﻿using HG.Actors.BaseClasses;
using HG.Engine;
using HG.Types.Geometry;
using HG.Utility;
using System.IO;

namespace HG.Actors.Ordinary
{
    internal class ActorRandomShipPartParticle : ActorParticleBase
    {
        private const string _assetPath = @"Graphics\Fragments";
        private readonly int _imageCount = 1488;
        private readonly int selectedImageIndex = 0;


        private double _rotationSpeed;
        private HgRelativeDirection _rotationDirection;
        private HgAngle _travelAngle = new();

        public ActorRandomShipPartParticle(Core core, double x, double y)
            : base(core)
        {
            selectedImageIndex = HgRandom.Random.Next(0, 1000) % _imageCount;

            Initialize(Path.Combine(_assetPath, $"{selectedImageIndex}.png"));

            X = x;
            Y = y;

            _rotationSpeed = HgRandom.RandomNumber(1, 100) / 20.0;
            _rotationDirection = HgRandom.FlipCoin() ? HgRelativeDirection.Left : HgRelativeDirection.Right;
            _travelAngle.Degrees = HgRandom.RandomNumber(0, 360);

            Velocity.ThrottlePercentage = 100;
            Velocity.MaxSpeed = HgRandom.RandomNumber(1.0, 4.0);

            _core = core;
        }

        public override void ApplyMotion(HgPoint displacementVector)
        {
            X += _travelAngle.X * (Velocity.MaxSpeed * Velocity.ThrottlePercentage) - displacementVector.X;
            Y += _travelAngle.Y * (Velocity.MaxSpeed * Velocity.ThrottlePercentage) - displacementVector.Y;

            if (_rotationDirection == HgRelativeDirection.Right)
            {
                this.Velocity.Angle.Degrees += _rotationSpeed;
            }
            else if (_rotationDirection == HgRelativeDirection.Left)
            {
                this.Velocity.Angle.Degrees -= _rotationSpeed;
            }

            if (_core.Display.TotalScreenBounds.IntersectsWith(Bounds) == false) //Remove off-screen stars.
            {
                QueueForDelete();
            }
        }
    }
}
