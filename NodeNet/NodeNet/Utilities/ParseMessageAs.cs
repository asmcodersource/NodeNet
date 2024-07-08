using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NodeNet.NodeNet.Message;

namespace NodeNet.NodeNet.Utilities
{
    public static class MessageAs<T>
    {
        public static T? Parse(MessageContext messageContext)
        {
            var document = JsonDocument.Parse(messageContext.Message.Data);
            return document.Deserialize<T>();
        }
    }
}
