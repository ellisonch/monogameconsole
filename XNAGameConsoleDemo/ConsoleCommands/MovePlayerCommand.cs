using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNAGameConsole;
using XNATextInput;

namespace XNAGameConsoleDemo.ConsoleCommands
{
    class MovePlayerCommand:ICommand
    {
        public string Name
        {
            get { return "move"; }
        }

        public string Description
        {
            get { return "Moves the player"; }
        }

        private Player player;
        public MovePlayerCommand(Player player)
        {
            this.player = player;
        }

        public string Execute(string[] arguments)
        {
            var newPosition = new Vector2(float.Parse(arguments[0]), float.Parse(arguments[1]));
            player.Position = newPosition;
            return "Moved player to " + newPosition;
        }
    }
}
