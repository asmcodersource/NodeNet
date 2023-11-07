using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet.Communication
{
    internal interface INodeConnection : INodeSender, INodeReceiver
    {
        public event Action<INodeConnection> MessageReceived;
        public event Action<INodeConnection> WebSocketClosed;
        public bool Connect(string addr);
        public void CloseConnection();
    }
}
