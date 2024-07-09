using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NodeNet.NodeNet.Message;
using NodeNet.NodeNet.SignOptions;

namespace NodeNet.NodeNet.RSASigner
{
    internal class ReceiverSignOptions : IReceiverSignOptions
    {
        public string PublicKey { get; set; } = "";

        public ReceiverSignOptions(Message.Message message)
        {
            PublicKey = message.Info.SenderPublicKey;
        }

        public ReceiverSignOptions(string publicKey)
        {
            PublicKey = publicKey;
        }
    }
}
