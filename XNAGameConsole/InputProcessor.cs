using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNAGameConsole.Hooks;

namespace XNAGameConsole
{
    class InputProcessor
    {
        public event EventHandler Open = delegate { };
        public event EventHandler Close = delegate { };

        public List<PastCommand> History { get; set; }
        public string Buffer { get; set; }

        private const int BACKSPACE = 8;
        private const int ENTER = 13;
        private char toggleKey;
        private bool isActive;
        private ICommandProcesser commandProcesser;

        public InputProcessor(char toggleKey, ICommandProcesser commandProcesser)
        {
            this.commandProcesser = commandProcesser;
            isActive = false;
            this.toggleKey = toggleKey;
            History = new List<PastCommand>();
            HookManager.KeyPress += HookManager_KeyPress;
        }

        void HookManager_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == toggleKey)
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
                e.Handled = true;
                return;
            }

            if (!isActive)
            {
                return;
            }
            switch ((int)e.KeyChar)
            {
                case ENTER: ExecuteBuffer(); break;
                case BACKSPACE:
                    if (Buffer.Length > 0)
                    {
                        Buffer = Buffer.Substring(0, Buffer.Length - 1);
                    }
                    break;
                default:
                    Buffer += e.KeyChar;
                    break;
            }
            e.Handled = true;
        }

        void ExecuteBuffer()
        {
            var status = commandProcesser.Process(Buffer);
            History.Add(status);
            Buffer = "";
        }
    }
}
