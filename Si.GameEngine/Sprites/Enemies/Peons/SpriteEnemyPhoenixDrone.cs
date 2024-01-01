﻿using Si.GameEngine.Engine;
using Si.Shared.Messages.Notify;
using Si.Sprites.BasesAndInterfaces;

namespace Si.GameEngine.Sprites.Enemies.Peons
{
    internal class SpriteEnemyPhoenixDrone : SpriteEnemyPhoenix, ISpriteDrone
    {

        public SpriteEnemyPhoenixDrone(EngineCore gameCore)
            : base(gameCore)
        {
        }

        public void ApplyMultiplayVector(SiSpriteVector vector)
        {
            this.Visable = true;
            MultiplayX = vector.X;
            MultiplayY = vector.Y;

            //MultiplayX = _gameCore.Player.Sprite.X + vector.X;
            //MultiplayY = _gameCore.Player.Sprite.Y + vector.Y;
            Velocity.Angle.Degrees = vector.AngleDegrees;
            ThrustAnimation.Visable = vector.ThrottlePercentage > 0;
            BoostAnimation.Visable = vector.BoostPercentage > 0;
        }
    }
}