using NodeNet.NodeNet.Message;
using NodeNet.NodeNet.RSASigner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet.ReceiveMiddleware
{
    internal class MessageCacheMiddleware
    {
        public IReceiveMiddleware Next { get; protected set; } = null;

        public bool Invoke(MessageContext messageContext)
        {
            // TODO...
        }

        public void SetNext(IReceiveMiddleware next)
        {
            Next = next;
        }
    }
}
