using AI2D.Types;
using Determinet;

namespace AI2D.AI
{
    internal interface IIntelligenceObject
    {
        public DniNeuralNetwork Network { get; set; }
        public void ApplyIntelligence(Point<double> frameAppliedOffset);
    }
}
