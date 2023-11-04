using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet.Message
{
    // Main information about message
    // This part of message is signing by sender, and can't be change
    [Serializable]
    internal class MessageInfo
    {
        public string SenderPublicKey { get; protected set; } 
        public string ReceiverPublicKey { get; protected set; } = string.Empty; // Empty means broadcast
        public DateTime SendingTime { get; protected set; }
        public bool IsTechnical { get; protected set; } = false;

        public MessageInfo(string senderPublicKey, string receiverPublicKey)
        {
            SenderPublicKey = senderPublicKey;
            ReceiverPublicKey = receiverPublicKey;
            SendingTime = DateTime.UtcNow;
        }

        public void SetToTechnical()
        {
            IsTechnical = true;
        }
    }
}
