using NodeNet.NodeNet.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace NodeNet.NodeNetSession.SessionMessage
{
    public static class MessageContextHelper<T>
    {
        public static T? Parse(MessageContext messageContext)
        {
            var sessionMessage = JsonSerializer.Deserialize<SessionMessage>(messageContext.Message.Data);
            if (sessionMessage is null)
                return default(T);
            return JsonSerializer.Deserialize<T>(sessionMessage.Data);
        }
    }
}
