using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAGameConsole
{
    class CommandHistory:List<string>
    {
        public int Index { get; set; }

        public void Reset()
        {
            Index = Count - 1;
        }

        public string Next()
        {
            return Index + 1 > Count - 1 ? this[Index] : this[++Index];
        }

        public string Previous()
        {
            return Index - 1 < 0 ? this[0] : this[--Index];
        }

        public new void Add(string command)
        {
            var parts = command.Split('\n');
            foreach (var part in parts)
            {
                if (part != "")
                {
                    base.Add(part);
                }
            }
            Index = Count;
        }
    }
}
