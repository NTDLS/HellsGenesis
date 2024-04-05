using Newtonsoft.Json;
using Si.Library.Mathematics.Geometry;
using System.Collections.Generic;

namespace Si.GameEngine.Sprite.SupportingClasses.Metadata
{
    public class InteractiveSpriteAttachment
    {
        public string Type { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        [JsonIgnore]
        public SiVector LocationRelativeToOwner { get => new SiVector(X, Y); }

        public List<InteractiveSpriteWeapon> Weapons { get; set; } = new();
    }
}
