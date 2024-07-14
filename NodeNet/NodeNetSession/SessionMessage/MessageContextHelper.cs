using NodeNet.NodeNet.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace NodeNet.NodeNetSession.SessionMessage
{
    public static class MessageContextHelper
    {
        public static T? Parse<T>(MessageContext messageContext)
        {
            var sessionMessage = JsonSerializer.Deserialize<SessionMessage>(messageContext.Message.Data);
            if (sessionMessage is null)
                return default(T);
            return JsonSerializer.Deserialize<T>(sessionMessage.Data);
        }

        public static string? GetSessionMessageData(MessageContext messageContext)
        {
            return JsonSerializer.Deserialize<SessionMessage>(messageContext.Message.Data)?.Data;
        }
    }
}
