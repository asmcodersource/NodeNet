using NodeNet.NodeNet.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet.HttpCommunication
{
    internal class HttpConnections : INodeConnections
    {
        List<INodeConnection> nodeHttpConnections = new List<INodeConnection>();

        public void AddConnection(INodeConnection connection)
        {
            nodeHttpConnections.Add(connection);
        }

        public void RemoveConnection(INodeConnection connection)
        {
            nodeHttpConnections.Remove(connection);
        }

        public List<INodeConnection> Connections()
        {
            return new List<INodeConnection>(nodeHttpConnections);
        }
    }
}
