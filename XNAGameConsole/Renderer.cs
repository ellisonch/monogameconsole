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

        int ConsoleWidth
        {
            get
            {
                return width - margin*2;
            }
        }

        private Texture2D roundedEdge;
        private Color consoleColor;

        public Renderer(Game game, SpriteBatch spriteBatch, InputProcessor inputProcessor, SpriteFont consoleFont)
        {
            roundedEdge = game.Content.Load<Texture2D>("roundedCorner");
            animationSpeed = 1f;
            CurrentState = State.Closed;
            width = game.GraphicsDevice.Viewport.Width;
            Position = ClosedPosition = new Vector2(margin,-height - roundedEdge.Height);
            OpenedPosition = new Vector2(margin,0);
            this.spriteBatch = spriteBatch;
            this.inputProcessor = inputProcessor;
            commandSpacing = consoleFont.LineSpacing;
            this.consoleFont = consoleFont;
            consoleColor = new Color(0,0,0,125);
            consoleBackground = new Texture2D(game.GraphicsDevice,1,1,1,TextureUsage.None,SurfaceFormat.Color);
            consoleBackground.SetData(new [] { consoleColor });
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
                //return;
            }
            spriteBatch.Draw(consoleBackground, new Rectangle((int)Position.X, (int)Position.Y, ConsoleWidth, height), Color.White);
            DrawRoundedEdges();
            var currCommandPosition = DrawExistingCommands();
            DrawCommand(inputProcessor.Buffer.ToString(), currCommandPosition, fontColor);
        }

        void DrawRoundedEdges()
        {
            //Bottom-left edge
            spriteBatch.Draw(roundedEdge, new Vector2(Position.X, Position.Y + height), null, consoleColor, 0, Vector2.Zero, 1, SpriteEffects.None, 1); 
            //Bottom-right edge 
            spriteBatch.Draw(roundedEdge, new Vector2(Position.X + ConsoleWidth - roundedEdge.Width, Position.Y + height), null, consoleColor, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 1);
            //connecting bottom-rectangle
            spriteBatch.Draw(consoleBackground, new Rectangle((int)Position.X + roundedEdge.Width, (int)Position.Y + height, ConsoleWidth - (roundedEdge.Width*2), roundedEdge.Height), Color.White);
        }

        Vector2 DrawCommand(string command, Vector2 position, Color color)
        {
            ValidateFirstCommandPosition(position.Y);
            var oneCharWidth = consoleFont.MeasureString(command).X / command.Length;
            var maxCharactersPerLine = (int)((ConsoleWidth - padding) / oneCharWidth) - 1;
            var splitLines = command.Length > maxCharactersPerLine ? SplitCommand(command, maxCharactersPerLine) : new []{command};
            position.X += padding;
            foreach (var line in splitLines)
            {
                spriteBatch.DrawString(consoleFont, line, position, color);
                position.Y += commandSpacing;
            }
            return position;
        }         

        IEnumerable<string> SplitCommand(string command, int max)
        {
            var lines = new List<string>();
            while (command.Length > max)
            {
                var splitCommand = command.Substring(0, max);
                lines.Add(splitCommand);
                command = command.Substring(max, command.Length - max);
            }
            lines.Add(command);

            return lines;
        }

        Vector2 DrawExistingCommands()
        {
            var currPosition = firstCommandPosition;
            foreach (var command in inputProcessor.Out)
            {
                currPosition.Y = DrawCommand(command.ToString(), currPosition, fontColor).Y;
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
