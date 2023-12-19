using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StrikeforceInfinity.Game.Engine;
using System.Collections.Generic;

namespace StrikeforceInfinity.Game.Loudouts
{
    /// <summary>
    /// Contains a single instance of a player loadout.
    /// </summary>
    internal class PlayerShipLoadout
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public HgPlayerClass Class { get; set; }
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

        public PlayerShipLoadout(HgPlayerClass shipClass)
        {
            Class = shipClass;
        }
    }
}
