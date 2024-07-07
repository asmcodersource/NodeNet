using NodeNet.NodeNet.Message;
using NodeNet.NodeNetSession.MessageWaiter;
using NodeNet.NodeNetSession.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NodeNet.NodeNetSession.SessionPredictes
{
    public class ReceiverSessionFilterPredicate
    {
        static public MessageFilterPredicate CreateFilter(string targetSession)
        {
            return (messageContext) => Method(messageContext, targetSession);
        }

        static private bool Method(MessageContext messageContext, string targetSession)
        {
            var sessionMsgJsonDocument = JsonDocument.Parse(messageContext.Message.Data);
            var sessionMsg = sessionMsgJsonDocument.Deserialize<SessionMessage.SessionMessage>();
            if (sessionMsg is null)
                return false;
            if (sessionMsg.Info.OppositeSessionId == targetSession)
                return false;
            return true;
        }
    }
}
