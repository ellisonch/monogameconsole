using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using XNAGameConsole.KeyboardCapture;

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
        private char toggleKey;
        private bool isActive;
        private CommandProcesser commandProcesser;

        public InputProcessor(char toggleKey, CommandProcesser commandProcesser)
        {
            this.commandProcesser = commandProcesser;
            isActive = false;
            this.toggleKey = toggleKey;
            CommandHistory = new CommandHistory();
            Out = new List<OutputLine>();
            Buffer = new OutputLine("",OutputLineType.Command);
            EventInput.CharEntered += EventInput_CharEntered;
            EventInput.KeyDown += EventInput_KeyDown;
        }

        void EventInput_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up: Buffer.Output = CommandHistory.Previous(); break;
                case Keys.Down: Buffer.Output = CommandHistory.Next(); break;
            }
        }

        void EventInput_CharEntered(object sender, CharacterEventArgs e)
        {
            if (e.Character == toggleKey)
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
                    Buffer.Output += e.Character;
                    break;
            }
        }

        void ExecuteBuffer()
        {
            var output = commandProcesser.Process(Buffer.Output);
            Out.Add(new OutputLine(Buffer.Output,OutputLineType.Command));
            Out.Add(new OutputLine(output,OutputLineType.Output));
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
    }
}
