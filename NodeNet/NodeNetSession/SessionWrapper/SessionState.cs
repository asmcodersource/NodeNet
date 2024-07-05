using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.SessionWrapper.SessionWrapper
{
    internal enum SessionState
    {
        Created,
        WaitingForHandshake,
        Established,
        Disconnected,
        Faulted,
    }
}
