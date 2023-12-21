using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Shared.Messages.Notify;
using StrikeforceInfinity.Sprites.BasesAndInterfaces;

namespace StrikeforceInfinity.Game.Sprites.Player
{
    internal class SpriteStarfighterPlayerDrone : SpriteStarfighterPlayer, ISpriteDrone
    {
        public SpriteStarfighterPlayerDrone(EngineCore gameCore)
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
