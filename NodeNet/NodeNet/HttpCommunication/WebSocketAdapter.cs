using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet.HttpCommunication
{
    internal class WebSocketAdapter: IWebSocket
    {
        protected WebSocket socket = null;

        public Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> bytes, CancellationToken cancellationToken)
        {
            return socket.ReceiveAsync(bytes, cancellationToken);
        }

        public Task SendAsync(ArraySegment<byte> bytes, WebSocketMessageType type, bool endOfMessage, CancellationToken cancellationToken)
        {
            return socket.SendAsync(bytes, type, endOfMessage, cancellationToken);
        }

        public WebSocketAdapter(WebSocket webSocket)
        {
            socket = webSocket;
        }

        public object GetInnerObject()
        {
            return socket;
        }
    }
}
