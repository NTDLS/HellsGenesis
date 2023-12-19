using StrikeforceInfinity.Shared.Messages.Notify;

namespace StrikeforceInfinity.Shared.Payload
{
    public class SiSpriteInfo
    {
        public string FullTypeName { get; set; } = string.Empty;
        public SiSpriteAbsoluteState State { get; set; } = new();

        public Type NativeType()
        {
            var type = Type.GetType(FullTypeName);
            if (type == null)
            {
                throw new Exception("The sprite type could not be determined.");
            }
            return type;
        }

        public SiSpriteInfo(Type type)
        {
            if (type.FullName == null)
            {
                throw new Exception("The sprite type name cannot be null.");
            }
            FullTypeName = type.FullName;
        }

        public SiSpriteInfo()
        {
        }
    }
}
