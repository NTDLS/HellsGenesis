namespace Si.GameEngine.Interrogation
{
    public class InterrogationCommandParameterPrototype
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

        public InterrogationCommandParameterPrototype(string name, bool isRequired, string defautValue, DebugCommandParameterType commandParameterType)
        {
            Name = name;
            IsRequired = isRequired;
            DefaultValue = defautValue;
            CommandParameterType = commandParameterType;
        }
    }
}
