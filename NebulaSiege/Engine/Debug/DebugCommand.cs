using System;
using System.Collections.Generic;
using System.Linq;

namespace NebulaSiege.Engine.Debug
{
    internal class DebugCommand
    {
        public string Name { get; set; }
        public List<DebugCommandParameter> Parameters { get; private set; } = new();

        public string PhysicalFunctionKey => $"DebugHandler_{Name}".Replace('-', '_').Replace("__", "_").ToLower();

        public T ParameterValue<T>(string parameterName)
        {
            var parameter = Parameters.Where(o => o.Prototype.Name.ToLower() == parameterName.ToLower()).FirstOrDefault();
            if (parameter == null)
            {
                throw new Exception($"Parameter '{parameterName}' was not found.");
            }

            if (parameter.RawValue == null)
            {
                if (parameter.Prototype.IsRequired)
                {
                    throw new Exception($"Parameter '{parameter.Prototype.Name}' is not optional.");
                }
                return default;
            }

            return (T)Convert.ChangeType(parameter.RawValue, typeof(T));
        }

        public DebugCommand(string name)
        {
            Name = name;
        }
    }
}
