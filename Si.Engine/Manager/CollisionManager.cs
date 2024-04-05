using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.SupportingClasses;
using Si.GameEngine.Sprite.SupportingClasses;
using System.Collections.Generic;

namespace Si.Engine.Manager
{
    /// <summary>
    /// This class is used to keep track of which sprites are collidable and which collisions have been handled.
    /// I do no yet know how to really "handle" collisions, but I do know that if we detect a collision with
    ///     sprite "A" then we will also detect a seperate collision with sprite "B" and I wanted a way to determine
    ///     if that "collision" interaction had already been addressed with the first objects detection.
    ///     
    /// Each time the game loop begins, CollisionManager.Reset() is called to (1) clear all the detected collisions,
    ///     (2) create a list of collidable sprites, and (3) calculate the predicted location and rotation of those sprites.
    /// </summary>
    public class CollisionManager
    {
        private readonly EngineCore _engine;
        public Dictionary<string, OverlappingKinematicBodyPair> Detected { get; private set; } = new();

        public PredictedKinematicBody[] Colliadbles { get; private set; }

        public CollisionManager(EngineCore engine)
        {
            _engine = engine;
        }

        public OverlappingKinematicBodyPair Create(PredictedKinematicBody body1, PredictedKinematicBody body2)
        {
            var key = OverlappingKinematicBodyPair.MakeKey(body1.Sprite.UID, body2.Sprite.UID);

            var collisionPair = new OverlappingKinematicBodyPair(key, body1, body2)
            {
                //We are just adding these here for demonstration purposes. This is probably over the top
                // and we DEFINITELY do not need GetIntersectionBoundingBox() AND GetIntersectedPolygon().
                //
                // Q: Also note that this is just the collision for predicted1→predicted2, which I am thinkning might be different??
                // A: I tested it, they are definitely different.
                //https://github.com/NTDLS/StrikeforceInfinite/wiki/Collision-Detection-Issues
                OverlapRectangle = body1.GetIntersectionBoundingBox(body2),
                OverlapPolygon = body1.GetIntersectedPolygon(body2)
            };

            return collisionPair;
        }

        public void Record(OverlappingKinematicBodyPair pair)
        {
            Detected.Add(pair.Key, pair);
        }

        public OverlappingKinematicBodyPair CreateAndRecord(PredictedKinematicBody body1, PredictedKinematicBody body2)
        {
            var collisionPair = Create(body1, body2);
            Record(collisionPair);
            return collisionPair;
        }

        public void Reset(float epoch)
        {
            Colliadbles = _engine.Sprites.VisibleColliadblePredictiveMove(epoch);
            Detected.Clear();
        }

        public bool IsAlreadyHandled(SpriteInteractiveBase sprite1, SpriteInteractiveBase sprite2)
            => Detected.ContainsKey(OverlappingKinematicBodyPair.MakeKey(sprite1.UID, sprite2.UID));

        public bool IsAlreadyHandled(PredictedKinematicBody body1, PredictedKinematicBody body2)
            => Detected.ContainsKey(OverlappingKinematicBodyPair.MakeKey(body1.Sprite.UID, body2.Sprite.UID));
    }
}
