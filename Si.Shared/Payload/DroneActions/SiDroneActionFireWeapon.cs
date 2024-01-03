namespace Si.Shared.Payload.DroneActions
{
    public class SiDroneActionFireWeapon : SiDroneAction
    {
        public string WeaponTypeName { get; set; } = string.Empty;

        public SiDroneActionFireWeapon(Guid multiplayUID)
            : base(multiplayUID)
        {
        }
    }
}
