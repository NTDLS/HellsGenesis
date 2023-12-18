using NebulaSiege.Game.Engine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace NebulaSiege.Game.Loudouts
{
    /// <summary>
    /// Contains a single instance of a enemy loadout.
    /// </summary>
    internal class EnemyShipLoadout
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public HgEnemyClass Class { get; set; }
        [JsonIgnore]
        public int ImageIndex => (int)Class;
        [JsonIgnore]
        public string Name => Class.ToString();
        public string Description { get; set; }
        public double MaxSpeed { get; set; }
        public double MaxBoost { get; set; }
        public int HullHealth { get; set; }
        public int ShieldHealth { get; set; }

        public List<ShipLoadoutWeapon> Weapons { get; set; } = new();

        public EnemyShipLoadout()
        {
        }

        public EnemyShipLoadout(HgEnemyClass shipClass)
        {
            Class = shipClass;
        }
    }
}
