using Si.Shared.Payload.SpriteActions;
using Si.Shared.Types.Geometry;

namespace Si.GameEngine.Sprites._Superclass
{
    internal interface ISpriteDrone
    {
        public void ApplyAbsoluteMultiplayVector(SiSpriteActionVector vector);
        public void ApplyMotion(SiPoint displacementVector);
        public void Hit(int damage);
        public void Explode();
        public void QueueForDelete();
        public bool FireDroneWeapon(string weaponTypeName);
    }
}
