using NodeNet.NodeNet.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NodeNet.NodeNetSession.SessionMessage
{
    public static class SessionMessageHelper<T>
    {

        public static T? Parse( SessionMessage sessionMessage)
        {
            return JsonSerializer.Deserialize<T>(sessionMessage.Data);
        }
    }
}
