﻿using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Sprites;
using StrikeforceInfinity.Game.Sprites.Enemies.BaseClasses;
using StrikeforceInfinity.Game.Sprites.Player.BaseClasses;
using StrikeforceInfinity.Game.Utility;
using StrikeforceInfinity.Game.Weapons.BaseClasses;
using System;

namespace StrikeforceInfinity.Game.Weapons.Munitions
{
    /// <summary>
    /// The munition base is the base for all bullets/projectiles/etc.
    /// </summary>
    internal class MunitionBase : SpriteBase
    {
        public HgFiredFromType FiredFromType { get; private set; }
        public WeaponBase Weapon { get; private set; }
        public DateTime CreatedDate { get; private set; } = DateTime.UtcNow;
        public double MilisecondsToLive { get; set; } = 4000;
        public double AgeInMilliseconds => (DateTime.UtcNow - CreatedDate).TotalMilliseconds;

        public MunitionBase(EngineCore gameCore, WeaponBase weapon, SpriteBase firedFrom, string imagePath, SiPoint xyOffset = null)
            : base(gameCore)
        {
            Initialize(imagePath);

            Weapon = weapon;
            Velocity.ThrottlePercentage = 1.0;

            RadarDotSize = new SiPoint(1, 1);

            double headingDegrees = firedFrom.Velocity.Angle.Degrees;
            if (weapon.AngleVarianceDegrees > 0)
            {
                var randomNumber = HgRandom.Between(0, weapon.AngleVarianceDegrees * 100.0) / 100.0;
                headingDegrees += (HgRandom.FlipCoin() ? 1 : -1) * randomNumber;
            }

            double initialSpeed = weapon.Speed;
            if (Weapon.SpeedVariancePercent > 0)
            {
                var randomNumber = HgRandom.Between(0, weapon.SpeedVariancePercent * 100.0) / 100.0;
                var variance = randomNumber * weapon.Speed;
                initialSpeed += (HgRandom.FlipCoin() ? 1 : -1) * variance;
            }

            var initialVelocity = new HgVelocity()
            {
                Angle = new SiAngle(headingDegrees),
                MaxSpeed = initialSpeed,
                ThrottlePercentage = 1.0
            };

            Location = firedFrom.Location + (xyOffset ?? SiPoint.Zero);

            if (firedFrom is SpriteEnemyBase)
            {
                FiredFromType = HgFiredFromType.Enemy;
            }
            else if (firedFrom is SpritePlayerBase)
            {
                FiredFromType = HgFiredFromType.Player;
            }

            Velocity = initialVelocity;
        }

        public virtual void ApplyIntelligence(SiPoint displacementVector)
        {
            if (AgeInMilliseconds > MilisecondsToLive)
            {
                Explode();
                return;
            }
        }

        public override void ApplyMotion(SiPoint displacementVector)
        {
            if (X < -_gameCore.Settings.MunitionSceneDistanceLimit
                || X >= _gameCore.Display.TotalCanvasSize.Width + _gameCore.Settings.MunitionSceneDistanceLimit
                || Y < -_gameCore.Settings.MunitionSceneDistanceLimit
                || Y >= _gameCore.Display.TotalCanvasSize.Height + _gameCore.Settings.MunitionSceneDistanceLimit)
            {
                QueueForDelete();
                return;
            }

            X += Velocity.Angle.X * (Velocity.MaxSpeed * Velocity.ThrottlePercentage) - displacementVector.X;
            Y += Velocity.Angle.Y * (Velocity.MaxSpeed * Velocity.ThrottlePercentage) - displacementVector.Y;
        }

        public override void Explode()
        {
            if (Weapon != null && Weapon.ExplodesOnImpact)
            {
                HitExplosion();
            }
            QueueForDelete();
        }
    }
}
