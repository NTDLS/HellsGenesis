using Newtonsoft.Json;
using Si.Library.Mathematics.Geometry;
using System.Collections.Generic;

namespace Si.GameEngine.Sprite.Metadata
{
    public class InteractiveSpriteAttachment
    {
        public string Type { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        [JsonIgnore]
        public SiPoint LocationRelativeToOwner { get => new SiPoint(X, Y); }

        public List<InteractiveSpriteWeapon> Weapons { get; set; } = new();
    }
}
