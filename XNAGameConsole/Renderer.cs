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

        public bool IsOpen
        {
            get
            {
                return CurrentState == State.Opened;
            }
        }

        private readonly SpriteBatch spriteBatch;
        private readonly InputProcessor inputProcessor;
        private Texture2D pixel;
        private int width;
        private State CurrentState;
        private Vector2 OpenedPosition, ClosedPosition, Position;
        private DateTime stateChangeTime;
        private Vector2 firstCommandPositionOffset;
        private Vector2 FirstCommandPosition
        {
            get
            {
                return new Vector2(InnerBounds.X, InnerBounds.Y) + firstCommandPositionOffset;
            }
        }

        Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, width - (GameConsoleOptions.Options.Margin * 2), GameConsoleOptions.Options.Height);
            }
        }

        Rectangle InnerBounds
        {
            get
            {
                return new Rectangle(Bounds.X + GameConsoleOptions.Options.Padding, Bounds.Y + GameConsoleOptions.Options.Padding, Bounds.Width - GameConsoleOptions.Options.Padding, Bounds.Height);
            }
        }

        private float oneCharacterWidth;
        private int maxCharactersPerLine;

        public Renderer(Game game, SpriteBatch spriteBatch, InputProcessor inputProcessor)
        {
            CurrentState = State.Closed;
            width = game.GraphicsDevice.Viewport.Width;
            Position = ClosedPosition = new Vector2(GameConsoleOptions.Options.Margin, -GameConsoleOptions.Options.Height - GameConsoleOptions.Options.RoundedCorner.Height);
            OpenedPosition = new Vector2(GameConsoleOptions.Options.Margin, 0);
            this.spriteBatch = spriteBatch;
            this.inputProcessor = inputProcessor;
            pixel = new Texture2D(game.GraphicsDevice, 1, 1, 1, TextureUsage.None, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });
            firstCommandPositionOffset = Vector2.Zero;
            oneCharacterWidth = GameConsoleOptions.Options.Font.MeasureString("x").X;
            maxCharactersPerLine = (int)((Bounds.Width - GameConsoleOptions.Options.Padding * 2) / oneCharacterWidth);
        }

        public void Update(GameTime gameTime)
        {
            if (CurrentState == State.Opening)
            {
                Position.Y = MathHelper.SmoothStep(Position.Y, OpenedPosition.Y, ((float)((DateTime.Now - stateChangeTime).TotalSeconds / GameConsoleOptions.Options.AnimationSpeed)));
                if (Position.Y == OpenedPosition.Y)
                {
                    CurrentState = State.Opened;
                }
            }
            if (CurrentState == State.Closing)
            {
                Position.Y = MathHelper.SmoothStep(Position.Y, ClosedPosition.Y, ((float)((DateTime.Now - stateChangeTime).TotalSeconds / GameConsoleOptions.Options.AnimationSpeed)));
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
            spriteBatch.Draw(pixel, Bounds, GameConsoleOptions.Options.BackgroundColor);
            DrawRoundedEdges();
            var nextCommandPosition = DrawCommands(inputProcessor.Out, FirstCommandPosition);
            nextCommandPosition = DrawPrompt(nextCommandPosition);
            var bufferPosition = DrawCommand(inputProcessor.Buffer.ToString(), nextCommandPosition, GameConsoleOptions.Options.BufferColor); //Draw the buffer
            DrawCursor(bufferPosition, gameTime);
        }

        void DrawRoundedEdges()
        {
            //Bottom-left edge
            spriteBatch.Draw(GameConsoleOptions.Options.RoundedCorner, new Vector2(Position.X, Position.Y + GameConsoleOptions.Options.Height), null, GameConsoleOptions.Options.BackgroundColor, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            //Bottom-right edge 
            spriteBatch.Draw(GameConsoleOptions.Options.RoundedCorner, new Vector2(Position.X + Bounds.Width - GameConsoleOptions.Options.RoundedCorner.Width, Position.Y + GameConsoleOptions.Options.Height), null, GameConsoleOptions.Options.BackgroundColor, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 1);
            //connecting bottom-rectangle
            spriteBatch.Draw(pixel, new Rectangle(Bounds.X + GameConsoleOptions.Options.RoundedCorner.Width, Bounds.Y + GameConsoleOptions.Options.Height, Bounds.Width - GameConsoleOptions.Options.RoundedCorner.Width * 2, GameConsoleOptions.Options.RoundedCorner.Height), GameConsoleOptions.Options.BackgroundColor);
        }

        void DrawCursor(Vector2 position, GameTime gameTime)
        {
            if (!IsInBounds(position.Y))
            {
                return;
            }
            var split = SplitCommand(inputProcessor.Buffer.ToString(), maxCharactersPerLine).Last();
            position.X += GameConsoleOptions.Options.Font.MeasureString(split).X;
            position.Y -= GameConsoleOptions.Options.Font.LineSpacing;
            spriteBatch.DrawString(GameConsoleOptions.Options.Font, (int)(gameTime.TotalRealTime.TotalSeconds / GameConsoleOptions.Options.CursorBlinkSpeed) % 2 == 0 ? GameConsoleOptions.Options.Cursor.ToString() : "", position, GameConsoleOptions.Options.CursorColor);
        }

        /// <summary>
        /// Draws the specified command and returns the position of the next command to be drawn
        /// </summary>
        /// <param name="command"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        Vector2 DrawCommand(string command, Vector2 position, Color color)
        {
            var splitLines = command.Length > maxCharactersPerLine ? SplitCommand(command, maxCharactersPerLine) : new[] { command };
            foreach (var line in splitLines)
            {
                if (IsInBounds(position.Y))
                {
                    spriteBatch.DrawString(GameConsoleOptions.Options.Font, line, position, color);
                }
                ValidateFirstCommandPosition(position.Y + GameConsoleOptions.Options.Font.LineSpacing);
                position.Y += GameConsoleOptions.Options.Font.LineSpacing;
            }
            return position;
        }

        static IEnumerable<string> SplitCommand(string command, int max)
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

        /// <summary>
        /// Draws the specified collection of commands and returns the position of the next command to be drawn
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        Vector2 DrawCommands(IEnumerable<OutputLine> lines, Vector2 position)
        {
            var originalX = position.X;
            foreach (var command in lines)
            {
                if (command.Type == OutputLineType.Command)
                {
                    position = DrawPrompt(position);
                }
                //position.Y = DrawCommand(command.ToString(), position, GameConsoleOptions.Options.FontColor).Y;
                position.Y = DrawCommand(command.ToString(), position, command.Type == OutputLineType.Command ? GameConsoleOptions.Options.PastCommandColor : GameConsoleOptions.Options.PastCommandOutputColor).Y;
                position.X = originalX;
            }
            return position;
        }

        /// <summary>
        /// Draws the prompt at the specified position and returns the position of the text that will be drawn next to it
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        Vector2 DrawPrompt(Vector2 position)
        {
            spriteBatch.DrawString(GameConsoleOptions.Options.Font, GameConsoleOptions.Options.Prompt, position, GameConsoleOptions.Options.PromptColor);
            position.X += oneCharacterWidth * GameConsoleOptions.Options.Prompt.Length + oneCharacterWidth;
            return position;
        }

        public void Open()
        {
            if (CurrentState == State.Opening || CurrentState == State.Opened)
            {
                return;
            }
            stateChangeTime = DateTime.Now;
            CurrentState = State.Opening;
        }

        public void Close()
        {
            if (CurrentState == State.Closing || CurrentState == State.Closed)
            {
                return;
            }
            stateChangeTime = DateTime.Now;
            CurrentState = State.Closing;
        }

        void ValidateFirstCommandPosition(float nextCommandY)
        {
            if (!IsInBounds(nextCommandY))
            {
                firstCommandPositionOffset.Y -= GameConsoleOptions.Options.Font.LineSpacing;
            }
        }

        bool IsInBounds(float yPosition)
        {
            return yPosition < OpenedPosition.Y + GameConsoleOptions.Options.Height;
        }
    }
}