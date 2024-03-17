using NodeNet.NodeNet.RSAEncryptions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NodeNet.NodeNet.Message
{
    public class MessageValidator : IMessageValidator
    {
        public ReceiverSignOptions ValidateOptions { get; protected set; }


        public void SetValidateOptions(IReceiverSignOptions options)
        {
            ValidateOptions = options as ReceiverSignOptions;
            if (ValidateOptions == null)
                throw new ArgumentException(nameof(options));
        }

        public bool Validate(Message message)
        {
            if (ValidateOptions == null)
                throw new NullReferenceException(nameof(ValidateOptions));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, message.Info);
                formatter.Serialize(memoryStream, message.Data);
                return RSAEncryption.VerifySign(memoryStream.ToArray(), message.MessageSign, ValidateOptions);
            }
        }

        public static IReceiverSignOptions GetReceiverValidateOptions(Message message)
        {
            return new ReceiverSignOptions(message.Info.SenderPublicKey);
        }
    }
}
