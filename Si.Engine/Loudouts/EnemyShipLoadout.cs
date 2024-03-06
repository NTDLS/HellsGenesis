using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using static Si.Library.SiConstants;

namespace Si.Engine.Loudouts
{
    /// <summary>
    /// Contains a single instance of a enemy loadout.
    /// </summary>
    public class EnemyShipLoadout
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public SiEnemyClass Class { get; set; }
        [JsonIgnore]
        public int ImageIndex => (int)Class;
        [JsonIgnore]
        public string Name => Class.ToString();
        public string Description { get; set; }
        public float Speed { get; set; }
        public float Boost { get; set; }
        public int HullHealth { get; set; }
        public int ShieldHealth { get; set; }

        public List<ShipLoadoutWeapon> Weapons { get; set; } = new();

        public EnemyShipLoadout()
        {
        }

        public EnemyShipLoadout(SiEnemyClass shipClass)
        {
            Class = shipClass;
        }
    }
}
