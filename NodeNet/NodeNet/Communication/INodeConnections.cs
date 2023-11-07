using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet.Communication
{
    internal interface INodeConnections
    {
        public List<INodeConnection> Connections();
        public void AddConnection(INodeConnection connection);
        public void RemoveConnection(INodeConnection connection);
    }
}
