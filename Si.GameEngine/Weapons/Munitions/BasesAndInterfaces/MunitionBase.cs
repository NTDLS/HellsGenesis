﻿using Si.GameEngine.Engine;
using Si.GameEngine.Sprites;
using Si.GameEngine.Sprites.Enemies.BasesAndInterfaces;
using Si.GameEngine.Sprites.Player.BasesAndInterfaces;
using Si.GameEngine.Weapons.BasesAndInterfaces;
using Si.Shared;
using Si.Shared.Types;
using Si.Shared.Types.Geometry;
using System;
using static Si.Shared.SiConstants;

namespace Si.GameEngine.Weapons.Munitions
{
    /// <summary>
    /// The munition base is the base for all bullets/projectiles/etc.
    /// </summary>
    public class MunitionBase : SpriteBase
    {
        public SiFiredFromType FiredFromType { get; private set; }
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
                var randomNumber = SiRandom.Between(0, weapon.AngleVarianceDegrees * 100.0) / 100.0;
                headingDegrees += (SiRandom.FlipCoin() ? 1 : -1) * randomNumber;
            }

            double initialSpeed = weapon.Speed;
            if (Weapon.SpeedVariancePercent > 0)
            {
                var randomNumber = SiRandom.Between(0, weapon.SpeedVariancePercent * 100.0) / 100.0;
                var variance = randomNumber * weapon.Speed;
                initialSpeed += (SiRandom.FlipCoin() ? 1 : -1) * variance;
            }

            var initialVelocity = new SiVelocity()
            {
                Angle = new SiAngle(headingDegrees),
                MaxSpeed = initialSpeed,
                ThrottlePercentage = 1.0
            };

            if (firedFrom.IsFixedPosition)
            {
                LocalLocation = (firedFrom.RenderLocation + _gameCore.Display.BackgroundOffset) + (xyOffset ?? SiPoint.Zero);
            }
            else
            {
                LocalLocation = firedFrom.RenderLocation + (xyOffset ?? SiPoint.Zero);
            }

            if (firedFrom is SpriteEnemyBase)
            {
                FiredFromType = SiFiredFromType.Enemy;
            }
            else if (firedFrom is SpritePlayerBase)
            {
                FiredFromType = SiFiredFromType.Player;
            }

            Velocity = initialVelocity;
        }

        public virtual void ApplyIntelligence(SiReadonlyPoint displacementVector)
        {
            if (AgeInMilliseconds > MilisecondsToLive)
            {
                Explode();
                return;
            }
        }

        public override void ApplyMotion(SiReadonlyPoint displacementVector)
        {
            if (LocalX < -_gameCore.Settings.MunitionSceneDistanceLimit
                || LocalX >= _gameCore.Display.TotalCanvasSize.Width + _gameCore.Settings.MunitionSceneDistanceLimit
                || LocalY < -_gameCore.Settings.MunitionSceneDistanceLimit
                || LocalY >= _gameCore.Display.TotalCanvasSize.Height + _gameCore.Settings.MunitionSceneDistanceLimit)
            {
                QueueForDelete();
                return;
            }

            LocalX += Velocity.Angle.X * (Velocity.MaxSpeed * Velocity.ThrottlePercentage);
            LocalY += Velocity.Angle.Y * (Velocity.MaxSpeed * Velocity.ThrottlePercentage);
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
