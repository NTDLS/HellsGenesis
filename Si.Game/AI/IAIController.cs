using NTDLS.Determinet;
using Si.Shared.Types.Geometry;

namespace Si.Game.AI
{
    internal interface IAIController
    {
        public DniNeuralNetwork Network { get; set; }
        public void ApplyIntelligence(SiPoint displacementVector);
    }
}
