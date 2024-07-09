using NodeNet.NodeNet.RSAEncryptions;
using NodeNet.NodeNet.SignOptions;

namespace NodeNet.NodeNet.Message
{
    public interface IMessageSigner
    {
        void SetSignOptions(ISenderSignOptions options);
        void Sign(Message message);
    }
}
