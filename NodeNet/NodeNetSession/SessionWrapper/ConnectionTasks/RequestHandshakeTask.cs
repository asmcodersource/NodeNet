using NodeNet.NodeNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNetSession.SessionWrapper.ConnectionTasks
{
    internal class RequestHandshakeTask
    {
        private readonly Node node;
        private readonly MessageWaiter.MessageWaiter messageWaiter;

        public RequestHandshakeTask(Node node, MessageWaiter.MessageWaiter messageWaiter)
        {
            this.node = node;
            this.messageWaiter = messageWaiter;
        }

        public async Task<bool> Execute()
        {
            return true;
        }
    }
}
