using NTDLS.Determinet;
using Si.Library.Types.Geometry;

namespace Si.GameEngine.AI
{
    public interface IAIController
    {
        public DniNeuralNetwork Network { get; set; }
        public void ApplyIntelligence(SiPoint displacementVector);
    }
}
