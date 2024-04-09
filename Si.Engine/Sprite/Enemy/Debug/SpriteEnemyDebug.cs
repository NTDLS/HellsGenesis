using Si.Engine.Sprite.Enemy.Peon._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.Enemy.Debug
{
    /// <summary>
    /// Debugging enemy unit - a scary sight to see.
    /// </summary>
    internal class SpriteEnemyDebug : SpriteEnemyPeonBase
    {
        public SpriteEnemyDebug(EngineCore engine)
            : base(engine)
        {
            SetImageAndLoadMetadata(@"Sprites\Enemy\Debug\Hull.png");
            Throttle = 0;
        }

        public override void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            //var deltaAngle = _engine.Player.Sprite.DeltaAngleInSignedDegrees(this);
            //var unsigned = _engine.Player.Sprite.Location.AngleToInSignedRadians(Location);
            //System.Diagnostics.Debug.WriteLine($"DeltaAngle: {deltaAngle:n4}");

            base.ApplyIntelligence(epoch, displacementVector);
        }
    }
}

