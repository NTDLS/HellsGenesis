using HellsGenesis.Weapons.Munitions;
using NebulaSiege.Engine;
using NebulaSiege.Engine.Types;
using NebulaSiege.Engine.Types.Geometry;
using System.Drawing;

namespace NebulaSiege.Sprites
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

        public override bool TryMunitionHit(NsPoint displacementVector, _MunitionBase munition, NsPoint hitTestPosition)
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
