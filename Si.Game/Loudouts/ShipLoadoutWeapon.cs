using System;

namespace Si.Game.Loudouts
{
    public class ShipLoadoutWeapon
    {
        public string Type { get; set; }
        public int MunitionCount { get; set; }

        public ShipLoadoutWeapon() { }

        public ShipLoadoutWeapon(Type type, int munitionCount)
        {
            Type = type.Name;
            MunitionCount = munitionCount;
        }
    }
}
