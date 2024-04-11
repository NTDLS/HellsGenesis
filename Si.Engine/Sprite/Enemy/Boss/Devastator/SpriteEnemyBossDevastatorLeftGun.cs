using Si.Engine;
using Si.Engine.Sprite;
using Si.Engine.Sprite.Weapon;
using Si.Library.Mathematics;

namespace Si.GameEngine.Sprite.Enemy.Starbase.Garrison
{
    internal class SpriteEnemyBossDevastatorLeftGun : SpriteAttachment
    {
        public SpriteEnemyBossDevastatorLeftGun(EngineCore engine)
            : base(engine)
        {
            SetImageAndLoadMetadata($@"Sprites\Enemy\Boss\Devastator\Gun.Left.png");

            SetHullHealth(10);
        }

        public override void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            if (this.IsPointingAt(_engine.Player.Sprite, 10, 1000))
            {
                FireWeapon<WeaponLancer>();
            }

            base.ApplyIntelligence(epoch, displacementVector);
        }
    }
}
