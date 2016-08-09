using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameConsoleDemo
{
    public class Player
    {
        public Vector2 Position { get; set; }
        public float Angle { get; set; }
        Vector2 Velocity
        {
            get
            {
                return new Vector2((float)Math.Sin(Angle), -(float)Math.Cos(Angle)) * 10;
            }
        }


        Texture2D PlayerTexture { get; set; }

        private SpriteBatch spriteBatch;

        public Player(Game game)
        {
            spriteBatch = (SpriteBatch) game.Services.GetService(typeof (SpriteBatch));
            PlayerTexture = game.Content.Load<Texture2D>("p1");
            Position = new Vector2(300,300);
        }

        public void Update(GameTime gameTime)
        {
            Angle += Keyboard.GetState().IsKeyDown(Keys.Right) ? 0.1f : Keyboard.GetState().IsKeyDown(Keys.Left) ? -0.1f : 0;
            Position += Keyboard.GetState().IsKeyDown(Keys.Up) ? Velocity : Keyboard.GetState().IsKeyDown(Keys.Down) ? -Velocity : Vector2.Zero;
        }

        public void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(PlayerTexture, Position, null, Color.White, Angle, new Vector2(PlayerTexture.Width / 2, PlayerTexture.Height / 2 + 10), 1, SpriteEffects.None, 1);
        }
    }
}
