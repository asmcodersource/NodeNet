namespace NodeNet.NodeNet.Communication
{
    public interface INodeListener
    {
        public event Action<INodeConnection> ConnectionOpened;
        public string GetConnectionAddress();
        public void StartListening();
        public void StopListening();
    }
}
