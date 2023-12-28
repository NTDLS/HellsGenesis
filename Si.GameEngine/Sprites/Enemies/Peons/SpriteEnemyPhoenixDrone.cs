using Si.GameEngine.Engine;
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
            var bgOffset = _gameCore.Display.BackgroundOffset;
            X = vector.X;// - bgOffset.X;
            Y = vector.Y;// - bgOffset.Y;
            Velocity.Angle.Degrees = vector.AngleDegrees;
            ThrustAnimation.Visable = vector.ThrottlePercentage > 0;
            BoostAnimation.Visable = vector.BoostPercentage > 0;
        }
    }
}