using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using static Si.Shared.SiConstants;

namespace Si.GameEngine.Loudouts
{
    /// <summary>
    /// Contains a single instance of a player loadout.
    /// </summary>
    public class PlayerShipLoadout
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public SiPlayerClass Class { get; set; }
        [JsonIgnore]
        public int ImageIndex => (int)Class;
        [JsonIgnore]
        public string Name => Class.ToString();
        public string Description { get; set; }
        public double MaxSpeed { get; set; }
        public double MaxBoost { get; set; }
        public int HullHealth { get; set; }
        public int ShieldHealth { get; set; }

        public ShipLoadoutWeapon PrimaryWeapon { get; set; }
        public List<ShipLoadoutWeapon> SecondaryWeapons { get; set; } = new();

        public PlayerShipLoadout()
        {
        }

        public PlayerShipLoadout(SiPlayerClass shipClass)
        {
            Class = shipClass;
        }
    }
}
