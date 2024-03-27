using Si.Library.Mathematics;
using Si.Library.Mathematics.Geometry;

namespace Si.Library.Sprite
{
    /// <summary>
    /// All sprites (or their base classes) must inherit this interface.
    /// </summary>
    public interface ISprite
    {
        public SiPoint Location { get; set; }
        public SiTravelVector Travel { get; set; }
        public float RotationSpeed { get; set; }
        public SiAngle Direction { get; set; }
    }
}
