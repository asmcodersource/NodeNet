using NodeNet.NodeNet.Message;
using NodeNet.NodeNet.SignOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NodeNet.NodeNet.RSASigner
{
    internal class MessageSigner : IMessageSigner
    {
        public SenderSignOptions SignOptions { get; protected set; }


        public void SetSignOptions(ISenderSignOptions options)
        {
            SignOptions = options as SenderSignOptions;
            if( SignOptions == null)
                throw new ArgumentException(nameof(options));
        }

        public void Sign(Message.Message message)
        {
            if( SignOptions == null) 
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
