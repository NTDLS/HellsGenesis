using Determinet;
using HG.Engine.Types.Geometry;

namespace HG.AI
{
    internal interface IAIController
    {
        public DniNeuralNetwork Network { get; set; }
        public void ApplyIntelligence(HgPoint displacementVector);
    }
}
