using Newtonsoft.Json;
using Si.GameEngine.Loudout;
using System.Collections.Generic;

namespace Si.Engine.Loudout
{
    /// <summary>
    /// Contains sprite metadata.
    /// </summary>
    public class SpriteMetadata
    {
        [JsonIgnore]
        public string Description { get; set; } = string.Empty;
        public float Speed { get; set; } = 1f;
        public float Boost { get; set; } = 0f;
        public int HullHealth { get; set; } = 0;
        public int ShieldHealth { get; set; } = 0;
        public int Bounty { get; set; } = 0;
        public bool TakesDamage { get; set; } = true;

        public List<SpriteMetadataAttachment> Attachments { get; set; } = new();

        public List<ShipLoadoutWeapon> Weapons { get; set; } = new();

        public SpriteMetadata()
        {
        }
    }
}
