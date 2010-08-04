using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNAGameConsole
{
    public class GameConsole
    {
        public GameConsoleOptions Options { get { return GameConsoleOptions.Options; } }
        public List<Command> Commands { get { return GameConsoleOptions.Commands; } }
        public bool Enabled { get; set; }

        private GameConsoleComponent console;

        public GameConsole(Game game, SpriteBatch spriteBatch, IEnumerable<Command> commands) :this(game,spriteBatch,commands,new GameConsoleOptions()){}
        public GameConsole(Game game, SpriteBatch spriteBatch, IEnumerable<Command> commands, GameConsoleOptions options) 
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
            
        }
    }
}
