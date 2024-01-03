using Si.GameEngine.Engine;
using Si.Shared.Payload.DroneActions;
using Si.Sprites.BasesAndInterfaces;

namespace Si.GameEngine.Sprites.Player
{
    internal class SpriteReaverPlayerDrone : SpriteReaverPlayer, ISpriteDrone
    {
        public SpriteReaverPlayerDrone(EngineCore gameCore)
            : base(gameCore)
        {
        }

        public void ApplyMultiplayVector(SiSpriteVector vector)
        {
            MultiplayX = _gameCore.Player.Sprite.LocalX + vector.X;
            MultiplayY = _gameCore.Player.Sprite.LocalY + vector.Y;
            Velocity.Angle.Degrees = vector.AngleDegrees;
            ThrustAnimation.Visable = vector.ThrottlePercentage > 0;
            BoostAnimation.Visable = vector.BoostPercentage > 0;
        }
    }
}
