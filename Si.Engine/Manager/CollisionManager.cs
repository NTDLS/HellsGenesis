using Si.Engine.Sprite._Superclass;
using Si.GameEngine.Sprite.SupportingClasses;
using System.Collections.Generic;
using System.Drawing;

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
        public Dictionary<string, Collision> Detected { get; private set; } = new();

        public PredictedSpriteRegion[] Colliadbles { get; private set; }

        /// <summary>
        /// Holds information about a collision event.
        /// </summary>
        public struct Collision
        {
            /// <summary>
            /// The key that identifies the collision pair.
            /// </summary>
            public string Key { get; private set; }

            public PredictedSpriteRegion Object1 { get; private set; }
            public PredictedSpriteRegion Object2 { get; private set; }

            /// <summary>
            /// The overlapping rectangle of the two sprites. This is mostly for concept - I dont know what to do with it yet.
            /// </summary>
            public RectangleF OverlapRectangle { get; set; }

            /// <summary>
            /// The overlapping polygon of the two sprites. This is mostly for concept - I dont know what to do with it yet.
            /// </summary>
            public PointF[] OverlapPolygon { get; set; }

            public Collision(string key, PredictedSpriteRegion sprite1, PredictedSpriteRegion sprite2)
            {
                Key = key;
                Object1 = sprite1;
                Object2 = sprite2;
            }
        }

        public CollisionManager(EngineCore engine)
        {
            _engine = engine;
        }

        /// <summary>
        /// Creates a unique string key for a pair of sprites.
        /// </summary>
        /// <param name="uid1"></param>
        /// <param name="uid2"></param>
        /// <returns></returns>
        public static string MakeCollisionKey(uint uid1, uint uid2)
            => uid1 > uid2 ? $"{uid1}:{uid2}" : $"{uid2}:{uid1}";

        public Collision Add(PredictedSpriteRegion object1, PredictedSpriteRegion object2)
        {
            var key = MakeCollisionKey(object1.Sprite.UID, object2.Sprite.UID);

            var collision = new Collision(key, object1, object2)
            {
                //We are just adding these here for demonstration purposes. This is probably over the top
                // and we DEFINITELY do not need GetIntersectionBoundingBox() AND GetIntersectedPolygon().
                //
                // Q: Also note that this is just the collision for predicted1→predicted2, which I am thinkning might be different??
                // A: I tested it, they are definitely different.
                //https://github.com/NTDLS/StrikeforceInfinite/wiki/Collision-Detection-Issues
                OverlapRectangle = object1.GetIntersectionBoundingBox(object2),
                OverlapPolygon = object1.GetIntersectedPolygon(object2)
            };

            Detected.Add(key, collision);
            return collision;
        }

        public void Reset(float epoch)
        {
            Colliadbles = _engine.Sprites.VisibleColliadblePredictiveMove(epoch);
            Detected.Clear();
        }

        public bool IsAlreadyHandled(SpriteInteractiveBase sprite1, SpriteInteractiveBase sprite2)
            => Detected.ContainsKey(MakeCollisionKey(sprite1.UID, sprite2.UID));

        public bool IsAlreadyHandled(PredictedSpriteRegion object1, PredictedSpriteRegion object2)
            => Detected.ContainsKey(MakeCollisionKey(object1.Sprite.UID, object2.Sprite.UID));
    }
}
