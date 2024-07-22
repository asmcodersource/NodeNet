using NodeNet.NodeNet.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNetSession.MessageWaiter
{
    public class MessageReceiverFilterPredicate
    {
        static public MessageFilterPredicate CreateFilter(string targetPublicKey)
        {
            return (messageContext) => MessageReceiverFilterPredicate.Method(messageContext.MessageContext, targetPublicKey);
        }

        static private bool Method(MessageContext messageContext, string targetPublicKey)
        {
            return messageContext.Message.Info.ReceiverPublicKey == targetPublicKey;
        }
    }
}
