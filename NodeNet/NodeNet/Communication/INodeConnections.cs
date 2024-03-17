namespace NodeNet.NodeNet.Communication
{
    public interface INodeConnections
    {
        public List<INodeConnection> Connections();
        public void AddConnection(INodeConnection connection);
        public void RemoveConnection(INodeConnection connection);
    }
}
