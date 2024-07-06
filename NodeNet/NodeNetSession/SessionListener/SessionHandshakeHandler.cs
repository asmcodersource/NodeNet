using NodeNet.NodeNet;
using NodeNet.NodeNetSession.MessageWaiter;
using NodeNet.NodeNetSession.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace NodeNet.NodeNetSession.SessionListener
{
    public record HandshakeResponse(string Session);

    internal class SessionHandshakeHandler
    {
        private readonly Node node;
        private readonly MessageWaiter.MessageWaiter messageWaiter;

        public SessionHandshakeHandler(Node node)
        {
            this.node = node;
            messageWaiter = new MessageWaiter.MessageWaiter(node);
            messageWaiter.MessageFilterPredicate = WhenDataIsPredicate<HandshakeRequest>.MessageFilterPredicate;
        }

        public void StartMessageListening()
        {
            messageWaiter.IsAllowListening = true;
        }

        public void StopMessageListening()
        {
            messageWaiter.IsAllowListening = false;
        }

        public async Task<Session.Session> HandleNextRequest(string listeningResource)
        {
            return await HandleNextRequest(listeningResource, CancellationToken.None);
        }

        public async Task<Session.Session> HandleNextRequest(string listeningResource, CancellationToken cancellationToken)
        {
            // Wait for handshake request, and accept it
            var handshakeMessage = await messageWaiter.WaitForMessage();
            var handshakeJsonDocument = JsonDocument.Parse(handshakeMessage.Message.Data);
            var handshakeRequest = handshakeJsonDocument.Deserialize<HandshakeRequest>();
            if (handshakeRequest is null)
                throw new OperationCanceledException();
            if( handshakeRequest.Resource != listeningResource )
                throw new OperationCanceledException();
            if( handshakeMessage.Message.Info.ReceiverPublicKey != node.SignOptions.PublicKey )
                throw new OperationCanceledException();
            // Make handshake response, send it back
            var handshakeResponse = new HandshakeResponse(handshakeRequest.Session);
            var handshakeResponseRawJson = JsonSerializer.Serialize(handshakeResponse);
            await node.SendMessage(handshakeResponseRawJson, handshakeMessage.Message.Info.SenderPublicKey);
            return new Session.Session(node, handshakeMessage.Message.Info.SenderPublicKey, handshakeResponse.Session, handshakeRequest.Resource);
        }
    }
}
