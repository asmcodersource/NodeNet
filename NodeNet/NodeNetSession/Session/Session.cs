using NodeNet.NodeNet;
using System.Collections.Concurrent;
using NodeNet.NodeNet.Message;

namespace NodeNet.NodeNetSession.Session
{

    public class Session
    {
        public string? SessionId { get; protected set; } = null;
        public string? Resource { get; protected set; } = null;
        public string? ReceiverPublicKey { get; protected set; } = null;
        public SessionState State { get; protected set; } = SessionState.Created;
        public ConcurrentQueue<MessageContext> MessageQueue { get; protected set; } = new ConcurrentQueue<MessageContext>();

        private readonly Node wrappedNode;
        private readonly MessageWaiter.MessageWaiter outerMessageWaiter;
        private readonly MessageWaiter.MessageWaiter iternalMessageWaiter;

        /// <summary>
        /// Creates a new session object that can be initialized by a connection or handle a received connection request.
        /// </summary>
        /// <param name="wrappedNode">The node that will be used to communicate</param>
        public Session(Node wrappedNode)
        {
            this.wrappedNode = wrappedNode;
            this.iternalMessageWaiter = new MessageWaiter.MessageWaiter(wrappedNode);
            this.outerMessageWaiter = new MessageWaiter.MessageWaiter();
        }

        public Session(Node wrappedNode, string receiver, string session, string resource )
        {
            this.wrappedNode = wrappedNode;
            this.iternalMessageWaiter = new MessageWaiter.MessageWaiter(wrappedNode);
            this.outerMessageWaiter = new MessageWaiter.MessageWaiter();
            ReceiverPublicKey = receiver;
            Resource = resource;
            SessionId = session;
            ChangeState(SessionState.Established);
        }

        public async Task<ConnectionResult> Connect(string receiverPublicKey, string resource)
        {
            return await Connect(receiverPublicKey, resource, CancellationToken.None);
        }

        public async Task<ConnectionResult> Connect(string receiverPublicKey, string resource, CancellationToken cancellationToken)
        {
            ChangeState(SessionState.WaitingForHandshake);
            var handshakeRequestTask = new SessionHandshakeAction(wrappedNode);
            var handshakeRequestResult = await handshakeRequestTask.MakeHandshake(receiverPublicKey, resource, this.GetHashCode().ToString(), cancellationToken);
            if (handshakeRequestResult is not true)
            {
                ChangeState(SessionState.Faulted);
                return ConnectionResult.Fault;
            }
            ReceiverPublicKey = receiverPublicKey;
            Resource = resource;
            ChangeState(SessionState.Established);
            return ConnectionResult.Connected;
        }

        protected void ChangeState(SessionState state)
        {
            State = state;
        }
    }
}
