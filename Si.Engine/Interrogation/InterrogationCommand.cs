using System;
using System.Collections.Generic;
using System.Linq;

namespace Si.Engine.Interrogation
{
    public class InterrogationCommand
    {
        public string Name { get; set; }
        public List<InterrogationCommandParameter> Parameters { get; private set; } = new();

        public string PhysicalFunctionKey => $"CommandHandler_{Name}".Replace('-', '_').Replace("__", "_").ToLower();

        public T ParameterValue<T>(string parameterName, T defaultValue)
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
                return defaultValue;
            }

            return (T)Convert.ChangeType(parameter.RawValue, typeof(T));
        }

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

                if (parameter.Prototype.DefaultValue != null)
                {
                    return (T)Convert.ChangeType(parameter.Prototype.DefaultValue, typeof(T));
                }

                return default;
            }

            return (T)Convert.ChangeType(parameter.RawValue, typeof(T));
        }

        public InterrogationCommand(string name)
        {
            Name = name;
        }
    }
}
