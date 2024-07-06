using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNetSession.Session
{
    public enum SessionState
    {
        Created,
        WaitingForHandshake,
        Established,
        Disconnected,
        Faulted,
    }
}
