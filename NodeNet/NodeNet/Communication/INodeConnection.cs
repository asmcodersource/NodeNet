namespace NodeNet.NodeNet.Communication
{
    public interface INodeConnection : INodeSender, INodeReceiver
    {
        public event Action<INodeConnection> MessageReceived;
        public event Action<INodeConnection> ConnectionClosed;
        public bool Connect(string addr);
        public void CloseConnection();
        public void ListenMessages();

        public string GetConnectionAddress();
        public Task<byte[]> ReceiveRawData(CancellationToken cancellationToken);
        public Task SendRawData(byte[] data, CancellationToken cancellationToken);

    }
}
