using Si.Engine.AI.Logistics;
using Si.Engine.Sprite.Enemy.Boss._Superclass;
using Si.Library;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Enemy.Boss.Devastator
{
    internal class SpriteEnemyBossDevastator : SpriteEnemyBossBase
    {
        public SpriteEnemyBossDevastator(EngineCore engine)
            : base(engine, @"Sprites\Enemy\Boss\Devastator\Hull.png")
        {
            Orientation.DegreesUnsigned = SiRandom.Between(0, 359);

            AddAIController(new AILogisticsHostileEngagement(_engine, this, _engine.Player.Sprite));

            SetCurrentAIController<AILogisticsHostileEngagement>();
        }

        public override void ApplyMotion(float epoch, SiVector displacementVector)
        {
            base.ApplyMotion(epoch, displacementVector);
        }
    }
}

