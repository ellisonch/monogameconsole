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
        private readonly SpriteBatch spriteBatch;
        private readonly InputProcessor inputProcesser;
        private readonly Renderer renderer;

        public GameConsole(Game game, SpriteBatch spriteBatch, IEnumerable<Command> commands) : this(game, spriteBatch, commands, new GameConsoleOptions()) { }
        public GameConsole(Game game, SpriteBatch spriteBatch, IEnumerable<Command> commands, GameConsoleOptions options)
            : base(game)
        {
            if (options.Font == null)
            {
                options.Font = Game.Content.Load<SpriteFont>("ConsoleFont");
            }
            GameConsoleOptions.Options = options;
            EventInput.Initialize(game.Window);
            this.spriteBatch = spriteBatch;
            inputProcesser = new InputProcessor(new CommandProcesser(commands), renderer);
            inputProcesser.Open += (s, e) => renderer.Open();
            inputProcesser.Close += (s, e) => renderer.Close();

            renderer = new Renderer(game, spriteBatch, inputProcesser);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            renderer.Draw(gameTime);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            renderer.Update(gameTime);
            base.Update(gameTime);
        }

        public void WriteLine(string text)
        {
            inputProcesser.AddToBuffer(text);
        }
    }
}