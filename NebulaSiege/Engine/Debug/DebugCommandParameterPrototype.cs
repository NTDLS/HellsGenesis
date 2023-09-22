namespace NebulaSiege.Engine.Debug
{
    internal class DebugCommandParameterPrototype
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

        public DebugCommandParameterPrototype(string name, bool isRequired, DebugCommandParameterType commandParameterType)
        {
            Name = name;
            IsRequired = isRequired;
            CommandParameterType = commandParameterType;
        }
    }
}
