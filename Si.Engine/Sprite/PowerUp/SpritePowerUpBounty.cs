using Si.Engine.Sprite.PowerUp._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.PowerUp
{
    internal class SpritePowerupBounty : SpritePowerupBase
    {
        private readonly int imageCount = 3;

        public SpritePowerupBounty(EngineCore engine)
            : base(engine)
        {
            PowerupAmount = 100;

            int multiplier = SiRandom.Between(0, imageCount - 1);
            SetImage(@$"Sprites\Powerup\Bounty\{multiplier}.png");
            PowerupAmount *= multiplier + 1;
        }

        public override void ApplyIntelligence(float epoch, SiPoint displacementVector)
        {
            if (IntersectsAABB(_engine.Player.Sprite))
            {
                Explode();
            }
            else if (AgeInMilliseconds > TimeToLive)
            {
                Explode();
            }
        }
    }
}
