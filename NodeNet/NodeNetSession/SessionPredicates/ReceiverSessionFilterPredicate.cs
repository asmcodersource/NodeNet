using NodeNet.NodeNet.Message;
using NodeNet.NodeNetSession.MessageWaiter;
using NodeNet.NodeNetSession.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NodeNet.NodeNetSession.SessionPredicates
{
    public class ReceiverSessionFilterPredicate
    {
        static public MessageFilterPredicate CreateFilter(string targetSession)
        {
            return (messageContext) => Method(messageContext, targetSession);
        }

        static private bool Method(MessageContext messageContext, string targetSession)
        {
            var sessionMsg = JsonSerializer.Deserialize<SessionMessage.SessionMessage>(messageContext.Message.Data);
            if (sessionMsg is null)
                return false;
            return sessionMsg.Info.OppositeSessionId == targetSession;
        }
    }
}
