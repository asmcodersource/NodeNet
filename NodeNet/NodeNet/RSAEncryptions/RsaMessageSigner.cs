using NodeNet.NodeNet.RSAEncryptions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NodeNet.NodeNet.SignOptions;
using NodeNet.NodeNet.Message;

namespace NodeNet.NodeNet.RSAEncryptions
{
    public class RsaMessageSigner : IMessageSigner
    {
        public SenderSignOptions SignOptions { get; protected set; }


        public void SetSignOptions(ISenderSignOptions options)
        {
            SignOptions = options as SenderSignOptions;
            if (SignOptions == null)
                throw new ArgumentException(nameof(options));
        }

        public void Sign(Message.Message message)
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
