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
    public class SenderSessionFilterPredicate
    {
        static public MessageFilterPredicate CreateFilter(string targetSession)
        {
            return (messageContext) => Method(messageContext.SessionMessage, targetSession);
        }

        static private bool Method(SessionMessage.SessionMessage sessionMessage, string targetSession)
        {
            return sessionMessage.Info.SenderSessionId == targetSession;
        }
    }
}
