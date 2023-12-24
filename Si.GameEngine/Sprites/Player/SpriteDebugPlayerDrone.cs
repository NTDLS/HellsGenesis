using Si.GameEngine.Engine;
using Si.Shared.Messages.Notify;
using Si.Sprites.BasesAndInterfaces;

namespace Si.GameEngine.Sprites.Player
{
    internal class SpriteDebugPlayerDrone : SpriteDebugPlayer, ISpriteDrone
    {
        public SpriteDebugPlayerDrone(EngineCore gameCore)
            : base(gameCore)
        {
        }

        public void ApplyMultiplayVector(SiSpriteVector vector)
        {
            X = vector.X;
            Y = vector.Y;
            Velocity.Angle.Degrees = vector.AngleDegrees;
            ThrustAnimation.Visable = vector.ThrottlePercentage > 0;
            BoostAnimation.Visable = vector.BoostPercentage > 0;
        }
    }
}
