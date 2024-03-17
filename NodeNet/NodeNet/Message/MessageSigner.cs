using NodeNet.NodeNet.RSAEncryptions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NodeNet.NodeNet.Message
{
    public class MessageSigner : IMessageSigner
    {
        public SenderSignOptions SignOptions { get; protected set; }


        public void SetSignOptions(ISenderSignOptions options)
        {
            SignOptions = options as SenderSignOptions;
            if (SignOptions == null)
                throw new ArgumentException(nameof(options));
        }

        public void Sign(Message message)
        {
            if (SignOptions == null)
                throw new NullReferenceException(nameof(SignOptions));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, message.Info);
                formatter.Serialize(memoryStream, message.Data);

                string sign = RSAEncryption.Sign(memoryStream.ToArray(), SignOptions);
                message.SetMessageSign(sign);
            }
        }
    }
}
