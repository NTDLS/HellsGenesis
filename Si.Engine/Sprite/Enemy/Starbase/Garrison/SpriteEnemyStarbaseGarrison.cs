using Si.Engine;
using Si.Engine.Sprite.Enemy.Starbase._Superclass;
using Si.Library.Mathematics;

namespace Si.GameEngine.Sprite.Enemy.Starbase.Garrison
{
    internal class SpriteEnemyStarbaseGarrison(EngineCore engine)
        : SpriteEnemyStarbase(engine, @"Sprites\Enemy\Starbase\Garrison\Hull.png")
    {
        public override void ApplyMotion(float epoch, SiVector displacementVector)
        {
            Orientation.Degrees += 0.005f;
            base.ApplyMotion(epoch, displacementVector);
        }
    }
}

