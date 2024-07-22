using NodeNet.NodeNet.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNetSession.MessageWaiter
{
    public class MessageSenderFilterPredicate
    {
        static public MessageFilterPredicate CreateFilter(string senderPublicKey)
        {
            return (messageContext) => MessageSenderFilterPredicate.Method(messageContext.MessageContext, senderPublicKey);
        }

        static private bool Method(MessageContext messageContext, string senderPublicKey)
        {
            return messageContext.Message.Info.SenderPublicKey == senderPublicKey;
        }
    }
}
