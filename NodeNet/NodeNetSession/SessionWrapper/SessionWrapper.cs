using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNet.NodeNet;
using NodeNet.NodeNet.TcpCommunication;
using NodeNet.NodeNet.RSAEncryptions;
using System.Collections.Concurrent;
using NodeNet.NodeNet.Message;
using NodeNet.NodeNetSession.SessionWrapper.ConnectionTasks;
using NodeNet.NodeNetSession.MessageWaiter;

namespace NodeNet.SessionWrapper.SessionWrapper
{

    internal class SessionWrapper
    {
        public string? SessionId { get; protected set; } = null;
        public string? ReceiverPublicKey { get; protected set; } = null;
        public SessionState State { get; protected set; } = SessionState.Created;
        public ConcurrentQueue<MessageContext> MessageQueue { get; protected set; } = new ConcurrentQueue<MessageContext>();

        private readonly Node wrappedNode;
        private readonly MessageWaiter messageWaiter;

        /// <summary>
        /// Creates a new session object that can be initialized by a connection or handle a received connection request.
        /// </summary>
        /// <param name="wrappedNode">The node that will be used to communicate</param>
        public SessionWrapper(Node wrappedNode)
        {
            this.wrappedNode = wrappedNode;
            this.messageWaiter = new MessageWaiter(wrappedNode);
        }

        public async Task<ConnectionResult> Connect(string ReceiverPublicKey)
        {
            var handshakeRequestTask = new RequestHandshakeTask(wrappedNode, messageWaiter);
            var handshakeRequestResult = await handshakeRequestTask.Execute();
            if (handshakeRequestResult is not true)
                return ConnectionResult.Fault;
            return ConnectionResult.Connected;
        }
    }
}
