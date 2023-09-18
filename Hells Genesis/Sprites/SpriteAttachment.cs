using HG.Engine;
using HG.Engine.Types;
using HG.Engine.Types.Geometry;
using HG.Weapons.Bullets;
using System.Drawing;

namespace HG.Sprites
{
    internal class SpriteAttachment : SpriteShipBase
    {
        public bool TakesDamage { get; set; }

        public SpriteAttachment(EngineCore core, string imagePath, Size? size = null)
            : base(core)
        {
            Initialize(imagePath, size);

            X = 0;
            Y = 0;
            Velocity = new HgVelocity();
        }

        public override bool TestHit(HgPoint displacementVector, BulletBase bullet, HgPoint hitTestPosition)
        {
            if (bullet.FiredFromType == HgFiredFromType.Player)
            {
                if (Intersects(hitTestPosition))
                {
                    if (Hit(bullet))
                    {
                        if (HullHealth <= 0)
                        {
                            Explode();
                        }
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
