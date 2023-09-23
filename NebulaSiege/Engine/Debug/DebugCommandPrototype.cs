using System.Collections.Generic;

namespace NebulaSiege.Engine.Debug
{
    internal class DebugCommandPrototype
    {
        public string Name { get; private set; }
        public List<DebugCommandParameterPrototype> Parameters { get; private set; } = new();

        public DebugCommandPrototype(string name)
        {
            Name = name;
        }
    }
}
