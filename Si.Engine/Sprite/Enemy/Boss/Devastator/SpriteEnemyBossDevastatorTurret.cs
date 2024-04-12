using Si.Engine.Sprite.Weapon;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Enemy.Boss.Devastator
{
    internal class SpriteEnemyBossDevastatorTurret : SpriteAttachment
    {
        public bool FireToggler { get; set; }

        public SpriteEnemyBossDevastatorTurret(EngineCore engine)
            : base(engine, $@"Sprites\Enemy\Boss\Devastator\Turret.png")
        {
        }

        public override void ApplyMotion(float epoch, SiVector displacementVector)
        {
            // Since the turret.BaseLocation is relative to the top-left corner of the base sprite, we need
            // to get the position relative to the center of the base sprite image so that we can rotate around that.
            var turretOffset = LocationRelativeToOwner - (Owner.Size / 2.0f);

            // Apply the rotated offsets to get the new turret location relative to the base sprite center.
            Location = Owner.Location + turretOffset.RotatedBy(Owner.Orientation.RadiansSigned);

            if (DistanceTo(_engine.Player.Sprite) < 1000)
            {
                //Rotate the turret toward the player.
                var deltaAngltToPlayer = this.HeadingAngleToInSignedDegrees(_engine.Player.Sprite);
                if (deltaAngltToPlayer < 1)
                {
                    Orientation.Degrees -= 1.5f;
                }
                else if (deltaAngltToPlayer > 1)
                {
                    Orientation.Degrees += 1.5f;
                }

                if (deltaAngltToPlayer.IsBetween(-10, 10))
                {
                    if (FireToggler)
                    {
                        var pointRight = Orientation.RotatedBy(90.ToRadians()) * new SiVector(10, 10);
                        FireToggler = !FireWeapon<WeaponThunderstrikeMissile>(Location + pointRight);
                    }
                    else
                    {
                        var pointLeft = Orientation.RotatedBy(-90.ToRadians()) * new SiVector(10, 10);
                        FireToggler = FireWeapon<WeaponThunderstrikeMissile>(Location + pointLeft);
                    }
                }
            }
        }
    }
}
