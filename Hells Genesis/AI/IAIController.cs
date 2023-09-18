using HG.Engine.Types.Geometry;
using NTDLS.Determinet;

namespace HG.AI
{
    internal interface IAIController
    {
        public DniNeuralNetwork Network { get; set; }
        public void ApplyIntelligence(HgPoint displacementVector);
    }
}
