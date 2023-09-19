using NebulaSiege.Engine.Types.Geometry;
using NTDLS.Determinet;

namespace NebulaSiege.AI
{
    internal interface IAIController
    {
        public DniNeuralNetwork Network { get; set; }
        public void ApplyIntelligence(NsPoint displacementVector);
    }
}
