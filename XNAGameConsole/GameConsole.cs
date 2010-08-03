using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNAGameConsole.KeyboardCapture;

namespace XNAGameConsole
{
    public class GameConsole : DrawableGameComponent
    {
        private readonly SpriteBatch spriteBatch;
        private readonly InputProcessor inputProcesser;
        private readonly Renderer renderer;

        public GameConsole(Game game, SpriteBatch spriteBatch, IEnumerable<Command> commands) : this(game,spriteBatch,commands, new GameConsoleOptions()){}
        public GameConsole(Game game, SpriteBatch spriteBatch, IEnumerable<Command> commands, GameConsoleOptions options) : base(game)
        {
            GameConsoleOptions.Options = options;
            EventInput.Initialize(game.Window);
            this.spriteBatch = spriteBatch;
            inputProcesser = new InputProcessor(new CommandProcesser(commands));
            inputProcesser.Open += (s, e) => renderer.Open();
            inputProcesser.Close += (s, e) => renderer.Close();

            renderer = new Renderer(game, spriteBatch, inputProcesser, Game.Content.Load<SpriteFont>("ConsoleFont"));
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
    }
}
