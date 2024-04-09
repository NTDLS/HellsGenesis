using Si.Engine.Sprite.PowerUp._Superclass;
using Si.Library;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.PowerUp
{
    internal class SpritePowerupShield : SpritePowerupBase
    {
        private readonly int imageCount = 3;

        public SpritePowerupShield(EngineCore engine)
            : base(engine)
        {
            PowerupAmount = 100;

            int multiplier = SiRandom.Between(0, imageCount - 1);
            SetImage(@$"Sprites\Powerup\Shield\{multiplier}.png");
            PowerupAmount *= multiplier + 1;
        }

        public override void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            if (IntersectsAABB(_engine.Player.Sprite))
            {
                _engine.Player.Sprite.AddShieldHealth(PowerupAmount);
                Explode();
            }
            else if (AgeInMilliseconds > TimeToLive)
            {
                Explode();
            }
        }
    }
}
