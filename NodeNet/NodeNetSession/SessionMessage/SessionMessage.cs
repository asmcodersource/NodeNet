using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NodeNet.NodeNetSession.SessionMessage
{
    [Serializable]
    public class SessionMessage
    {
        public SessionMessageInfo Info { get; set; } 
        public string Data { get; set; }

        public SessionMessage() { }

        public SessionMessage(SessionMessageInfo info, string data = "")
        {
            Info = info;
            Data = data;
        }
    }
}
