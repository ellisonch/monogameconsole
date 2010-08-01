using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNATextInput
{
    public class Player
    {
        public Vector2 Position { get; set; }
        public float Angle { get; set; }

        Texture2D PlayerTexture { get; set; }

        private SpriteBatch spriteBatch;

        public Player(Game game)
        {
            spriteBatch = (SpriteBatch) game.Services.GetService(typeof (SpriteBatch));
            PlayerTexture = game.Content.Load<Texture2D>("player");
            Position = new Vector2(300,300);
        }

        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(PlayerTexture, Position,null,Color.White,Angle,new Vector2(PlayerTexture.Width / 2, PlayerTexture.Height/2),1,SpriteEffects.None,1);
        }

        public void Write()
        {
            Console.WriteLine("ASDASDASDASDASD");
        }
    }
}
