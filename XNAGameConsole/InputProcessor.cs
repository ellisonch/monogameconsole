using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using XNAGameConsole.KeyboardCapture;
using KeyEventArgs=XNAGameConsole.KeyboardCapture.KeyEventArgs;
using Keys=Microsoft.Xna.Framework.Input.Keys;

namespace XNAGameConsole
{
    class InputProcessor
    {
        public event EventHandler Open = delegate { };
        public event EventHandler Close = delegate { };
        public event EventHandler PlayerCommand = delegate { };
        public event EventHandler ConsoleCommand = delegate { };

        public CommandHistory CommandHistory { get; set; }
        public OutputLine Buffer { get; set; }
        public List<OutputLine> Out { get; set; }

        private const int BACKSPACE = 8;
        private const int ENTER = 13;
        private const int TAB = 9;
        private bool isActive;
        private CommandProcesser commandProcesser;
        private Renderer renderer;

        public InputProcessor(CommandProcesser commandProcesser, Renderer renderer)
        {
            this.commandProcesser = commandProcesser;
            this.renderer = renderer;
            isActive = false;
            CommandHistory = new CommandHistory();
            Out = new List<OutputLine>();
            Buffer = new OutputLine("", OutputLineType.Command);
            EventInput.CharEntered += EventInput_CharEntered;
            EventInput.KeyDown += EventInput_KeyDown;
        }

        void EventInput_KeyDown(object sender, KeyEventArgs e)
        {
            bool handled = false;
            if (Keyboard.GetState().IsKeyDown(Keys.V) && Keyboard.GetState().IsKeyDown(Keys.LeftControl)) // CTRL + V
            {
                if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA) //Thread Apartment must be in Single-Threaded for the Clipboard to work
                {
                    AddToBuffer(Clipboard.GetText());
                }
                handled = true;
            }

            if (handled)
            {
                return;
            }
            switch (e.KeyCode)
            {
                case Keys.Up: Buffer.Output = CommandHistory.Previous(); break;
                case Keys.Down: Buffer.Output = CommandHistory.Next(); break;
                //case Keys.Left: 
            }
        }

        void EventInput_CharEntered(object sender, CharacterEventArgs e)
        {
            if (e.Character == GameConsoleOptions.Options.ToggleKey)
            {
                isActive = !isActive;
                if (isActive)
                {
                    Open(this, EventArgs.Empty);
                }
                else
                {
                    Close(this, EventArgs.Empty);
                }
                return;
            }

            if (!isActive)
            {
                return;
            }
            switch ((int)e.Character)
            {
                case ENTER: ExecuteBuffer(); break;
                case BACKSPACE:
                    if (Buffer.Output.Length > 0)
                    {
                        Buffer.Output = Buffer.Output.Substring(0, Buffer.Output.Length - 1);
                    }
                    break;
                case TAB: AutoComplete(); break;
                default:
                    if (IsValid(e.Character))
                    {
                        Buffer.Output += e.Character;
                    }
                    break;
            }
        }

        void ExecuteBuffer()
        {
            if (Buffer.Output.Length == 0)
            {
                return;
            }
            var output = commandProcesser.Process(Buffer.Output);
            Out.Add(new OutputLine(Buffer.Output, OutputLineType.Command));
            Out.Add(new OutputLine(output, OutputLineType.Output));
            CommandHistory.Add(Buffer.Output);
            Buffer.Output = "";
        }

        void AutoComplete()
        {
            var match = GetMatchingCommand();
            if (match == null)
            {
                return;
            }
            var restOfTheCommand = match.Name.Substring(Buffer.Output.Length);
            Buffer.Output += restOfTheCommand + " ";
        }

        Command GetMatchingCommand()
        {
            var matchingCommands = commandProcesser.Commands.Where(c => c.Name.IndexOf(Buffer.Output, 0) == 0);
            return matchingCommands.Count() < 1 ? null : matchingCommands.First();
        }

        public void AddToBuffer(string text)
        {
            Out.Add(new OutputLine(text, OutputLineType.Output));
            if (GameConsoleOptions.Options.OpenOnWrite)
            {
                Open(this, EventArgs.Empty);
            }
        }

        bool IsValid(char letter)
        {
            return GameConsoleOptions.Options.Font.Characters.Contains(letter);
        }
    }
}
