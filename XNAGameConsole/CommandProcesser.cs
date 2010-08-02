using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAGameConsole
{
    class CommandProcesser//:ICommandProcesser
    {
        public IEnumerable<Command> Commands { get; set; }

        public CommandProcesser(IEnumerable<Command> commands)
        {
            Commands = commands;
        }

        public string Process(string buffer)
        {
            string commandName = GetCommandName(buffer);
            Command command = Commands.Where(c => c.Name == commandName).FirstOrDefault();
            var arguments = GetArguments(buffer);
            if (command == null)
            {
                return "ERROR: Command not found";
            }
            string commandOutput;
            try
            {
            commandOutput = command.Action(arguments);
            }
            catch (Exception ex)
            {
                commandOutput = "ERROR: " + ex.Message;
            }
            return commandOutput;
        }

        static string GetCommandName(string buffer)
        {
            var firstSpace = buffer.IndexOf(' ');
            return buffer.Substring(0, firstSpace < 0 ? buffer.Length : firstSpace);
        }

        static string[] GetArguments(string buffer)
        {
            var firstSpace = buffer.IndexOf(' ');
            if (firstSpace < 0)
            {
                return null;
            }
            
            var args = buffer.Substring(firstSpace, buffer.Length - firstSpace);
            var allArgs = args.Split(' ');
            return allArgs.Where(a => a != "").ToArray();
        }
    }
}
