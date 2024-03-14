using Newtonsoft.Json;
using Si.Engine.Loudout;
using Si.Library.Mathematics.Geometry;
using System.Collections.Generic;

namespace Si.GameEngine.Loudout
{
    public class SpriteLayoutAttachment
    {
        public string Type { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        [JsonIgnore]
        public SiPoint LocationRelativeToOwner { get => new SiPoint(X, Y); }

        public List<ShipLoadoutWeapon> Weapons { get; set; } = new();
    }
}
