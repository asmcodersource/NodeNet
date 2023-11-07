using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet.HttpCommunication
{
    internal interface IWebSocket
    {
        public object GetInnerObject();
        public Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> bytes, CancellationToken cancellationToken);
        public Task SendAsync(ArraySegment<byte> bytes, WebSocketMessageType type, bool endOfMessage, CancellationToken cancellationToken);
    }
}
