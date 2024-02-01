using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NodeNet.NodeNet.Communication;

namespace NodeNet.NodeNet.TcpCommunication
{
    internal class NodeTcpConnection : INodeConnection
    {
        public bool IsListening { get; set; } = false;
        protected Task? ListeningTask { get; set; } = null;
        public TcpClient TcpClient { get; protected set; }
        public ITcpAddressProvider TcpAddressProvider { get; set; }

        protected Queue<Message.Message> MessagesQueue = new Queue<Message.Message>();
        public event Action<INodeConnection> MessageReceived;
        public event Action<INodeConnection> ConnectionClosed;


        public NodeTcpConnection()
        {
            TcpClient = new TcpClient();
        }

        public NodeTcpConnection(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
        }
        
        public bool Connect( string addr )
        {
            string ip = addr.Split(":")[0];
            string port = addr.Split(":")[1];
            try
            {
                TcpClient.Connect(ip, Convert.ToInt32(port));
                return true;
            } catch (Exception ex) { }
            return false;
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
            var stream = TcpClient.GetStream();
            stream.Write(segment);
        }

        public async Task SendRawData(byte[] data, CancellationToken cancellationToken)
        {
            await TcpClient.GetStream().WriteAsync(data, cancellationToken);
        }

        public async Task<byte[]> ReceiveRawData(CancellationToken cancellationToken)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[4096]);
            try
            {
                var size = await TcpClient.GetStream().ReadAsync(buffer, cancellationToken);
                return buffer.Array.Take(size).ToArray();
            }
            catch (Exception ex)
            {
                throw new WebSocketException("Socket closed");
            }
        }

        public void CloseConnection()
        {
            if (TcpClient == null)
                throw new Exception("Socket is not connected");
            if (IsListening)
                ConnectionClosed?.Invoke(this);
            IsListening = false;
            
        }

        protected async Task MessageListener() {
            // TODO: verify disconnect execution
            var buffer = new ArraySegment<byte>(new byte[1024*16]);
            try
            {
                while (IsListening)
                {
                    var size = await TcpClient.GetStream().ReadAsync(buffer, CancellationToken.None);
                    try
                    {
                        var jsonString = Encoding.UTF8.GetString(buffer.Take(size).ToArray());
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

        public string GetConnectionAddress()
        {
            if (TcpClient == null)
                throw new Exception("Connections is not initialized");
            if (TcpClient.Connected == false)
                throw new Exception("Connection is not active");

            string clientAddress = ((IPEndPoint)TcpClient.Client.LocalEndPoint).Address.MapToIPv4().ToString();
            int clientPort = TcpAddressProvider.GetNodeTcpPort();
            return clientAddress + ":" + clientPort;
        }
    }
}
