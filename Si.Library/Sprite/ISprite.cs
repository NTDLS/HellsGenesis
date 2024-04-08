using Si.Library.Mathematics.Geometry;

namespace Si.Library.Sprite
{
    /// <summary>
    /// All sprites (or their base classes) must inherit this interface.
    /// </summary>
    public interface ISprite
    {
        public SiVector Location { get; set; }
        public float RotationSpeed { get; set; }
        public SiVector PointingAngle { get; set; }
    }
}
