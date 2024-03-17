using NodeNet.NodeNet.RSAEncryptions;

namespace NodeNet.NodeNet.Message
{
    public interface IMessageSigner
    {
        void SetSignOptions(ISenderSignOptions options);
        void Sign(Message message);
    }
}
