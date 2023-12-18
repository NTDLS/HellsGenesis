using System.Collections.Generic;

namespace NebulaSiege.Game.Engine.Debug
{
    internal class DebugCommandPrototype
    {
        public string NameLowered { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public List<DebugCommandParameterPrototype> Parameters { get; private set; } = new();

        public DebugCommandPrototype(string name, string description)
        {
            Name = name;
            NameLowered = name.ToLower();
            Description = description;
        }
    }
}
