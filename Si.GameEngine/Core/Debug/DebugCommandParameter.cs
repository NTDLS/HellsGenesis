using System;
using System.Text.RegularExpressions;
using static Si.GameEngine.Core.Debug.DebugCommandParameterPrototype;

namespace Si.GameEngine.Core.Debug
{
    public class DebugCommandParameter
    {
        public DebugCommandParameterPrototype Prototype { get; private set; }
        public object RawValue { get; private set; }

        public DebugCommandParameter(DebugCommandParameterPrototype prototype, string value)
        {
            Prototype = prototype;
            RawValue = value;

            if (value != null)
            {
                if (prototype.CommandParameterType == DebugCommandParameterType.String)
                {
                    RawValue = value; //No validation required.
                }
                else if (prototype.CommandParameterType == DebugCommandParameterType.Numeric)
                {
                    if (double.TryParse(value, out var validatedValue) == false)
                    {
                        throw new Exception($"Could not convert value '{value}' for '{prototype.Name}' to numeric.");
                    }
                    RawValue = validatedValue;
                }
                else if (prototype.CommandParameterType == DebugCommandParameterType.Boolean)
                {
                    if (bool.TryParse(value, out var validatedValue) == false)
                    {
                        throw new Exception($"Could not convert value '{value}' for '{prototype.Name}' to booelan.");
                    }
                    RawValue = validatedValue;
                }
                else if (prototype.CommandParameterType == DebugCommandParameterType.Criterion)
                {
                    RawValue = new DebugCommandParameterCriterion(value);
                }
                else
                {
                    throw new Exception($"Unknown parameter type '{prototype.CommandParameterType}' for '{prototype.Name}'.");
                }
            }
        }

        public static bool IsMatchLike(string input, string pattern, bool isNotLike = false)
        {
            if (input == null || pattern == null)
            {
                return false;
            }

            input = input.ToLower();
            pattern = pattern.ToLower();

            string regexPattern = "^" + Regex.Escape(pattern).Replace("%", ".*").Replace("_", ".") + "$";
            var result = Regex.IsMatch(input, regexPattern);
            return isNotLike ? !result : result;
        }
    }
}
