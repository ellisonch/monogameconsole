﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoGameConsole.Commands
{
    class CommandComparer:IComparer<IConsoleCommand>
    {
        public int Compare(IConsoleCommand x, IConsoleCommand y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }
}
