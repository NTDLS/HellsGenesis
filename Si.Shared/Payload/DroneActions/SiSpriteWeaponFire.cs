namespace Si.Shared.Payload.DroneActions
{
    public class SiSpriteWeaponFire : SiSpriteAction
    {
        public string WeaponTypeName { get; set; } = string.Empty;

        public SiSpriteWeaponFire(Guid multiplayUID)
            : base(multiplayUID)
        {
        }
    }
}
