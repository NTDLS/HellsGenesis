using Si.Engine.Sprites.Powerup._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprites.Powerup
{
    internal class SpritePowerupAmmo : SpritePowerupBase
    {
        private readonly int imageCount = 3;

        public SpritePowerupAmmo(EngineCore engine)
            : base(engine)
        {
            PowerupAmount = 100;

            int multiplier = SiRandom.Between(0, imageCount - 1);
            SetImage(@$"Graphics\Powerup\Ammo\{multiplier}.png");
            PowerupAmount *= multiplier + 1;
        }

        public override void ApplyIntelligence(float epoch, SiPoint displacementVector)
        {
            if (Intersects(_engine.Player.Sprite))
            {
                _engine.Player.Sprite.PrimaryWeapon.RoundQuantity += PowerupAmount;
                if (_engine.Player.Sprite.SelectedSecondaryWeapon != null)
                {
                    _engine.Player.Sprite.SelectedSecondaryWeapon.RoundQuantity += PowerupAmount;
                }
                Explode();
            }
            else if (AgeInMilliseconds > TimeToLive)
            {
                Explode();
            }
        }
    }
}
