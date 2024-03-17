using NodeNet.NodeNet.Communication;

namespace NodeNet.NodeNet.TcpCommunication
{
    public class TcpConnections : INodeConnections
    {
        List<INodeConnection> nodeHttpConnections = new List<INodeConnection>();

        public void AddConnection(INodeConnection connection)
        {
            lock (nodeHttpConnections)
                nodeHttpConnections.Add(connection);
        }

        public void RemoveConnection(INodeConnection connection)
        {
            lock (nodeHttpConnections)
                nodeHttpConnections.Remove(connection);
        }

        public List<INodeConnection> Connections()
        {
            return new List<INodeConnection>(nodeHttpConnections);
        }
    }
}
