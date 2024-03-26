using Si.Engine.Sprite._Superclass;
using Si.GameEngine.Sprite.SupportingClasses;
using System.Collections.Generic;

namespace Si.Engine.Manager
{
    public class CollisionManager
    {
        private readonly EngineCore _engine;
        private readonly HashSet<string> _collisions = new();

        public PredictedSpriteRegion[] Colliadbles { get; private set; }

        public CollisionManager(EngineCore engine)
        {
            _engine = engine;
        }

        public string MakeCollisionKey(SpriteBase sprite1, SpriteBase sprite2)
        {
            if (sprite1.UID > sprite2.UID)
            {
                return $"{sprite1.UID}:{sprite2.UID}";
            }
            return $"{sprite2.UID}:{sprite1.UID}";
        }

        public void Add(SpriteBase sprite1, SpriteBase sprite2)
        {
            _collisions.Add(MakeCollisionKey(sprite1, sprite2));
        }

        public void Reset(float epoch)
        {
            Colliadbles = _engine.Sprites.VisibleColliadblePredictiveMove(epoch);
            _collisions.Clear();
        }

        public bool IsAlreadyHandled(SpriteBase sprite1, SpriteBase sprite2)
        {
            return _collisions.Contains(MakeCollisionKey(sprite1, sprite2));
        }
    }
}
