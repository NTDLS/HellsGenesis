using System.Collections.Generic;

namespace Si.Engine.Interrogation
{
    public class InterrogationCommandPrototype
    {
        public string NameLowered { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public List<InterrogationCommandParameterPrototype> Parameters { get; private set; } = new();

        public InterrogationCommandPrototype(string name, string description)
        {
            Name = name;
            NameLowered = name.ToLower();
            Description = description;
        }
    }
}
