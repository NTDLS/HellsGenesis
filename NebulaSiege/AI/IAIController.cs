using NebulaSiege.Engine.Types.Geometry;
using NTDLS.Determinet;
using NTDLS.Determinet.Types;

namespace NebulaSiege.AI
{
    internal interface IAIController
    {
        public DniNeuralNetwork Network { get; set; }
        public void ApplyIntelligence(NsPoint displacementVector);
    }
}
