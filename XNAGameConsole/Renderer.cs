using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNAGameConsole
{
    class Renderer
    {
        enum State
        {
            Opened,
            Opening,
            Closed,
            Closing
        }

        private readonly SpriteBatch spriteBatch;
        private readonly InputProcessor inputProcessor;
        private readonly SpriteFont consoleFont;
        private readonly int commandSpacing;
        private Texture2D consoleBackground;
        private int width, height = 300, margin = 15, padding = 5;
        private State CurrentState;
        private Color fontColor;
        private Vector2 OpenedPosition, ClosedPosition, Position;
        private DateTime stateChangeTime;
        private float animationSpeed;
        private Vector2 firstCommandPositionOffset;
        private Vector2 firstCommandPosition
        {
            get
            {
                return new Vector2(Position.X + padding, Position.Y + padding) + firstCommandPositionOffset;
            }
        }


        public Renderer(GraphicsDevice device, SpriteBatch spriteBatch, InputProcessor inputProcessor, SpriteFont consoleFont)
        {
            animationSpeed = 0.5f;
            CurrentState = State.Closed;
            width = device.Viewport.Width;
            Position = ClosedPosition = new Vector2(margin,-height);
            OpenedPosition = new Vector2(margin,0);
            this.spriteBatch = spriteBatch;
            this.inputProcessor = inputProcessor;
            commandSpacing = consoleFont.LineSpacing;
            this.consoleFont = consoleFont;
            consoleBackground = new Texture2D(device,1,1,1,TextureUsage.None,SurfaceFormat.Color);
            consoleBackground.SetData(new [] { new Color(0, 0, 0, 125) });
            fontColor = Color.White;
            firstCommandPositionOffset = Vector2.Zero;
        }

        public void Update(GameTime gameTime)
        {
            if (CurrentState == State.Opening)
            {
                Position.Y = MathHelper.SmoothStep(Position.Y, OpenedPosition.Y, ((float)((DateTime.Now - stateChangeTime).TotalSeconds / animationSpeed)));
                if (Position.Y == OpenedPosition.Y)
                {
                    CurrentState = State.Opened;
                }
            }
            if (CurrentState == State.Closing)
            {
                Position.Y = MathHelper.SmoothStep(Position.Y, ClosedPosition.Y, ((float)((DateTime.Now - stateChangeTime).TotalSeconds / animationSpeed)));
                if (Position.Y == ClosedPosition.Y)
                {
                    CurrentState = State.Closed;
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (CurrentState == State.Closed) //Do not draw if the console is closed
            {
                return;
            }
            spriteBatch.Draw(consoleBackground, new Rectangle((int)Position.X, (int)Position.Y, width - margin * 2, height), Color.White);
            var currCommandPosition = DrawExistingCommands();
            DrawCommand(inputProcessor.Buffer.ToString(), currCommandPosition, fontColor);
        }

        void DrawCommand(string command, Vector2 position, Color color)
        {
            ValidateFirstCommandPosition(position.Y);
            Console.WriteLine("{0} {1}",command,consoleFont.MeasureString(command));
            spriteBatch.DrawString(consoleFont, command, new Vector2(position.X + padding, position.Y), color);
        }         

        Vector2 DrawExistingCommands()
        {
            var currPosition = firstCommandPosition;
            foreach (var command in inputProcessor.Out)
            {
                DrawCommand(command.ToString(), currPosition, fontColor);
                currPosition.Y += commandSpacing;
            }
            return currPosition;
        }

        public void Open()
        {
            stateChangeTime = DateTime.Now;
            CurrentState = State.Opening;
        }

        public void Close()
        {
            stateChangeTime = DateTime.Now;
            CurrentState = State.Closing;
        }

        void ValidateFirstCommandPosition(float nextCommandY)
        {
            if (nextCommandY + commandSpacing > OpenedPosition.Y + height)
            {
                firstCommandPositionOffset.Y -= commandSpacing;
            }
        }
    }
}
