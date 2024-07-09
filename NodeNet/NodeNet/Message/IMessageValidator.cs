using NodeNet.NodeNet.RSAEncryptions;
using NodeNet.NodeNet.SignOptions;

namespace NodeNet.NodeNet.Message
{
    public interface IMessageValidator
    {
        void SetValidateOptions(IReceiverSignOptions options);
        bool Validate(Message message);
    }
}
