using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAGameConsole.Commands
{
    class CommandComparer:IComparer<ICommand>
    {
        public int Compare(ICommand x, ICommand y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }
}
