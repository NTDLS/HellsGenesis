using Si.Engine;
using Si.GameEngine.Sprite.PowerUp._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;

namespace Si.GameEngine.Sprite.PowerUp
{
    internal class SpritePowerupBoost : SpritePowerupBase
    {
        private readonly int imageCount = 3;

        public SpritePowerupBoost(EngineCore engine)
            : base(engine)
        {
            PowerupAmount = 100;

            int multiplier = SiRandom.Between(0, imageCount - 1);
            SetImage(@$"Graphics\Powerup\Boost\{multiplier}.png");
            PowerupAmount *= multiplier + 1;
        }

        public override void ApplyIntelligence(float epoch, SiPoint displacementVector)
        {
            if (Intersects(_engine.Player.Sprite))
            {
                _engine.Player.Sprite.Velocity.AvailableBoost += PowerupAmount;
                Explode();
            }
            else if (AgeInMilliseconds > TimeToLive)
            {
                Explode();
            }
        }
    }
}
