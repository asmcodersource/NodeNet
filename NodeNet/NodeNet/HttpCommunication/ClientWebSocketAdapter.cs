using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet.HttpCommunication
{
    internal class ClientWebSocketAdapter : IWebSocket
    {
        ClientWebSocket clientWebSocket;

        public ClientWebSocketAdapter(ClientWebSocket clientWebSocket)
        {
            this.clientWebSocket = clientWebSocket;
        }

        public Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> bytes, CancellationToken cancellationToken)
        {
            return clientWebSocket.ReceiveAsync(bytes, cancellationToken);
        }

        public Task SendAsync(ArraySegment<byte> bytes, WebSocketMessageType type, bool endOfMessage, CancellationToken cancellationToken)
        {
            return clientWebSocket.SendAsync(bytes, type, endOfMessage, cancellationToken);
        }

        public object GetInnerObject()
        {
            return clientWebSocket;
        }
    }
}
