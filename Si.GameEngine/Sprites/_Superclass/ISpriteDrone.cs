using Si.Library.Payload.SpriteActions;
using Si.Library.Types.Geometry;

namespace Si.GameEngine.Sprites._Superclass
{
    internal interface ISpriteDrone
    {
        public void ApplyAbsoluteMultiplayVector(SiSpriteActionVector vector);
        public void ApplyMotion(double epoch, SiPoint displacementVector);
        public void Hit(int damage);
        public void Explode();
        public void QueueForDelete();
        public bool FireDroneWeapon(string weaponTypeName);
    }
}
