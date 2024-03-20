using System;

namespace Si.GameEngine.Sprite.Metadata
{
    public class InteractiveSpriteWeapon
    {
        public string Type { get; set; }
        public int MunitionCount { get; set; }

        public InteractiveSpriteWeapon() { }

        public InteractiveSpriteWeapon(Type type, int munitionCount)
        {
            Type = type.Name;
            MunitionCount = munitionCount;
        }
    }
}
