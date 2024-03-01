using Si.Library.Mathematics.Geometry;
using Si.Library.Payload.SpriteActions;

namespace Si.Library.Sprite
{
    public interface ISpriteDrone
    {
        public void ApplyAbsoluteMultiplayVector(SiSpriteActionVector vector);
        public void ApplyMotion(float epoch, SiPoint displacementVector);
        public void Hit(int damage);
        public void Explode();
        public void QueueForDelete();
        public bool FireDroneWeapon(string weaponTypeName);
    }
}
