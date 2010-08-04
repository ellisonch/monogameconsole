using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNAGameConsole.Commands;

namespace XNAGameConsole
{
    public class GameConsole
    {
        public GameConsoleOptions Options { get { return GameConsoleOptions.Options; } }
        public List<ICommand> Commands { get { return GameConsoleOptions.Commands; } }
        public bool Enabled { get; set; }

        private GameConsoleComponent console;

        public GameConsole(Game game, SpriteBatch spriteBatch, GameConsoleOptions options) : this(game, spriteBatch, new ICommand[0],options){}
        public GameConsole(Game game, SpriteBatch spriteBatch, IEnumerable<ICommand> commands) :this(game,spriteBatch,commands,new GameConsoleOptions()){}
        public GameConsole(Game game, SpriteBatch spriteBatch, IEnumerable<ICommand> commands, GameConsoleOptions options) 
        {
            if (options.Font == null)
            {
                options.Font = game.Content.Load<SpriteFont>("ConsoleFont");
            }
            GameConsoleOptions.Options = options;
            GameConsoleOptions.Commands = commands.ToList();
            Enabled = true;
            console = new GameConsoleComponent(this, game, spriteBatch);
            game.Services.AddService(typeof(GameConsole), this);
            game.Components.Add(console);
        }

        /// <summary>
        /// Write directly to the output stream of the console
        /// </summary>
        /// <param name="text"></param>
        public void Write(string text)
        {
            console.WriteLine(text);
        }

        public void AddCommand(params ICommand[] command)
        {
            Commands.AddRange(command);
        }

        public void AddCommand(string name, Func<string[], string> action)
        {
            AddCommand(name, action,"");
        }

        public void AddCommand(string name, Func<string[], string> action, string description)
        {
            Commands.Add(new CustomCommand(name,action,description));
        }
    }
}
