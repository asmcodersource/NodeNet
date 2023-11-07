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
        public IWebSocket WebSocket { get; protected set; }
        protected Queue<Message.Message> MessagesQueue = new Queue<Message.Message>();
        public event Action<INodeReceiver> MessageReceived;

        public NodeHttpConnection()
        {
            WebSocket = null;
        }

        public NodeHttpConnection(HttpListenerWebSocketContext context)
        {
            WebSocket = new WebSocketAdapter(context.WebSocket);
        }
        
        public async Task<bool> Connect( string addr )
        {
            ClientWebSocket clientWebSocket = new ClientWebSocket();
            await clientWebSocket.ConnectAsync(new Uri(addr), CancellationToken.None);
            if (clientWebSocket.State != WebSocketState.Open)
                return false;
            WebSocket = new ClientWebSocketAdapter(clientWebSocket);
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
            WebSocket.SendAsync(segment, WebSocketMessageType.Binary, true, CancellationToken.None);
        }

        protected async Task MessageListener() {
            var buffer = new ArraySegment<byte>();
            try
            {
                while (IsListening)
                {
                    var result = await WebSocket.ReceiveAsync(buffer, CancellationToken.None);
                    if (result.EndOfMessage != true)
                        continue;

                    try
                    {
                        var jsonString = Encoding.UTF8.GetString(buffer.ToArray<byte>());
                        JsonSerializer.Deserialize<Message.Message>(jsonString);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            } catch (Exception ex)
            {
                IsListening = false;
            }
        }

        protected void AddMessageToQueue(Message.Message message)
        {
            MessagesQueue.Enqueue(message);
            MessageReceived?.Invoke(this);
        }
    }
}
