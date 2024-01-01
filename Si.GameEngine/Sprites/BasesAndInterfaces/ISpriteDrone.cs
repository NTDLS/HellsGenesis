using Si.Shared.Messages.Notify;
using Si.Shared.Types.Geometry;

namespace Si.Sprites.BasesAndInterfaces
{
    internal interface ISpriteDrone
    {
        public void ApplyMultiplayVector(SiSpriteVector vector);
        public void ApplyMotion(SiPoint displacementVector);
        //public void ApplyIntelligence(SiPoint displacementVector);
    }
}
