using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNAGameConsole.KeyboardCapture;

namespace XNAGameConsole
{
    public class GameConsole : DrawableGameComponent
    {
        public GameConsoleOptions Options
        {
            get
            {
                return GameConsoleOptions.Options;
            }
        }
        public bool Active { get; set; }

        private readonly SpriteBatch spriteBatch;
        private readonly InputProcessor inputProcesser;
        private readonly Renderer renderer;
        private List<Command> commands;

        public GameConsole(Game game, SpriteBatch spriteBatch, IEnumerable<Command> commands) : this(game, spriteBatch, commands, new GameConsoleOptions()) { }
        public GameConsole(Game game, SpriteBatch spriteBatch, IEnumerable<Command> commands, GameConsoleOptions options)
            : base(game)
        {
            if (options.Font == null)
            {
                options.Font = Game.Content.Load<SpriteFont>("ConsoleFont");
            }
            this.commands = commands.ToList();
            Active = true;
            GameConsoleOptions.Options = options;
            EventInput.Initialize(game.Window);
            this.spriteBatch = spriteBatch;
            AddPresetCommands();
            inputProcesser = new InputProcessor(new CommandProcesser(commands), renderer);
            inputProcesser.Open += (s, e) => renderer.Open();
            inputProcesser.Close += (s, e) => renderer.Close();

            renderer = new Renderer(game, spriteBatch, inputProcesser);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Active)
            {
                return;
            }
            spriteBatch.Begin();
            renderer.Draw(gameTime);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Active)
            {
                return;
            }
            renderer.Update(gameTime);
            base.Update(gameTime);
        }

        public void WriteLine(string text)
        {
            inputProcesser.AddToOutput(text);
        }

        void AddPresetCommands()
        {
            commands.Add(new Command("exit", a =>
                                                 {
                                                     Game.Exit();
                                                     return "Existing game";
                                                 }, "Forcefully exist the game"));
            commands.Add(new Command("help", a =>
                                                 {
                                                     if (a != null && a.Length >= 1)
                                                     {
                                                         var command = commands.Where(c => c.Name == a[0]).FirstOrDefault();
                                                         if (command != null)
                                                         {
                                                             return String.Format("{0}: {1}\n", command.Name, command.Description);
                                                         }
                                                         return "ERROR: Invalid command '" + a[0] + "'";
                                                     }
                                                     var help = new StringBuilder();
                                                     commands.Sort();
                                                     foreach (var command in commands)
                                                     {
                                                         help.Append(String.Format("{0}: {1}\n", command.Name, command.Description));
                                                     }
                                                     return help.ToString();
                                                 }, "Show all commands and their description"));
        }
    }
}