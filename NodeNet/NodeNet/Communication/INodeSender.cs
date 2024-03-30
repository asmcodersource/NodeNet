namespace NodeNet.NodeNet.Communication
{
    public interface INodeSender
    {
        public void SendMessage(NodeNet.Message.Message message);
    }
}
