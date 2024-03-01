using Si.Library.Mathematics.Geometry;

namespace Si.GameEngine.AI.Logistics._Superclass
{
    public interface IIAController
    {
        public void ApplyIntelligence(float epoch, SiPoint displacementVector);
    }
}
