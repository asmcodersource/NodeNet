using NodeNet.NodeNet.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet.ReceiveMiddleware
{
    // Every middleware have to perform some actions on received message
    // Its separate logic of different actions, and relations
    // Last middleware returns true if message have to be accepted
    internal interface IReceiveMiddleware
    {
        public void SetNode(Node node);
        public bool Invoke(MessageContext messageContext);
        public void SetNext(IReceiveMiddleware next);
    }
}
