using Si.Engine;
using Si.Engine.Sprite;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;

namespace Si.GameEngine.Sprite.Enemy.Starbase.Garrison
{
    internal class SpriteEnemyBossDevastatorLeftJet : SpriteAttachment
    {
        public SpriteEnemyBossDevastatorLeftJet(EngineCore engine)
            : base(engine, $@"Sprites\Enemy\Boss\Devastator\Jet.Right.png")
        {
        }

        public override void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            Visable = !Owner.MovementVector.Magnitude().IsNearZero();
            base.ApplyIntelligence(epoch, displacementVector);
        }
    }
}
