using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAGameConsole
{
    public class Command
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Func<string[], string> Action { get; set; }

        public Command(string name, Func<string[], string> action) : this(name,action,"") { }
        public Command(string name, Func<string[], string> action, string description)
        {
            Name = name;
            Action = action;
            Description = description;
        }

    }
}
