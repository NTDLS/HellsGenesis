namespace Si.GameEngine.Core.Debug
{
    public class DebugCommandParameterPrototype
    {
        public enum DebugCommandParameterType
        {
            String,
            Numeric,
            Boolean,
            Criterion
        }

        public string Name { get; private set; }
        public DebugCommandParameterType CommandParameterType { get; private set; }
        public bool IsRequired { get; private set; }

        public string DefaultValue { get; private set; }

        public DebugCommandParameterPrototype(string name, bool isRequired, string defautValue, DebugCommandParameterType commandParameterType)
        {
            Name = name;
            IsRequired = isRequired;
            DefaultValue = defautValue;
            CommandParameterType = commandParameterType;
        }
    }
}
