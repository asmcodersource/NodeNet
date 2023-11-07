using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NodeNet.NodeNet.Communication;

namespace NodeNet.NodeNet.HttpCommunication
{
    internal class NodeHttpConnection : INodeConnection
    {
        public bool IsListening { get; set; } = false;
        protected Task? ListeningTask { get; set; } = null;
        public WebSocket WebSocket { get; protected set; }
        protected Queue<Message.Message> MessagesQueue = new Queue<Message.Message>();
        public event Action<INodeConnection> MessageReceived;
        public event Action<INodeConnection> WebSocketClosed;


        public NodeHttpConnection()
        {
            WebSocket = null;
        }

        public NodeHttpConnection(HttpListenerWebSocketContext context)
        {
            WebSocket = context.WebSocket;
        }
        
        public bool Connect( string addr )
        {
            ClientWebSocket clientWebSocket = new ClientWebSocket();
            clientWebSocket.ConnectAsync(new Uri(addr), CancellationToken.None).Wait();
            if (clientWebSocket.State != WebSocketState.Open)
                return false;
            WebSocket = clientWebSocket as WebSocket;
            return true;
        }

        public Message.Message? GetLastMessage()
        {
            return MessagesQueue.Count != 0 ? MessagesQueue.Dequeue() : null;
        }

        public List<Message.Message> GetMessageList()
        {
            var messageList = MessagesQueue.ToList<Message.Message>();
            MessagesQueue.Clear();
            return messageList;
        }

        public void ListenMessages()
        {
            IsListening = true;
            Task.Run(() => MessageListener());
        }

        public void SendMessage(Message.Message message)
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            var segment = new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonMessage));
            WebSocket.SendAsync(segment, WebSocketMessageType.Binary, true, CancellationToken.None).Wait();
        }

        public void CloseConnection()
        {
            if (WebSocket == null)
                throw new Exception("Socket is not connected");
            if (IsListening)
                WebSocketClosed?.Invoke(this);
            IsListening = false;
            
        }

        protected async Task MessageListener() {
            var buffer = new ArraySegment<byte>(new byte[1024*16]);
            try
            {
                while (IsListening)
                {
                    var result = await WebSocket.ReceiveAsync(buffer, CancellationToken.None);
                    if (result.CloseStatus == WebSocketCloseStatus.Empty)
                        break;
                    if (result.EndOfMessage != true)
                        continue;
                    try
                    {
                        var jsonString = Encoding.UTF8.GetString(buffer.Take(result.Count).ToArray());
                        var message = JsonSerializer.Deserialize<Message.Message>(jsonString);
                        Array.Fill<byte>(buffer.Array, 0, 0, 1024 * 16);
                        AddMessageToQueue(message);
                        MessageReceived?.Invoke(this);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            } catch (Exception ex)
            {
            }
            CloseConnection();
        }

        protected void AddMessageToQueue(Message.Message message)
        {
            MessagesQueue.Enqueue(message);
            MessageReceived?.Invoke(this);
        }
    }
}
