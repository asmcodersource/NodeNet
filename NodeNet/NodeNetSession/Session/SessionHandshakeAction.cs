using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NodeNet.NodeNet;
using NodeNet.NodeNet.Message;
using NodeNet.NodeNetSession.MessageWaiter;
using NodeNet.NodeNetSession.SessionListener;
using NodeNet.NodeNetSession.SessionPredicates;
using Serilog.Context;

namespace NodeNet.NodeNetSession.Session
{
    public record HandshakeRequest(string Resource);

    public class SessionHandshakeAction
    {
        private readonly Node node;
        private readonly MessageWaiter.MessageWaiter messageWaiter;
        public readonly string TargetPublicKey;
        public readonly string TargetResource;
        public readonly string CurrentSessionId;

        public SessionHandshakeAction(Node node, string currentSessionId, string targetPublicKey, string resource)
        {
            this.node = node;
            messageWaiter = new MessageWaiter.MessageWaiter(node);
            messageWaiter.MessageFilterPredicate = MessageFilterAndPredicate.And(
                MessageWhenDataIsPredicate<SessionMessage.SessionMessage>.CreateFilter(),
                MessageReceiverFilterPredicate.CreateFilter(node.SignOptions.PublicKey),
                MessageSenderFilterPredicate.CreateFilter(targetPublicKey),
                ReceiverSessionFilterPredicate.CreateFilter(currentSessionId)
            );
            TargetPublicKey = targetPublicKey;
            TargetResource = resource;
            CurrentSessionId = currentSessionId;
        }

        public async Task<string> MakeHandshake()
        {
            return await MakeHandshake(CancellationToken.None); 
        }

        public async Task<string> MakeHandshake(CancellationToken cancellationToken)
        {
            messageWaiter.ClearQueue();
            messageWaiter.IsAllowListening = true;
            // Send handshake request
            var handshakeRequest = new HandshakeRequest(TargetResource);
            var handshakeRequestJsonRaw = JsonSerializer.Serialize(handshakeRequest);
            var sessionMessage = new SessionMessage.SessionMessage(
                new SessionMessage.SessionMessageInfo("", CurrentSessionId),
                handshakeRequestJsonRaw
            );
            var sessionMessageJson = JsonSerializer.Serialize(sessionMessage);
            await node.SendMessage(sessionMessageJson, TargetPublicKey);
            // Receive handshake response
            var responseMsgContext = await messageWaiter.WaitForMessage(cancellationToken);
            var sessionMsgDocument = JsonDocument.Parse(responseMsgContext.Message.Data);
            var sessionMsg = sessionMsgDocument.Deserialize<SessionMessage.SessionMessage>();
            messageWaiter.IsAllowListening = false;
            return sessionMsg.Info.SenderSessionId;
        }
    }
}
