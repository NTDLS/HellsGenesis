using Si.Library.Types.Geometry;

namespace Si.GameEngine.AI.Logistics._Superclass
{
    public interface IIAController
    {
        public void ApplyIntelligence(double epoch, SiPoint displacementVector);
    }
}
