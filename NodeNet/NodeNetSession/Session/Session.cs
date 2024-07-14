using NodeNet.NodeNet;
using System.Collections.Concurrent;
using NodeNet.NodeNet.Message;
using NodeNet.NodeNetSession.MessageWaiter;
using NodeNet.NodeNetSession.SessionPredicates;
using System.Text.Json;

namespace NodeNet.NodeNetSession.Session
{

    public class Session: IDisposable
    {
        public string? OppositeSessionId { get; protected set; } = null; // Mean session id of opposite side
        public string CurrentSessionId { get; protected set; } // Mean session id of current side
        public string? Resource { get; protected set; } = null;
        public string? ReceiverPublicKey { get; protected set; } = null;
        public SessionState State { get; protected set; } = SessionState.Created;
        public MessageWaiter.MessageWaiter SessionMessageWaiter { get; protected set; } = new MessageWaiter.MessageWaiter();

        private readonly Node wrappedNode;


        public Session()
        {
            CurrentSessionId = GetHashCode().ToString();
        }

        /// <summary>
        /// Creates a new session object that can be initialized by a connection or handle a received connection request.
        /// </summary>
        /// <param name="wrappedNode">The node that will be used to communicate</param>
        public Session(Node wrappedNode) : this()
        {
            this.wrappedNode = wrappedNode;
        }

        public Session(Node wrappedNode, string receiverPublicKey, string oppositeSessionId, string resource ) : this()
        {
            this.wrappedNode = wrappedNode;
            OppositeSessionId = oppositeSessionId;
            ReceiverPublicKey = receiverPublicKey;
            Resource = resource;
            SessionMessageWaiter = new MessageWaiter.MessageWaiter(wrappedNode);
            SessionMessageWaiter.MessageFilterPredicate = MessageFilterAndPredicate.And(
                MessageSenderFilterPredicate.CreateFilter(receiverPublicKey),
                MessageReceiverFilterPredicate.CreateFilter(wrappedNode.SignOptions.PublicKey),
                ReceiverSessionFilterPredicate.CreateFilter(CurrentSessionId)
            );
            SessionMessageWaiter.IsAllowListening = true;
            ChangeState(SessionState.Established);
        }

        public void Dispose()
        {
            SessionMessageWaiter.IsAllowListening = false;
            ChangeState(SessionState.Disconnected);
        }

        public async Task<ConnectionResult> Connect(string receiverPublicKey, string resource)
        {
            return await Connect(receiverPublicKey, resource, CancellationToken.None);
        }

        public async Task<ConnectionResult> Connect(string receiverPublicKey, string resource, CancellationToken cancellationToken)
        {
            ChangeState(SessionState.WaitingForHandshake);
            var handshakeRequestTask = new SessionHandshakeAction(wrappedNode, CurrentSessionId, receiverPublicKey, resource);
            try {
                Resource = resource;
                var oppositeSideSession = await handshakeRequestTask.MakeHandshake(cancellationToken);
                ReceiverPublicKey = receiverPublicKey;
                OppositeSessionId = oppositeSideSession;
                SessionMessageWaiter = new MessageWaiter.MessageWaiter(wrappedNode);
                SessionMessageWaiter.MessageFilterPredicate = MessageFilterAndPredicate.And(
                    MessageSenderFilterPredicate.CreateFilter(receiverPublicKey),
                    MessageReceiverFilterPredicate.CreateFilter(wrappedNode.SignOptions.PublicKey),
                    ReceiverSessionFilterPredicate.CreateFilter(CurrentSessionId)
                );
                SessionMessageWaiter.IsAllowListening = true;
                ChangeState(SessionState.Established);
                return ConnectionResult.Connected;
            } catch
            {
                ChangeState(SessionState.Faulted);
                return ConnectionResult.Fault;
            }
        }


        public async Task<MessageContext?> WaitForMessage()
        {
            return await SessionMessageWaiter.WaitForMessage(CancellationToken.None);
        }

        public async Task<MessageContext?> WaitForMessage(CancellationToken cancellationToken)
        {
            if (State != SessionState.Established)
                throw new Exception("Session is not established for communication");
            return await SessionMessageWaiter.WaitForMessage(cancellationToken);
        }
        
        public void SendMessage(string data)
        {
            if (State != SessionState.Established)
                throw new Exception("Session is not established for communication");
            var sessionMessage = new SessionMessage.SessionMessage(
                new SessionMessage.SessionMessageInfo(OppositeSessionId, CurrentSessionId),
                data
            );
            var sessionMessageJson = JsonSerializer.Serialize(sessionMessage);
            wrappedNode.SendMessage(sessionMessageJson, ReceiverPublicKey);
        }

        protected void ChangeState(SessionState state)
        {
            State = state;
        }
    }
}
