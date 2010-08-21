using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAGameConsole
{
    enum OutputLineType
    {
        Command,
        Output
    }

    class OutputLine
    {
        public string Output { get; set; }
        public OutputLineType Type { get; set; }

        public OutputLine(string output, OutputLineType type)
        {
            Output = output;
            Type = type;
        }

        public override string ToString()
        {
            return Output;
            switch (Type)
            {
                case OutputLineType.Command: return String.Format("{0} {1}", GameConsoleOptions.Options.Prompt, Output);
                case OutputLineType.Output: return String.Format("{0}", Output);
            }
            return "";
        }
    }
}
