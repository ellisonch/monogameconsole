using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAGameConsole
{
    public class CustomCommand
    {
        public string Name { get; set; }
        public Action<IEnumerable<string>> Action { get; set; }
        public string Description { get; set; }

        public CustomCommand(string name, Action<IEnumerable<string>> action) : this(name,action,"") { }
        public CustomCommand(string name, Action<IEnumerable<string>> action, string description)
        {
            Name = name;
            Action = action;
            Description = description;
        }

    }
}
