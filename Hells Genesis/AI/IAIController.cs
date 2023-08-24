using Determinet;
using HG.Types;

namespace HG.AI
{
    internal interface IAIController
    {
        public DniNeuralNetwork Network { get; set; }
        public void ApplyIntelligence(HgPoint<double> displacementVector);
    }
}
