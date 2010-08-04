using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNAGameConsole;
using XNATextInput;

namespace XNAGameConsoleDemo.ConsoleCommands
{
    class RotatePlayerDegreesCommand:ICommand
    {
        public string Name
        {
            get { return "rotDeg"; }
        }

        public string Description
        {
            get { return "Rotates the player"; }
        }

        private Player player;
        public RotatePlayerDegreesCommand(Player player)
        {
            this.player = player;
        }

        public string Execute(string[] arguments)
        {
            var angle = float.Parse(arguments[0]);
            player.Angle = MathHelper.ToRadians(angle);
            return String.Format("Rotated player to {0} degrees", angle);
        }
    }
}
