using System;

namespace Si.Shared.GameEngine.Loudouts
{
    public class ShipLoadoutWeapon
    {
        public string Type { get; set; } = string.Empty;
        public int MunitionCount { get; set; }

        public ShipLoadoutWeapon() { }

        public ShipLoadoutWeapon(Type type, int munitionCount)
        {
            Type = type.Name;
            MunitionCount = munitionCount;
        }
    }
}
