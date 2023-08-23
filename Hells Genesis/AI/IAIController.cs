using HG.Types;
using Determinet;

namespace HG.AI
{
    internal interface IAIController
    {
        public DniNeuralNetwork Network { get; set; }
        public void ApplyIntelligence(HGPoint<double> frameAppliedOffset);
    }
}
