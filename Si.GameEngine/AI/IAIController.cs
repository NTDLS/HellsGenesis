using NTDLS.Determinet;
using Si.Shared.Types.Geometry;

namespace Si.GameEngine.AI
{
    public interface IAIController
    {
        public DniNeuralNetwork Network { get; set; }
        public void ApplyIntelligence(SiPoint displacementVector);
    }
}
