using Si.Engine;
using Si.Engine.Sprite;
using Si.Engine.Sprite.Weapon;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;

namespace Si.GameEngine.Sprite.Enemy.Starbase.Garrison
{
    internal class SpriteEnemyStarbaseGarrisonTurret : SpriteAttachment
    {
        public bool FireToggler { get; set; }

        public SpriteEnemyStarbaseGarrisonTurret(EngineCore engine)
            : base(engine, $@"Sprites\Enemy\Starbase\Garrison\Turret.png")
        {
        }

        public override void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            if (DistanceTo(_engine.Player.Sprite) < 1000)
            {
                //Rotate the turret toward the player.
                var deltaAngltToPlayer = this.HeadingAngleToInSignedDegrees(_engine.Player.Sprite);
                if (deltaAngltToPlayer < 1)
                {
                    Orientation.Degrees -= 0.25f;
                }
                else if (deltaAngltToPlayer > 1)
                {
                    Orientation.Degrees += 0.25f;
                }

                if (deltaAngltToPlayer.IsBetween(-10, 10))
                {
                    if (FireToggler)
                    {
                        var pointRight = Orientation.RotatedBy(90.ToRadians()) * new SiVector(21, 21);
                        FireToggler = !FireWeapon<WeaponLancer>(Location + pointRight);
                    }
                    else
                    {
                        var pointLeft = Orientation.RotatedBy(-90.ToRadians()) * new SiVector(21, 21);
                        FireToggler = FireWeapon<WeaponLancer>(Location + pointLeft);
                    }
                }
            }

            base.ApplyIntelligence(epoch, displacementVector);
        }
    }
}
