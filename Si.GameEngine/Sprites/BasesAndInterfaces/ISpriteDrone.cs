using Si.Shared.Payload.DroneActions;
using Si.Shared.Types.Geometry;

namespace Si.Sprites.BasesAndInterfaces
{
    internal interface ISpriteDrone
    {
        public void ApplyAbsoluteMultiplayVector(SiDroneActionVector vector);
        public void ApplyMotion(SiPoint displacementVector);
        public void Hit(int damage);
        public void Explode();
        public bool FireDroneWeapon(string weaponTypeName);
    }
}
