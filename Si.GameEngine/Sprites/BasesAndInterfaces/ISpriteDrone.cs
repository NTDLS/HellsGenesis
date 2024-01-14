using Si.Shared.Payload.SpriteActions;
using Si.Shared.Types.Geometry;

namespace Si.Sprites.BasesAndInterfaces
{
    internal interface ISpriteDrone
    {
        public void ApplyAbsoluteMultiplayVector(SiSpriteActionVector vector);
        public void ApplyMotion(SiReadonlyPoint displacementVector);
        public void Hit(int damage);
        public void Explode();
        public void QueueForDelete();
        public bool FireDroneWeapon(string weaponTypeName);
    }
}
