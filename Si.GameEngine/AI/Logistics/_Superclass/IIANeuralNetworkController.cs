using NTDLS.Determinet;

namespace Si.GameEngine.AI.Logistics._Superclass
{
    public interface IIANeuralNetworkController : IIAController
    {
        public DniNeuralNetwork Network { get; set; }
    }
}
