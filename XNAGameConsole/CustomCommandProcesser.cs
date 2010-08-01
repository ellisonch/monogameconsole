using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAGameConsole
{
    class CustomCommandProcesser:ICommandProcesser
    {
        private IEnumerable<CustomCommand> commands;

        public CustomCommandProcesser(IEnumerable<CustomCommand> commands)
        {
            this.commands = commands;
        }

        public PastCommand Process(string buffer)
        {
            string commandName = GetCommandName(buffer);
            CustomCommand command = commands.Where(c => c.Name == commandName).FirstOrDefault();
            var arguments = GetArguments(buffer);
            if (command == null)
            {
                return new PastCommand(commandName, arguments,false);
            }
            command.Action(arguments);
            return new PastCommand(commandName, arguments, true);
        }

        static string GetCommandName(string buffer)
        {
            var firstSpace = buffer.IndexOf(' ');
            return buffer.Substring(0, firstSpace < 0 ? buffer.Length : firstSpace);
        }

        static IEnumerable<string> GetArguments(string buffer)
        {
            var firstSpace = buffer.IndexOf(' ');
            if (firstSpace < 0)
            {
                return null;
            }
            
            var args = buffer.Substring(firstSpace, buffer.Length - firstSpace);
            var allArgs = args.Split(' ');
            return allArgs.Where(a => a != "");
        }
    }
}
