using AI2D.Types;
using Determinet;

namespace AI2D.AI
{
    public interface IAIController
    {
        public DniNeuralNetwork Network { get; set; }
        public void ApplyIntelligence(Point<double> frameAppliedOffset);
    }
}
