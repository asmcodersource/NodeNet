using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNet.NodeNet;

namespace NodeNet.NodeNetSession.SessionWrapper.ConnectionTasks
{
    internal class HandshakeTask
    {
        private readonly Node node;
        private readonly MessageWaiter.MessageWaiter messageWaiter;

        public HandshakeTask(Node node, MessageWaiter.MessageWaiter messageWaiter) 
        {
            this.messageWaiter = messageWaiter;
            this.node = node;
        }

        public async Task<bool> Execute()
        {
            return true;
        }
    }
}
