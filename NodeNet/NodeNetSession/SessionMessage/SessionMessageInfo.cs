using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNetSession.SessionMessage
{
    [Serializable]
    public class SessionMessageInfo
    {
        public string OppositeSessionId { get; set; }
        public string SenderSessionId { get; set; }

        public SessionMessageInfo() { }

        public SessionMessageInfo(string oppositeSessionId, string senderSessionId)
        {
            OppositeSessionId = oppositeSessionId;
            SenderSessionId = senderSessionId;
        }
    }
}
