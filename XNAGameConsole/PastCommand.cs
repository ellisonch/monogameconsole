using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAGameConsole
{
    class PastCommand
    {
        public string Name { get; set; }
        public IEnumerable<string> Arguments { get; set; }
        public bool Status { get; set; }

        public PastCommand(string name, IEnumerable<string> args, bool status)
        {
            Name = name;
            Arguments = args;
            Status = status;
        }

        public override string ToString()
        {
            string args = Arguments != null && Arguments.Count() > 0 ? string.Join(" ", Arguments.ToArray()) : "";
            return String.Format("{0} {1}", Name, args);
        }
    }
}
