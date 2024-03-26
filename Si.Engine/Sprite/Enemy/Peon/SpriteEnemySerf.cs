using Si.Engine.Sprite.Enemy.Peon._Superclass;

namespace Si.Engine.Sprite.Enemy.Peon
{
    internal class SpriteEnemySerf : SpriteEnemyPeonBase
    {
        public SpriteEnemySerf(EngineCore engine)
            : base(engine)
        {
            SetImageAndLoadMetadata(@"Sprites\Enemy\Peon\Serf\Hull.png");
        }
    }
}
