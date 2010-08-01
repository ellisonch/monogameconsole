using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAGameConsole
{
    interface ICommandProcesser
    {
        PastCommand Process(string buffer);
    }
}
