using System;

namespace Si.GameEngine.Sprite.SupportingClasses.Metadata
{
    public struct InteractiveSpriteWeapon
    {
        public InteractiveSpriteWeapon() { }

        public string Type { get; set; }
        public int MunitionCount { get; set; }


        public InteractiveSpriteWeapon(Type type, int munitionCount)
        {
            Type = type.Name;
            MunitionCount = munitionCount;
        }
    }
}
