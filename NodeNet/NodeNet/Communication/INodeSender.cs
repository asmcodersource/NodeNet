namespace NodeNet.NodeNet.Communication
{
    public interface INodeSender
    {
        public Task SendMessage(ArraySegment<byte> segment);
        public Task SendMessage(NodeNet.Message.Message message);
    }
}
