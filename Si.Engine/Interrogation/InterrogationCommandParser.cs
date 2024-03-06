using System;
using System.Collections.Generic;
using System.Linq;
using static Si.Engine.Interrogation.InterrogationCommandParameterPrototype;

namespace Si.Engine.Interrogation
{
    public class InterrogationCommandParser
    {
        public List<InterrogationCommandPrototype> Commands { get; private set; } = new();

        public InterrogationCommandParser(string[] prototypes)
        {
            foreach (var prototype in prototypes)
            {
                var commandParts = prototype.Split('|');

                if (commandParts.Length != 3)
                {
                    throw new Exception("Malformed debug command prototype.");
                }

                var command = new InterrogationCommandPrototype(commandParts[0], commandParts[2]);

                var commandParameters = commandParts[1].Split(",");

                foreach (var commandParameter in commandParameters)
                {
                    if (string.IsNullOrEmpty(commandParameter) == false)
                    {
                        var commandParameterParts = commandParameter.Split(":");

                        if (commandParameterParts.Length != 3)
                        {
                            throw new Exception("Malformed debug command prototype parameter.");
                        }

                        bool isRequired = commandParameterParts[1].Trim().ToLower() == "required";
                        string defaultValue = null;

                        if (isRequired == false && commandParameterParts[1].Contains('='))
                        {
                            int indexOfEqual = commandParameterParts[1].IndexOf('=');
                            defaultValue = commandParameterParts[1].Substring(indexOfEqual + 1).Trim();
                        }

                        command.Parameters.Add(new InterrogationCommandParameterPrototype(
                            commandParameterParts[0], isRequired, defaultValue,
                            Enum.Parse<DebugCommandParameterType>(commandParameterParts[2], true)
                        ));
                    }
                }

                Commands.Add(command);
            }
        }

        public InterrogationCommand Parse(string commandText)
        {
            int paramStartIndex = commandText.IndexOf(' ');

            string commandName;

            if (paramStartIndex > 0)
            {
                commandName = commandText.Substring(0, paramStartIndex).Trim(); //We have parameters
                commandText = commandText.Substring(paramStartIndex).Trim();
            }
            else
            {
                commandName = commandText.Trim(); //We have no parameters.
                commandText = string.Empty;
            }

            var commandPrototype = Commands.Where(o => o.Name.ToLower() == commandName.ToLower()).FirstOrDefault();
            if (commandPrototype == null)
            {
                throw new Exception($"Unknown command '{commandName}'.");
            }

            var commandParameters = commandText.Split(',', StringSplitOptions.RemoveEmptyEntries);

            //If the supplied parameter count is more than we expect.
            if (commandParameters.Count() > commandPrototype.Parameters.Count)
            {
                throw new Exception($"Too many parameters supplied to '{commandName}'.");
            }

            var parsedCommand = new InterrogationCommand(commandName);

            int paramIndex = 0;

            //Loop though the supplied parameters:
            for (; paramIndex < commandParameters.Length; paramIndex++)
            {
                var paramPrototype = commandPrototype.Parameters[paramIndex];
                parsedCommand.Parameters.Add(new InterrogationCommandParameter(paramPrototype, commandParameters[paramIndex]));
            }

            //Loop through the not-supplied parameters:
            for (; paramIndex < commandPrototype.Parameters.Count; paramIndex++)
            {
                var paramPrototype = commandPrototype.Parameters[paramIndex];
                if (paramPrototype.IsRequired)
                {
                    throw new Exception($"Command '{parsedCommand.Name}' parameter '{paramPrototype.Name}' is not optional.");
                }

                parsedCommand.Parameters.Add(new InterrogationCommandParameter(paramPrototype, null));
            }

            return parsedCommand;
        }
    }
}
