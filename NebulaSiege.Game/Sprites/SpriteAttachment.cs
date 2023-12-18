using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Engine.Types;
using NebulaSiege.Game.Engine.Types.Geometry;
using NebulaSiege.Game.Weapons.Munitions;
using System.Drawing;

namespace NebulaSiege.Game.Sprites
{
    internal class SpriteAttachment : _SpriteShipBase
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

        public override bool TryMunitionHit(NsPoint displacementVector, MunitionBase munition, NsPoint hitTestPosition)
        {
            if (munition.FiredFromType == HgFiredFromType.Player)
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
