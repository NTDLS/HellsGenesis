using Si.GameEngine.Engine;
using Si.GameEngine.Weapons.Munitions;
using Si.Shared.Types;
using Si.Shared.Types.Geometry;
using System.Drawing;
using static Si.Shared.SiConstants;

namespace Si.GameEngine.Sprites
{
    public class SpriteAttachment : _SpriteShipBase
    {
        public bool TakesDamage { get; set; }

        public SpriteAttachment(EngineCore gameCore, string imagePath, Size? size = null)
            : base(gameCore)
        {
            Initialize(imagePath, size);

            X = 0;
            Y = 0;
            Velocity = new SiVelocity();
        }

        public override bool TryMunitionHit(SiPoint displacementVector, MunitionBase munition, SiPoint hitTestPosition)
        {
            if (munition.FiredFromType == SiFiredFromType.Player)
            {
                if (Intersects(hitTestPosition))
                {
                    Hit(munition);
                    if (HullHealth <= 0)
                    {
                        Explode();
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
