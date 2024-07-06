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

namespace NodeNet.NodeNetSession.Session
{
    public record HandshakeRequest(string Resource, string Session);

    public class SessionHandshakeAction
    {
        private readonly Node node;
        private readonly MessageWaiter.MessageWaiter messageWaiter;

        public SessionHandshakeAction(Node node)
        {
            this.node = node;
            messageWaiter = new MessageWaiter.MessageWaiter(node);
            messageWaiter.MessageFilterPredicate = WhenDataIsPredicate<HandshakeResponse>.MessageFilterPredicate;
        }

        public async Task<bool> MakeHandshake(string target, string targetResource, string session)
        {
            return await MakeHandshake(target, targetResource, session, CancellationToken.None); 
        }

        public async Task<bool> MakeHandshake(string target, string targetResource, string session, CancellationToken cancellationToken)
        {
            messageWaiter.IsAllowListening = true;
            try
            {
                // Send handshake request
                var handshakeRequest = new HandshakeRequest(targetResource, session);
                var jsonRaw = JsonSerializer.Serialize(handshakeRequest);
                await node.SendMessage(jsonRaw, target);
                // Receive handshake response
                HandshakeResponse? handshakeResponse = null;
                do
                {
                    var responseMsgContext = await messageWaiter.WaitForMessage(cancellationToken);
                    if (responseMsgContext.Message.Info.SenderPublicKey != target)
                        continue;
                    var responseJsonDocument = JsonDocument.Parse(responseMsgContext.Message.Data);
                    handshakeResponse = responseJsonDocument.Deserialize<HandshakeResponse>();
                    if (handshakeResponse is null)
                        return false;
                } while (string.Equals(handshakeResponse.Session, session) is not true );
                return true;
            }
            catch { /* In case of any exception, just take session as faulted connection */ }
            messageWaiter.IsAllowListening = false;
            return false;
        }
    }
}
