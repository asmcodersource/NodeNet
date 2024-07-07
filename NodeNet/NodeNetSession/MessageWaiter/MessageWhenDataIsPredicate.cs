using NodeNet.NodeNet.Message;
using NodeNet.NodeNetSession.SessionListener;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NodeNet.NodeNetSession.MessageWaiter
{
    // I did something weird here...
    public static class MessageWhenDataIsPredicate<T>
    {
        static public MessageFilterPredicate CreateFilter()
        {
            return MessageWhenDataIsPredicate<T>.Method;
        }

        static private bool Method(MessageContext messageContext)
        {
            try
            {
                var jsonDocument = JsonDocument.Parse(messageContext.Message.Data);
                var parsedObject = jsonDocument.Deserialize<T>();
                return parsedObject is not null;
            }
            catch (Exception exception) when (
                exception.GetType() == typeof(JsonException) ||
                exception.GetType() == typeof(NotSupportedException) ||
                exception.GetType() == typeof(InvalidOperationException)
            ) { }
            return false;
        }
    }
}
