using Si.Engine.Sprite.Enemy.Peon._Superclass;

namespace Si.Engine.Sprite.Enemy.Peon
{
    internal class SpriteEnemyMinnow : SpriteEnemyPeonBase
    {
        public SpriteEnemyMinnow(EngineCore engine)
            : base(engine)
        {
            SetImageAndLoadMetadata(@"Sprites\Enemy\Peon\Minnow\Hull.png");
        }
    }
}
