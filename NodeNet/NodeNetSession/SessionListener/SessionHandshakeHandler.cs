using NodeNet.NodeNet;
using NodeNet.NodeNetSession.MessageWaiter;
using NodeNet.NodeNetSession.Session;
using NodeNet.NodeNetSession.SessionPredicates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace NodeNet.NodeNetSession.SessionListener
{
    public record HandshakeResponse();

    internal class SessionHandshakeHandler
    {
        private readonly Node node;
        private readonly MessageWaiter.MessageWaiter messageWaiter;
        public readonly string ListeningResource;

        public SessionHandshakeHandler(Node node, string listeningResource)
        {
            this.node = node;
            ListeningResource = listeningResource;
            messageWaiter = new MessageWaiter.MessageWaiter(node);
            messageWaiter.MessageFilterPredicate = MessageFilterAndPredicate.And(
                MessageWhenDataIsPredicate<SessionMessage.SessionMessage>.CreateFilter(),
                MessageReceiverFilterPredicate.CreateFilter(node.SignOptions.PublicKey)
            );
        }

        public void StartMessageListening()
        {
            messageWaiter.IsAllowListening = true;
        }

        public void StopMessageListening()
        {
            messageWaiter.IsAllowListening = false;
        }

        public async Task<Session.Session> HandleNextRequest()
        {
            return await HandleNextRequest(CancellationToken.None);
        }

        public async Task<Session.Session> HandleNextRequest(CancellationToken cancellationToken)
        {
            // Wait for handshake request, and accept it
            var sessionMsgContext = await messageWaiter.WaitForMessage(cancellationToken);
            if (sessionMsgContext.SessionMessage is null)
                throw new OperationCanceledException();
            var handshakeRequest = JsonSerializer.Deserialize<HandshakeRequest>(sessionMsgContext.SessionMessage.Data);
            if( handshakeRequest is null )
                throw new OperationCanceledException();
            if (handshakeRequest.Resource != ListeningResource)
                throw new OperationCanceledException();
            // Handshake accepted
            var session = new Session.Session(
                node, 
                sessionMsgContext.MessageContext.Message.Info.SenderPublicKey, 
                sessionMsgContext.SessionMessage.Info.SenderSessionId, 
                ListeningResource
            );
            var handshakeResponseJson = JsonSerializer.Serialize(new HandshakeResponse());
            var sessionMessage = new SessionMessage.SessionMessage(
                   new SessionMessage.SessionMessageInfo(session.OppositeSessionId, session.CurrentSessionId),
                   handshakeResponseJson
            );
            var sessionMessageJson = JsonSerializer.Serialize(sessionMessage);
            await node.SendMessageAsync(sessionMessageJson, sessionMsgContext.MessageContext.Message.Info.SenderPublicKey);
            return session;
        }
    }
}
