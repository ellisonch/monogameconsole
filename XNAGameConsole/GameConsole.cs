using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNAGameConsole.Hooks;
using System.Reflection;

namespace XNAGameConsole
{
    public class GameConsole : DrawableGameComponent
    {
        public char ActivateKey { get; set; }

        private SpriteBatch spriteBatch;
        private InputProcessor inputProcesser;
        private Renderer renderer;

        public GameConsole(Game game, SpriteBatch spriteBatch, char activateKey, IEnumerable<Command> commands) : base(game)
        {
            this.spriteBatch = spriteBatch;
            inputProcesser = new InputProcessor(activateKey, new PlayerCommandProcesser(commands));
            inputProcesser.Open += (s, e) => renderer.Open();
            inputProcesser.Close += (s, e) => renderer.Close();


            ActivateKey = activateKey;
            renderer = new Renderer(game.GraphicsDevice, spriteBatch, inputProcesser, Game.Content.Load<SpriteFont>("ConsoleFont"));
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
