﻿using Si.Engine;
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
            : base(engine)
        {
            SetImageAndLoadMetadata($@"Sprites\Enemy\Starbase\Garrison\Turret.png");

            SetHullHealth(10);
        }

        public override void ApplyMotion(float epoch, SiVector displacementVector)
        {
            if (IsDeadOrExploded) return;

            // Since the turret.BaseLocation is relative to the top-left corner of the base sprite, we need
            // to get the position relative to the center of the base sprite image so that we can rotate around that.
            var turretOffset = LocationRelativeToOwner - (OwnerSprite.Size / 2.0f);

            // Apply the rotated offsets to get the new turret location relative to the base sprite center.
            Location = OwnerSprite.Location + turretOffset.RotatedBy(OwnerSprite.Orientation.RadiansSigned);

            if (DistanceTo(_engine.Player.Sprite) < 1000)
            {
                //Rotate the turret toward the player.
                var deltaAngltToPlayer = this.HeadingAngleToInSignedDegrees(_engine.Player.Sprite);
                if (deltaAngltToPlayer < 1)
                {
                    Orientation.DegreesUnsigned -= 0.25f;
                }
                else if (deltaAngltToPlayer > 1)
                {
                    Orientation.DegreesUnsigned += 0.25f;
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
            base.ApplyMotion(epoch, displacementVector);
        }
    }
}
