using NodeNet.NodeNet.Communication;

namespace NodeNet.NodeNet.Message
{
    // Represents received message in connection context
    // Can hold any data additional, wich can be useful in receive pipeline
    public class MessageContext
    {
        public Message Message { get; protected set; }
        public INodeConnection SenderConnection { get; protected set; }

        public MessageContext(Message message, INodeConnection senderConnection)
        {
            Message = message;
            SenderConnection = senderConnection;
        }
    }
}
