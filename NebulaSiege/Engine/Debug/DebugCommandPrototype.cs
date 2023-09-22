using System.Collections.Generic;

namespace NebulaSiege.Engine.Debug
{
    internal class DebugCommandPrototype
    {
        public string FriendlyName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Space))
                {
                    return Name;
                }
                else
                {
                    return $"{Space}-{Name}";
                }
            }
        }

        public string Space { get; private set; } //Sprite, display, core, "none", etc.
        public string Name { get; private set; }
        public List<DebugCommandParameterPrototype> Parameters { get; private set; } = new();

        public DebugCommandPrototype(string space, string name)
        {
            Space = space;
            Name = name;
        }
    }
}
