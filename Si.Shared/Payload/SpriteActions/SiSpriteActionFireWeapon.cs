namespace Si.Shared.Payload.SpriteActions
{
    public class SiSpriteActionFireWeapon : SiSpriteAction
    {
        public string WeaponTypeName { get; set; } = string.Empty;

        public SiSpriteActionFireWeapon(Guid multiplayUID)
            : base(multiplayUID)
        {
        }
    }
}
