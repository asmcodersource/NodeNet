using NodeNet.NodeNet.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNetSession.SessionMessage
{
    public class SessionMessageContext
    {
        public readonly MessageContext MessageContext;
        public readonly SessionMessage SessionMessage;

        public SessionMessageContext(MessageContext messageContext, SessionMessage sessionMessage) { 
            this.MessageContext = messageContext;
            this.SessionMessage = sessionMessage;
        }
    }
}
