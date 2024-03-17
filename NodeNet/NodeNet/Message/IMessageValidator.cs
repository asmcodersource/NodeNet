using NodeNet.NodeNet.RSAEncryptions;

namespace NodeNet.NodeNet.Message
{
    public interface IMessageValidator
    {
        void SetValidateOptions(IReceiverSignOptions options);
        bool Validate(Message message);
    }
}
