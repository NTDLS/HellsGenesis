using Si.GameEngine.Sprite.SupportingClasses;
using System.Drawing;

namespace Si.Engine.Sprite.SupportingClasses
{
    /// <summary>
    /// Holds information about a pair of overlapping sprites.
    /// 
    /// An object that contains both the location (position) and the velocity (vector indicating both
    /// speed and direction) of another object is commonly referred to as a "Kinematic" object.
    /// </summary>
    public struct OverlappingKinematicBodyPair
    {
        /// <summary>
        /// Creates a unique string key for a pair of sprites regardless of their order.
        /// </summary>
        /// <param name="uid1"></param>
        /// <param name="uid2"></param>
        /// <returns></returns>
        public static string MakeKey(uint uid1, uint uid2)
            => uid1 > uid2 ? $"{uid1}:{uid2}" : $"{uid2}:{uid1}";

        /// <summary>
        /// The key that identifies the collision pair.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// The predicted location, velocity, bounds and direction of the first sprite in the pair.
        /// </summary>
        public PredictedKinematicBody Body1 { get; private set; }

        /// <summary>
        /// The predicted location, velocity, bounds and direction of the second sprite in the pair.
        /// </summary>
        public PredictedKinematicBody Body2 { get; private set; }

        /// <summary>
        /// The overlapping rectangle of the two sprites. This is mostly for concept - I don't know what to do with it yet.
        /// </summary>
        public RectangleF OverlapRectangle { get; set; }

        /// <summary>
        /// The overlapping polygon of the two sprites. This is mostly for concept - I don't know what to do with it yet.
        /// </summary>
        public PointF[] OverlapPolygon { get; set; }

        public OverlappingKinematicBodyPair(string key, PredictedKinematicBody sprite1, PredictedKinematicBody sprite2)
        {
            Key = key;
            Body1 = sprite1;
            Body2 = sprite2;
        }
    }
}
