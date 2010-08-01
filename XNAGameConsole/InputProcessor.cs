using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNAGameConsole.KeyboardCapture;

namespace XNAGameConsole
{
    class InputProcessor
    {
        public event EventHandler Open = delegate { };
        public event EventHandler Close = delegate { };
        public event EventHandler PlayerCommand = delegate { };
        public event EventHandler ConsoleCommand = delegate { };

        public List<PastCommand> History { get; set; }
        public string Buffer { get; set; }

        private const int BACKSPACE = 8;
        private const int ENTER = 13;
        private const int TAB = 9;
        private char toggleKey;
        private bool isActive;
        private PlayerCommandProcesser commandProcesser;

        public InputProcessor(char toggleKey, PlayerCommandProcesser commandProcesser)
        {
            this.commandProcesser = commandProcesser;
            isActive = false;
            this.toggleKey = toggleKey;
            History = new List<PastCommand>();
            EventInput.CharEntered += EventInput_CharEntered;
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
                    if (Buffer.Length > 0)
                    {
                        Buffer = Buffer.Substring(0, Buffer.Length - 1);
                    }
                    break;
                case TAB: AutoComplete(); break;
                default:
                    Buffer += e.Character;
                    break;
            }
        }

        void ExecuteBuffer()
        {
            var status = commandProcesser.Process(Buffer);
            History.Add(status);
            Buffer = "";
        }

        void AutoComplete()
        {
            var match = GetMatchingCommand();
            if (match == null)
            {
                return;
            }
            var restOfTheCommand = match.Name.Substring(Buffer.Length);
            Buffer += restOfTheCommand + " ";

        }

        Command GetMatchingCommand()
        {
            var matchingCommands = commandProcesser.Commands.Where(c => c.Name.IndexOf(Buffer, 0) == 0);
            return matchingCommands.Count() < 1 ? null : matchingCommands.First();
        }
    }
}
