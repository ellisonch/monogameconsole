using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using XNAGameConsole;
using XNAGameConsoleDemo.ConsoleCommands;

namespace XNATextInput
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private GameConsole console;
        private Player player;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), spriteBatch);

            player = new Player(this);
            var commands = new IConsoleCommand[] { new MovePlayerCommand(player), new RotatePlayerDegreesCommand(player) };
            console = new GameConsole(this, spriteBatch, commands, new GameConsoleOptions
                                                                       {
                                                                           Font = Content.Load<SpriteFont>("GameFont"),
                                                                           FontColor = Color.LawnGreen,
                                                                           Prompt = "~>",
                                                                           PromptColor = Color.Crimson,
                                                                           CursorColor = Color.OrangeRed,
                                                                           BackgroundColor = new Color(0, 0, 0, 150), //Color.BLACK with transparency
                                                                           PastCommandOutputColor = Color.Aqua,
                                                                           BufferColor = Color.Gold
                                                                       });
            console.AddCommand("rotRad", a =>
                                             {
                                                 var angle = float.Parse(a[0]);
                                                 player.Angle = angle;
                                                 return String.Format("Rotated the player to {0} radians", angle);
                                             });
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (!console.Opened)
            {
                player.Update(gameTime);
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            player.Draw(gameTime);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
